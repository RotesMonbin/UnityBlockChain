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
using System.Runtime.InteropServices;
using ETH_Identicons;

public class Account : MonoBehaviour
{
    public static Account instance { get; private set; }

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

    private NexiumContract m_nexiumContract = new NexiumContract();
    private GeneshipsContract m_geneshipsContract = new GeneshipsContract();
    private SpaceMMContract m_spaceMMContract = new SpaceMMContract();

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

        Words[0].text = "dizzy";
        Words[1].text = "grow";
        Words[2].text = "empower";
        Words[3].text = "decrease";
        Words[4].text = "divert";
        Words[5].text = "gossip";
        Words[6].text = "name";
        Words[7].text = "claw";
        Words[8].text = "cycle";
        Words[9].text = "fruit";
        Words[10].text = "comfort";
        Words[11].text = "gospel";

        if (LoadJSONFromDisk())
        {
            if (RetrieveAddressFromKeystore())
            {
                // OK
                GetNexiumBalance();
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

    private IEnumerator getAccountBalance()
    {
        // Now we define a new EthGetBalanceUnityRequest and send it the testnet url where we are going to
        // (we get EthGetBalanceUnityRequest from the Netherum lib imported at the start)
        var getBalanceRequest = new EthGetBalanceUnityRequest("https://ropsten.infura.io");
        // Then we call the method SendRequest() from the getBalanceRequest we created
        // with the address and the newest created block.
        yield return getBalanceRequest.SendRequest(accountAddress, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (getBalanceRequest.Exception == null)
        {
            // We define balance and assign the value that the getBalanceRequest gave us.
            var balance = getBalanceRequest.Result.Value;
            // Finally we execute the callback and we use the Netherum.Util.UnitConversion
            // to convert the balance from WEI to ETHER (that has 18 decimal places)
            string etherBalance = "ETHER : " + Nethereum.Util.UnitConversion.Convert.FromWei(balance, 18);
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new InvalidOperationException("Get balance request failed");
        }
    }

    #region Nexium Contract Calls
    public void GetNexiumBalance()
    {
        StartCoroutine(NexiumBalance());
    }
    IEnumerator NexiumBalance()
    {
        var nexiumBalanceRequest = new EthCallUnityRequest(_url);
        var nexiumBalanceCallInput = m_nexiumContract.Create_balanceOf_Input(accountAddress);

        yield return nexiumBalanceRequest.SendRequest(nexiumBalanceCallInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (nexiumBalanceRequest.Exception == null)
        {
            string nexium = m_nexiumContract.Decode_balanceOf(nexiumBalanceRequest.Result).ToString();
            string prettyNexium = "NEXIUM : " + nexium.Substring(0, nexium.Length - 3) + "," + nexium.Substring(nexium.Length - 3);

            Debug.Log(prettyNexium);
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new InvalidOperationException("Get Nexium balance request failed");
        }
    }
    public void NexiumApproveTest()
    {
        // Address is Metamask faucet
        StartCoroutine(NexiumApprove("0x81b7e08f65bdf5648606c89998a9cc8164397647", 10));
    }
    IEnumerator NexiumApprove(string spender, BigInteger value)
    {
        string accountPrivateKey = GetPrivateKeyFromKeystore(Password.text);

        if (accountPrivateKey == "")
        {
            yield break;
        }

        var transactionInput = m_nexiumContract.Create_approve_TransactionInput(
            accountAddress,
            accountPrivateKey,
            spender,
            value,
            new HexBigInteger(20000),
            new HexBigInteger(10),
            new HexBigInteger(0)
        );

        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, accountPrivateKey, accountAddress);

        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);

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
    #endregion

    #region Geneships Contract Calls
    public void GetShipsList(Action<BigInteger[]> act)
    {
        StartCoroutine(ShipsList(act));
    }
    IEnumerator ShipsList(Action<BigInteger[]> act)
    {
        var shipsListRequest = new EthCallUnityRequest(_url);
        var shipsListCallInput = m_geneshipsContract.Create_shipsList_Input(accountAddress);

        yield return shipsListRequest.SendRequest(shipsListCallInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (shipsListRequest.Exception == null)
        {
            if (act != null)
                act(m_geneshipsContract.Decode_shipsList(shipsListRequest.Result));
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new InvalidOperationException("Get Ships List request failed");
        }
    }

    public void GetShipOwner(BigInteger shipID, Action<string> act)
    {
        StartCoroutine(ShipOwner(shipID, act));
    }
    IEnumerator ShipOwner(BigInteger shipID, Action<string> act)
    {
        var shipOwnerRequest = new EthCallUnityRequest(_url);
        var shipOwnerCallInput = m_geneshipsContract.Create_shipOwner_Input(shipID);

        yield return shipOwnerRequest.SendRequest(shipOwnerCallInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (shipOwnerRequest.Exception == null)
        {
            if (act != null)
                act(m_geneshipsContract.Decode_shipOwner(shipOwnerRequest.Result));
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new InvalidOperationException("Get Ship Owner request failed");
        }
    }

    public void GetShipGenes(BigInteger shipID, Action<BigInteger> act)
    {
        StartCoroutine(ShipGenes(shipID, act));
    }
    IEnumerator ShipGenes(BigInteger shipID, Action<BigInteger> act)
    {
        var shipGenesRequest = new EthCallUnityRequest(_url);
        var shipGenesCallInput = m_geneshipsContract.Create_shipGenes_Input(shipID);

        yield return shipGenesRequest.SendRequest(shipGenesCallInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (shipGenesRequest.Exception == null)
        {
            if (act != null)
                act(m_geneshipsContract.Decode_shipGenes(shipGenesRequest.Result));
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new InvalidOperationException("Get Ship Genes request failed");
        }
    }

    public void GetShipsGenes(BigInteger[] shipsID, Action<BigInteger[]> act)
    {
        StartCoroutine(ShipsGenes(shipsID, act));
    }
    IEnumerator ShipsGenes(BigInteger[] shipsID, Action<BigInteger[]> act)
    {
        var shipsGenesRequest = new EthCallUnityRequest(_url);
        var shipsGenesCallInput = m_geneshipsContract.Create_shipsGenes_Input(shipsID);

        yield return shipsGenesRequest.SendRequest(shipsGenesCallInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (shipsGenesRequest.Exception == null)
        {
            if (act != null)
                act(m_geneshipsContract.Decode_shipsGenes(shipsGenesRequest.Result));
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new InvalidOperationException("Get Ships Genes request failed");
        }
    }

    public void GetShipsStructure(BigInteger[] shipsID, Action<GeneshipsContract.ShipStructure[]> act)
    {
        StartCoroutine(ShipsStructure(shipsID, act));
    }
    IEnumerator ShipsStructure(BigInteger[] shipsID, Action<GeneshipsContract.ShipStructure[]> act)
    {
        var shipsStructureRequest = new EthCallUnityRequest(_url);
        var shipsStructureCallInput = m_geneshipsContract.Create_shipsStructure_Input(shipsID);

        yield return shipsStructureRequest.SendRequest(shipsStructureCallInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (shipsStructureRequest.Exception == null)
        {
            if (act != null)
                act(m_geneshipsContract.Decode_shipsStructure(shipsStructureRequest.Result));
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new InvalidOperationException("Get Ships Structure request failed");
        }
    }
    #endregion

    #region Space Match Making Contract Calls
    public void SendRequestDuel(Action act)
    {
        StartCoroutine(RequestDuel(act));
    }
    IEnumerator RequestDuel(Action act)
    {
        string accountPrivateKey = GetPrivateKeyFromKeystore(Password.text);

        if (accountPrivateKey == "")
        {
            yield break;
        }

        var transactionInput = m_spaceMMContract.Create_requestDuel_TransactionInput(
            accountAddress,
            accountPrivateKey,
            new HexBigInteger(20000),
            new HexBigInteger(10),
            new HexBigInteger(0)
        );

        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, accountPrivateKey, accountAddress);

        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);

        if (transactionSignedRequest.Exception == null)
        {
            if (act != null)
                act();
        }
        else
        {
            Debug.Log("Error Request Duel: " + transactionSignedRequest.Exception.Message);
        }
    }

    public void SendAnswerDuel(BigInteger duelID, string teamHash, Action act)
    {
        StartCoroutine(AnswerDuel(duelID, teamHash, act));
    }
    IEnumerator AnswerDuel(BigInteger duelID, string teamHash, Action act)
    {
        string accountPrivateKey = GetPrivateKeyFromKeystore(Password.text);

        if (accountPrivateKey == "")
        {
            yield break;
        }

        var transactionInput = m_spaceMMContract.Create_answerDuel_TransactionInput(
            accountAddress,
            accountPrivateKey,
            duelID,
            teamHash,
            new HexBigInteger(20000),
            new HexBigInteger(10),
            new HexBigInteger(0)
        );

        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, accountPrivateKey, accountAddress);

        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);

        if (transactionSignedRequest.Exception == null)
        {
            if (act != null)
                act();
        }
        else
        {
            Debug.Log("Error Answer Duel: " + transactionSignedRequest.Exception.Message);
        }
    }

    public void SendSetTeamOne(BigInteger duelID, BigInteger[] shipsID, Action act)
    {
        StartCoroutine(SetTeamOne(duelID, shipsID, act));
    }
    IEnumerator SetTeamOne(BigInteger duelID, BigInteger[] shipsID, Action act)
    {
        string accountPrivateKey = GetPrivateKeyFromKeystore(Password.text);

        if (accountPrivateKey == "")
        {
            yield break;
        }

        var transactionInput = m_spaceMMContract.Create_setTeamOne_TransactionInput(
            accountAddress,
            accountPrivateKey,
            duelID,
            shipsID,
            new HexBigInteger(20000),
            new HexBigInteger(10),
            new HexBigInteger(0)
        );

        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, accountPrivateKey, accountAddress);

        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);

        if (transactionSignedRequest.Exception == null)
        {
            if (act != null)
                act();
        }
        else
        {
            Debug.Log("Error Set Team One: " + transactionSignedRequest.Exception.Message);
        }
    }

    public void SendValidateDuel(BigInteger duelID, BigInteger[] shipsID, string salt, Action act)
    {
        StartCoroutine(ValidateDuel(duelID, shipsID, salt, act));
    }
    IEnumerator ValidateDuel(BigInteger duelID, BigInteger[] shipsID, string salt, Action act)
    {
        string accountPrivateKey = GetPrivateKeyFromKeystore(Password.text);

        if (accountPrivateKey == "")
        {
            yield break;
        }

        var transactionInput = m_spaceMMContract.Create_validateDuel_TransactionInput(
            accountAddress,
            accountPrivateKey,
            duelID,
            shipsID,
            salt,
            new HexBigInteger(20000),
            new HexBigInteger(10),
            new HexBigInteger(0)
        );

        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, accountPrivateKey, accountAddress);

        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);

        if (transactionSignedRequest.Exception == null)
        {
            if (act != null)
                act();
        }
        else
        {
            Debug.Log("Error Validate Duel: " + transactionSignedRequest.Exception.Message);
        }
    }

