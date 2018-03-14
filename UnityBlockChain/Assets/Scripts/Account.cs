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
    private BoostContract m_boostContract = new BoostContract();
    private GeneshipsContract m_geneshipsContract = new GeneshipsContract();
    private SpaceMMContract m_spaceMMContract = new SpaceMMContract();
    private PingTokenContractService pingTokenContractService = new PingTokenContractService();

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
                // Debug.Log(accountAddress);
                GetBoost();
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

    #region Boost
    public void GetBoost()
    {
        // StartCoroutine(BoostGetRetrieveCost());
        // StartCoroutine(BoostGetNumberOfBoost());
        // StartCoroutine(BoostChangeModify());
        //StartCoroutine(getTransferEventRequest());

        //checkMyBalance();
         checkPings();
         //ping();
        // transferBalance();
        //checkPongEvent();
    }

    IEnumerator BoostGetRetrieveCost()
    {
        var BoostRequest = new EthCallUnityRequest(_url);
        var boostContractFunction = m_boostContract.Create_Call_Cost("1");

        yield return BoostRequest.SendRequest(boostContractFunction, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (BoostRequest.Exception == null)
        {
            int owner = m_boostContract.Get_Cost(BoostRequest.Result);

            Debug.Log(owner);
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new InvalidOperationException("Get boost request failed");
        }
    }

    IEnumerator BoostGetNumberOfBoost()
    {
        var BoostRequest = new EthCallUnityRequest(_url);
        
        var boostContractFunction = m_boostContract.Create_Call_PlayerNumberOfBoost(accountAddress, 1);

        yield return BoostRequest.SendRequest(boostContractFunction, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
        
        // Now we check if the request has an exception
        if (BoostRequest.Exception == null)
        {
            var numberOfBoost = m_boostContract.Get_PlayerOfBoost(BoostRequest.Result);
            Debug.Log(numberOfBoost);
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new InvalidOperationException("Get player Number of boost request failed");
        }

    }

    IEnumerator BoostChangeModify()
    {
        string accountPrivateKey = GetPrivateKeyFromKeystore(Password.text);

        if (accountPrivateKey == "")
        {
            yield break;
        }

        int idBoost = 1;
        int costBoost = 666;

        var transactionInput = m_boostContract.Create_BoostOrModify(
            idBoost,
            costBoost,
            accountAddress,
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


    #region event nexium 
    public IEnumerator getTransferEventRequest()
    {
        var getTransferRequest = new EthGetLogsUnityRequest(_url);
        var getTransferInput = m_nexiumContract.CreateTransferInput();
        Debug.Log("Checking transfer event...");
        yield return getTransferRequest.SendRequest(getTransferInput);
        if (getTransferRequest.Exception == null)
        {
            Debug.Log("transfert Event: " + getTransferRequest.Result);
        }
        else
        {
            Debug.Log("Error getting transfer event: " + getTransferRequest.Exception.Message);
        }
    }

    #endregion


    public void checkMyBalance()
    {
        // we create a new balance request, sending the address we want to check
        // in this case we use our public address
        StartCoroutine(getBalanceOfRequest(accountAddress));
    }
    public void checkPings()
    {
        StartCoroutine(getPingsRequest());
    }
    public void checkPongEvent()
    {
        StartCoroutine(getPongEventRequest());
    }
    public void ping()
    {
        StartCoroutine(pingRequest());
    }
    public void transferBalance()
    {
        // we create a coroutine to start a transfer request, and we send
        // the receiver address and the quantity of ping tokens we want to transfer
        // converted to wei units, in this case we send 2010 tokens
        StartCoroutine(transferToRequest(accountAddress, Nethereum.Util.UnitConversion.Convert.ToWei(2010)));
    }

    // we are going to use this function, to call the CheckTransactionReceiptIsMined() function
    // we created in the pingTokenService class, this will trigger every 10 seconds until
    // the transaction hash we sent is mined, throws an error or tries 999 times (just for testing)
    public void checkTx(string txHash, Action<bool> callback)
    {
        StartCoroutine(pingTokenContractService.CheckTransactionReceiptIsMined(
            _url,
            txHash,
            (cb) => {
                Debug.Log("The transaction has been mined succesfully");
                // we send a callback to the function, here you can add some more logic if you wish
                callback(true);
            }
        ));
    }

    // here we define all the IEnumerator functions for the requests
    // Basically, the requests needs two things
    // First, a request, EthCallUnityRequest for calls,
    // EthGetLogsUnityRequest for events, and TransactionSignedUnityRequest for transactions), 
    // Second, an input, we created it in the pingTokenService for each case.
    // After that, we just yield return the request, sending the input and
    // the block you want to check (depending on what we are doing)

    public IEnumerator getBalanceOfRequest(string address)
    {
        var getBalanceRequest = new EthCallUnityRequest(_url);
        var getBalanceInput = pingTokenContractService.CreateBalanceOfInput(address);
        Debug.Log("Getting balance of: " + address);
        yield return getBalanceRequest.SendRequest(getBalanceInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
        if (getBalanceRequest.Exception == null)
        {
            // So, basically this is the same for all the requests, if we have no exception
            // we decode (if needed) the result, and if we have an exception we show 
            // the exception message
            Debug.Log("Balance: " + pingTokenContractService.DecodeBalance(getBalanceRequest.Result));
        }
        else
        {
            Debug.Log("Error getting balance: " + getBalanceRequest.Exception.Message);
        }
    }

    public IEnumerator getPingsRequest()
    {
        var getPingsRequest = new EthCallUnityRequest(_url);
        var getPingsInput = pingTokenContractService.CreatePingsInput();
        Debug.Log("Getting pings...");
        yield return getPingsRequest.SendRequest(getPingsInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
        if (getPingsRequest.Exception == null)
        {
            Debug.Log("Pings: " + pingTokenContractService.DecodePings(getPingsRequest.Result));
        }
        else
        {
            Debug.Log("Error getting pings: " + getPingsRequest.Exception.Message);
        }
    }

    public IEnumerator getPongEventRequest()
    {
        var getPongRequest = new EthGetLogsUnityRequest(_url);
        var getPongInput = pingTokenContractService.CreatePongInput();
        Debug.Log("Checking pong event...");
        yield return getPongRequest.SendRequest(getPongInput);
        if (getPongRequest.Exception == null)
        {
            Debug.Log("Pong Event: " + getPongRequest.Result);
        }
        else
        {
            Debug.Log("Error getting pong event: " + getPongRequest.Exception.Message);
        }
    }

    public IEnumerator pingRequest()
    {
        // since the ping request is a transaction we need to set gas (80000)
        // gas price (79) and the eth we are going to send (0)
        // (these are just testing values)
        var transactionInput = pingTokenContractService.CreatePingInput(
            accountAddress,
            new HexBigInteger(80000),
            new HexBigInteger(79),
            new HexBigInteger(0)
        );
        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, accountAddress, accountAddress);
        Debug.Log("Ping transaction being submitted..");
        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);
        if (transactionSignedRequest.Exception == null)
        {
            Debug.Log("Ping tx created: " + transactionSignedRequest.Result);
            // Here, after the ping request succeeds, we call checkTx(), and
            // we send the transaction Hash (the result of the ping request),
            // and a callback, this will trigger when the tx type == 'mined'
            checkTx(transactionSignedRequest.Result, (cb) => {
                // here you can add some more logic to trigger after the tx is mined in the blockchain
            });
        }
        else
        {
            Debug.Log("Error submitting Ping: " + transactionSignedRequest.Exception.Message);
        }
    }

    public IEnumerator transferToRequest(string address, BigInteger value)
    {
        // Here we create a transferInput, we send our address, the address
        // we are going to send tokens to, how many tokens we are going to send,
        // the gas amount, the gas price, and the amount of eth to send.
        // In this case, we dont send eth, and we set a high gas price and gas amount
        // just for testing purposes in the testnet
        var transactionInput = pingTokenContractService.CreateTransferInput(
            accountAddress,
            address,
            value,
            new HexBigInteger(200000),
            new HexBigInteger(190),
            new HexBigInteger(0)
        );
        Debug.Log("Transfering tokens to: " + address);
        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, accountAddress, accountAddress);
        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);
        if (transactionSignedRequest.Exception == null)
        {
            Debug.Log("Transfered tx created: " + transactionSignedRequest.Result);
            // Now we check the transaction until it's mined in the blockchain as we did in
            // the pingRequest, when the callback is triggered (transaction mined),
            // we execute the getBalanceRequest for the address we send tokens to
            checkTx(transactionSignedRequest.Result, (cb) => {
                StartCoroutine(getBalanceOfRequest(address));
            });
        }
        else
        {
            Debug.Log("Error transfering tokens: " + transactionSignedRequest.Exception.Message);
        }
    }


    public void deployEthereumContract()
    {
        print("Deploying contract...");

        // Here we have our ABI & bytecode required for both creating and accessing our contract.
        var abi = @"[{""constant"":true,""inputs"":[],""name"":""name"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""pings"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""INITIAL_SUPPLY"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""decimals"",""outputs"":[{""name"":"""",""type"":""uint8""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""ping"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_owner"",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""name"":""balance"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""symbol"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_to"",""type"":""address""},{""name"":""_value"",""type"":""uint256""}],""name"":""transfer"",""outputs"":[{""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""pong"",""type"":""uint256""}],""name"":""Pong"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""name"":""from"",""type"":""address""},{""indexed"":true,""name"":""to"",""type"":""address""},{""indexed"":false,""name"":""value"",""type"":""uint256""}],""name"":""Transfer"",""type"":""event""}]";
        var byteCode = @"6060604052341561000f57600080fd5b601260ff16600a0a6305f5e10002600181905550601260ff16600a0a6305f5e10002600260003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000208190555061074f806100836000396000f300606060405260043610610099576000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff16806306fdde031461009e57806318160ddd1461012c5780631e81ccb2146101555780632ff2e9dc1461017e578063313ce567146101a75780635c36b186146101d657806370a08231146101ff57806395d89b411461024c578063a9059cbb146102da575b600080fd5b34156100a957600080fd5b6100b1610334565b6040518080602001828103825283818151815260200191508051906020019080838360005b838110156100f15780820151818401526020810190506100d6565b50505050905090810190601f16801561011e5780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b341561013757600080fd5b61013f61036d565b6040518082815260200191505060405180910390f35b341561016057600080fd5b610168610373565b6040518082815260200191505060405180910390f35b341561018957600080fd5b610191610379565b6040518082815260200191505060405180910390f35b34156101b257600080fd5b6101ba61038a565b604051808260ff1660ff16815260200191505060405180910390f35b34156101e157600080fd5b6101e961038f565b6040518082815260200191505060405180910390f35b341561020a57600080fd5b610236600480803573ffffffffffffffffffffffffffffffffffffffff1690602001909190505061049d565b6040518082815260200191505060405180910390f35b341561025757600080fd5b61025f6104e6565b6040518080602001828103825283818151815260200191508051906020019080838360005b8381101561029f578082015181840152602081019050610284565b50505050905090810190601f1680156102cc5780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b34156102e557600080fd5b61031a600480803573ffffffffffffffffffffffffffffffffffffffff1690602001909190803590602001909190505061051f565b604051808215151515815260200191505060405180910390f35b6040805190810160405280600981526020017f50696e67546f6b656e000000000000000000000000000000000000000000000081525081565b60015481565b60005481565b601260ff16600a0a6305f5e1000281565b601281565b600080601260ff16600a0a6001029050600260003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205481111515156103ed57600080fd5b8060016000828254039250508190555080600260003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828254039250508190555060008081548092919060010191905055507f58b69f57828e6962d216502094c54f6562f3bf082ba758966c3454f9e37b15256000546040518082815260200191505060405180910390a160005491505090565b6000600260008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020549050919050565b6040805190810160405280600481526020017f50494e470000000000000000000000000000000000000000000000000000000081525081565b60008073ffffffffffffffffffffffffffffffffffffffff168373ffffffffffffffffffffffffffffffffffffffff161415151561055c57600080fd5b600260003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205482111515156105aa57600080fd5b81600260003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205403600260003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000208190555081600260008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205401600260008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020819055508273ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef846040518082815260200191505060405180910390a360019050929150505600a165627a7a72305820de8fe20456e277e097051eb40a1239d02fd93b0f5e435de8208865c1a7ebaf890029";

        StartCoroutine(deployContract(abi, byteCode, accountAddress, (result) => {
            print("Result " + result);
        }));

    }


    public IEnumerator deployContract(string abi, string byteCode, string senderAddress, System.Action<string> callback)
    {
        // Ammount of gas required to create the contract
        var gas = new HexBigInteger(900000);
        throw new System.InvalidOperationException("Deploy contract tx failed:");
        // First we build the transaction
        /*var transactionInput = contractTransactionBuilder.BuildTransaction(abi, byteCode, senderAddress, gas, null);

        // Here we create a new signed transaction Unity Request with the url, the private and public key
        // (this will sign the transaction automatically)
        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, accountAddress, accountAddress);

        // Then we send the request and wait for the transaction hash
        Debug.Log("Sending Deploy contract transaction...");
        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);
        if (transactionSignedRequest.Exception == null)
        {
            // If we don't have exceptions we just return the result!
            callback(transactionSignedRequest.Result);
        }
        else
        {
            // if we had an error in the UnityRequest we just display the Exception error
            throw new System.InvalidOperationException("Deploy contract tx failed:" + transactionSignedRequest.Exception.Message);
        }*/
    }

}
