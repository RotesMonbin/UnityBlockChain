using ETH_Identicons;
using Nethereum.HdWallet;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.KeyStore;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ProviderManager : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SendExternalTransaction(string to,string value,string gas,string gasPrice,string data);

    [DllImport("__Internal")]
    private static extern void SyncFiles();

    [DllImport("__Internal")]
    private static extern bool GetProvider();

    [DllImport("__Internal")]
    private static extern string GetExternalAddress();

#else
    bool GetProvider() { return false; }
    string GetExternalAddress() { return ""; }
    void SyncFiles() { Debug.Log("File sync"); }
    void SendExternalTransaction(string to, string value, string gas, string gasPrice, string data) { Debug.Log("Supposed to be launch on metamask, request rejected"); }
#endif

    [SerializeField]
    private string url = "https://ropsten.infura.io";

    [SerializeField]
    private Image identiconImage;

    [SerializeField]
    private SignTransactionManager transactionManager;

    internal static ProviderManager instance { get; private set; }

    internal string accountAddress = "";
    private string keystoreJSON = "";
    internal float ethereumBalance = 0;
    internal bool usingIntegratedProvider;

    internal Wallet m_wallet = null;

    private void Awake()
    {
        instance = this;
        usingIntegratedProvider = GetProvider();
        if (usingIntegratedProvider)
        {
            var test = GetExternalAddress();
            SetCurrentAddress(GetExternalAddress());
        }
    }


    #region Wallet & Keystore


    public void InstantiateWalletAndSaveOnDisk(String wordsChain, String password)
    {
        m_wallet = new Wallet(wordsChain, null);

        SetCurrentAddress(EthECKey.GetPublicAddress(ProviderManager.instance.m_wallet.GetWalletPrivateKeyAsString()));

        KeyStoreService m_keystoreService = new KeyStoreService();

        Debug.Log(Application.persistentDataPath);

        string keystoreJSON = m_keystoreService.EncryptAndGenerateDefaultKeyStoreAsJson(password, ProviderManager.instance.m_wallet.GetWalletPrivateKeyAsByte(), accountAddress);

        SaveJSONOnDisk(keystoreJSON, Application.persistentDataPath + "/keystore.ks");
    }

    private bool SaveJSONOnDisk(string toSave, string path)
    {

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.OpenOrCreate);

        if (file == null)
        {
            Debug.LogError("Failed to open the save file");
            return false;
        }

        bf.Serialize(file, toSave);
        file.Close();

        //SyncFiles();

        return true;
    }


    /// <summary>
    /// Try to load keystore from disk, store the content in keystoreJSON, return true if succeded, false if error
    /// </summary>
    /// <returns></returns>
    public bool LoadJSONFromDisk()
    {
        if (File.Exists(Application.persistentDataPath + "/keystore.ks"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/keystore.ks", FileMode.Open);

            if (file == null)
            {
                Debug.LogError("Failed to open the save file");
                return false;
            }

            keystoreJSON = (string)bf.Deserialize(file);
            file.Close();

            if (keystoreJSON == null || keystoreJSON == "")
            {
                Debug.LogError("Failed to deserialize the save file");
                return false;
            }


            RetrieveAddressFromKeystore();
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// Set the account address according to the keyStore found
    /// </summary>
    /// <returns></returns>
    public bool RetrieveAddressFromKeystore()
    {
        if (keystoreJSON == null || keystoreJSON == "")
            return false;
        KeyStoreService m_keystoreService = new KeyStoreService();
        SetCurrentAddress(m_keystoreService.GetAddressFromKeyStore(keystoreJSON));
        return true;
    }

    public void RemoveKeyStore()  
    {
        keystoreJSON = "";
        SetCurrentAddress("");

        if (File.Exists(Application.persistentDataPath + "/keystore.ks"))
        {
            File.Delete(Application.persistentDataPath + "/keystore.ks");
        }
    }

    private string GetPrivateKeyFromKeystore(string pass)
    {
        if (keystoreJSON == null || keystoreJSON == "" || pass == null || pass == "")
        {
            return "";
        }

        KeyStoreService m_keystoreService = new KeyStoreService();
        byte[] b = m_keystoreService.DecryptKeyStoreFromJson(pass, keystoreJSON);
        EthECKey myKey = new EthECKey(b, true);

        if (myKey.GetPublicAddress() != accountAddress)
        {
            return "";
        }

        return myKey.GetPrivateKey();
    }

    private void CreateBlocky(int resolution = 64)
    {
        Identicon id = new Identicon(accountAddress, 8);
        List<Identicon.PixelData> pixels = id.GetIdenticonPixels(resolution);

        Texture2D texture = new Texture2D(resolution, resolution);

        for (int i = 0; i < pixels.Count; ++i)
        {
            Color col = new Color(pixels[i].RGB[0] / 255f, pixels[i].RGB[1] / 255f, pixels[i].RGB[2] / 255f);
            texture.SetPixel(pixels[i].x, resolution - pixels[i].y - 1, col);
        }

        texture.Apply();

        identiconImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    private void SetCurrentAddress(string address)
    {
        accountAddress = address;
        CreateBlocky();
    }

    #endregion


    public IEnumerator getAccountBalance(System.Action<decimal> callback)
    {
        // Now we define a new EthGetBalanceUnityRequest and send it the testnet url where we are going to
        // (we get EthGetBalanceUnityRequest from the Netherum lib imported at the start)
        var getBalanceRequest = new EthGetBalanceUnityRequest(url);
        // Then we call the method SendRequest() from the getBalanceRequest we created
        // with the address and the newest created block.
        yield return getBalanceRequest.SendRequest(accountAddress, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (getBalanceRequest.Exception == null)
        {
            // We define balance and assign the value that the getBalanceRequest gave us.
            callback(Nethereum.Util.UnitConversion.Convert.FromWei(getBalanceRequest.Result.Value, 18));
            // Finally we execute the callback and we use the Netherum.Util.UnitConversion
            // to convert the balance from WEI to ETHER (that has 18 decimal places)
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new InvalidOperationException("Get balance request failed");
        }
    }

    public void SendTransaction(TransactionInput transac)
    {
        if (usingIntegratedProvider)
        {
            SendExternalTransaction(transac.To, transac.Value.HexValue, transac.Gas.HexValue, transac.GasPrice.HexValue, transac.Data);
        }
        else
        {
            transactionManager.OpenAndSignTransaction(transac);
        }
    }

    public void ConfirmTransactionWithPassword(TransactionInput transac, string password)
    {
        StartCoroutine(SendSignedTransaction(transac, password));
    }

    private IEnumerator SendSignedTransaction(TransactionInput transac, string password)
    {
        string privateKey = GetPrivateKeyFromKeystore(password);
        if (privateKey != "")
        {
            TransactionSignedUnityRequest transactionSignedRequest = new TransactionSignedUnityRequest(url, privateKey, accountAddress);
            yield return transactionSignedRequest.SignAndSendTransaction(transac);

            if (transactionSignedRequest.Exception == null)
            {
                // If we don't have exceptions we just display the result, congrats!
                Debug.Log("Nexium approve submitted: " + transactionSignedRequest.Result);
            }
            else
            {
                // if we had an error in the UnityRequest we just display the Exception error
                Debug.Log("Error submitting Nexium approve: " + transactionSignedRequest.Exception.Message);
            }
        }
        else
        {
            Debug.Log("Can't get private key");
        }

    }

}