    public void GetAvailableDuels(Action<BigInteger[]> act)
    {
        StartCoroutine(AvailableDuels(act));
    }
    IEnumerator AvailableDuels(Action<BigInteger[]> act)
    {
        var availableDuelsRequest = new EthCallUnityRequest(_url);
        var availableDuelsCallInput = m_spaceMMContract.Create_availableDuels_Input();

        yield return availableDuelsRequest.SendRequest(availableDuelsCallInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (availableDuelsRequest.Exception == null)
        {
            if (act != null)
                act(m_spaceMMContract.Decode_shipsList(availableDuelsRequest.Result));
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new InvalidOperationException("Get Duels List request failed");
        }
    }


    public void SendCancellation(BigInteger duelID, Action act)
    {
        StartCoroutine(Cancellation(duelID, act));
    }
    IEnumerator Cancellation(BigInteger duelID, Action act)
    {
        string accountPrivateKey = GetPrivateKeyFromKeystore(Password.text);

        if (accountPrivateKey == "")
        {
            yield break;
        }

        var transactionInput = m_spaceMMContract.Create_cancellation_TransactionInput(
            accountAddress,
            accountPrivateKey,
            duelID,
            new HexBigInteger(20000),
            new HexBigInteger(10),
            new HexBigInteger(0)
        );

        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, accountPrivateKey, accountAddress);

        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);

        if (transactionSignedRequest.Exception == null)
        {
            if (act != null)
                act();
        }
        else
        {
            Debug.Log("Error Validate Duel: " + transactionSignedRequest.Exception.Message);
        }
    }
    #endregion

}