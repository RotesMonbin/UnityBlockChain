using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.Hex.HexTypes;
using Nethereum.HdWallet;
using Nethereum.KeyStore;
using Nethereum.Signer;
using System;
using System.Numerics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ETH_Identicons;
using System.Runtime.InteropServices;

public class AccountExample : MonoBehaviour
{
    public static AccountExample instance { get; private set; }

    public Text debugInfo;

    #region UI Data
    public GameObject Canvas;
    public InputField[] Words = new InputField[12];
    public InputField Password;
    private Sprite sp;
    #endregion

    private KeyStoreService m_keystoreService = new KeyStoreService();

    private Wallet m_wallet = null;

    private string accountAddress = "";
    private string keystoreJSON = "";

    private string _url = "https://ropsten.infura.io";

    private NameContract m_nameContract = new NameContract();

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SyncFiles();
#endif

    private static string GetAccount() { return null; }
    private static string SendTransaction(string to, string data) { return null; }



    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Loader.instance.StartLoad();

        Words[0].text = "";
        Words[1].text = "";
        Words[2].text = "";
        Words[3].text = "";
        Words[4].text = "";
        Words[5].text = "";
        Words[6].text = "";
        Words[7].text = "";
        Words[8].text = "";
        Words[9].text = "";
        Words[10].text = "";
        Words[11].text = "";

        if (LoadJSONFromDisk())
        {
            if (RetrieveAddressFromKeystore())
            {
                // OK
                StartCoroutine(); // Start the contract example

                Canvas.SetActive(false);
                //LobbyManager.instance.Activate("Account", CreateBlocky());
            }
            else
            {
                Debug.LogError("Could not retrieve public address from existing keystore");
            }
        }
        else
        {
            // No keystore
            //LobbyManager.instance.Canvas.SetActive(false);
            Canvas.SetActive(true);
            Canvas.transform.Find("LinkMethod").gameObject.SetActive(true);
            Canvas.transform.Find("MnemonicWords").gameObject.SetActive(false);
        }

        //Loader.instance.StopLoad();
    }

    #region Wallet & Keystore

    public void OpenMnemonic()
    {
        Canvas.transform.Find("LinkMethod").gameObject.SetActive(false);
        Canvas.transform.Find("MnemonicWords").gameObject.SetActive(true);
    }

    public void RetrieveWallet()
    {
        //Loader.instance.StartLoad();

        string wordsChain = Words[0].text + " " + Words[1].text + " " + Words[2].text + " " + Words[3].text + " " + Words[4].text + " " + Words[5].text + " " + Words[6].text + " " + Words[7].text + " " + Words[8].text + " " + Words[9].text + " " + Words[10].text + " " + Words[11].text;
        m_wallet = new Wallet(wordsChain, null);

        accountAddress = EthECKey.GetPublicAddress(m_wallet.GetWalletPrivateKeyAsString());

        keystoreJSON = m_keystoreService.EncryptAndGenerateDefaultKeyStoreAsJson(Password.text, m_wallet.GetWalletPrivateKeyAsByte(), accountAddress);
        SaveJSONOnDisk();

        Canvas.SetActive(false);
        //LobbyManager.instance.Activate("Account", CreateBlocky());

        //Loader.instance.StopLoad();
    }

    private bool SaveJSONOnDisk()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/keystore.ks", FileMode.OpenOrCreate);

        if (file == null)
        {
            Debug.LogError("Failed to open the save file");
            return false;
        }

        bf.Serialize(file, keystoreJSON);
        file.Close();

#if UNITY_WEBGL
            SyncFiles();
#endif

        return true;
    }

    private bool LoadJSONFromDisk()
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

            return true;
        }
        else
            return false;
    }

    private bool RetrieveAddressFromKeystore()
    {
        if (keystoreJSON == null || keystoreJSON == "")
            return false;

        accountAddress = m_keystoreService.GetAddressFromKeyStore(keystoreJSON);
        return true;
    }

    public void RemoveKeyStore()
    {
        keystoreJSON = "";
        accountAddress = "";

        if (File.Exists(Application.persistentDataPath + "/keystore.ks"))
        {
            File.Delete(Application.persistentDataPath + "/keystore.ks");
        }

        //LobbyManager.instance.Canvas.SetActive(false);
        Canvas.SetActive(true);
        Canvas.transform.Find("LinkMethod").gameObject.SetActive(true);
        Canvas.transform.Find("MnemonicWords").gameObject.SetActive(false);
    }

    private string GetPrivateKeyFromKeystore(string pass)
    {
        if (keystoreJSON == null || keystoreJSON == "" || pass == null || pass == "")
        {
            return "";
        }

        byte[] b = m_keystoreService.DecryptKeyStoreFromJson(pass, keystoreJSON);
        EthECKey myKey = new EthECKey(b, true);

        if (myKey.GetPublicAddress() != accountAddress)
        {
            return "";
        }

        return myKey.GetPrivateKey();
    }

    private Sprite CreateBlocky(int resolution = 64)
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

        sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        sp.name = "Blocky";

        return sp;
    }

    #endregion


    #region Contract example

    public void StartCoroutine()
    {
        StartCoroutine(exampleContractCALL());
    }

    private IEnumerator exampleContractCALL()
    {
        var requestUrl = new EthCallUnityRequest(_url);
        var callInput = m_nameContract.Create_Call_Input();

        yield return requestUrl.SendRequest(callInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (requestUrl.Exception == null)
        {
            int result = m_nameContract.Get_Result(requestUrl.Result);

            Debug.Log(result);
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new InvalidOperationException("Get contract test request failed");
        }
    }
    #endregion

    #region event 
    public IEnumerator getEventRequest()
    {
        var getRequest = new EthGetLogsUnityRequest(_url);
        var getInput = m_nameContract.Create_Input_Event();
        Debug.Log("Checking event...");
        yield return getRequest.SendRequest(getInput);
        if (getRequest.Exception == null)
        {
            Debug.Log("Event: " + getRequest.Result);
        }
        else
        {
            Debug.Log("Error getting event: " + getRequest.Exception.Message);
        }
    }

    #endregion

}
