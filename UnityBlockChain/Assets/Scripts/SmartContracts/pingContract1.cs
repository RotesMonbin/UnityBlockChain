using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Collections;
using Nethereum.JsonRpc.UnityClient;
using UnityEngine;

public class PingTokenContractService
{
    // Here we set the ABI for our contract
    public static string ABI = @"[{""constant"":true,""inputs"":[],""name"":""name"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""pings"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""INITIAL_SUPPLY"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""decimals"",""outputs"":[{""name"":"""",""type"":""uint8""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""ping"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_owner"",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""name"":""balance"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""symbol"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_to"",""type"":""address""},{""name"":""_value"",""type"":""uint256""}],""name"":""transfer"",""outputs"":[{""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""pong"",""type"":""uint256""}],""name"":""Pong"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""name"":""from"",""type"":""address""},{""indexed"":true,""name"":""to"",""type"":""address""},{""indexed"":false,""name"":""value"",""type"":""uint256""}],""name"":""Transfer"",""type"":""event""}]";
    // This is the address of the contract we just created.
    private static string contractAddress = "0x582f185a1e2bd6553723dc923210af60274fa4de";

    private Contract contract;

    public PingTokenContractService()
    {
        // Initialize the contract with the ABI and the address
        this.contract = new Contract(null, ABI, contractAddress);
    }

    // -------------------------------------------------------------------------
    // Here we define all the contract's functions and events

    // There is a duplicated Namespace error with the Event class (it's defined in both Nethereum.Contracts and UnityEngine), 
    // that's why we declared it like "Nethereum.Contracts" and not only "Event"
    public Nethereum.Contracts.Event GetPongEvent()
    {
        return contract.GetEvent("Pong");
    }

    public Function GetBalanceOfFunction()
    {
        return contract.GetFunction("balanceOf");
    }

    public Function GetPingsFunction()
    {
        return contract.GetFunction("pings");
    }

    public Function PingFunction()
    {
        return contract.GetFunction("ping");
    }

    public Function TransferFunction()
    {
        return contract.GetFunction("transfer");
    }

    // -------------------------------------------------------------------------

    public CallInput CreateBalanceOfInput(string addressFrom)
    {
        var function = GetBalanceOfFunction();
        return function.CreateCallInput(addressFrom.RemoveHexPrefix());
    }

    public TransactionInput CreatePingInput(
        string addressFrom,
        HexBigInteger gas = null,
        HexBigInteger gasPrice = null,
        HexBigInteger valueAmount = null)
    {
        var function = PingFunction();
        return function.CreateTransactionInput(addressFrom, gas, gasPrice, valueAmount);
    }

    public CallInput CreatePingsInput()
    {
        var function = GetPingsFunction();
        return function.CreateCallInput();
    }

    public NewFilterInput CreatePongInput()
    {
        var evt = GetPongEvent();
        // for this event, when creating the input, we need to define the blockFrom and the blockTo parameters.
        // we send the first block and the last block in this case, so we retrieve the pong event
        // transactions in all the blocks
        return evt.CreateFilterInput(Nethereum.RPC.Eth.DTOs.BlockParameter.CreateEarliest(), Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
    }

    public TransactionInput CreateTransferInput(
        string addressFrom,
        // the addressTo is the receiver address of the tokens
        string addressTo,
        // the value is what we are going to transfer
        BigInteger value,
        HexBigInteger gas = null,
        HexBigInteger gasPrice = null,
        HexBigInteger valueAmount = null
    )
    {
        var function = TransferFunction();
        return function.CreateTransactionInput(
            // we send the addressTo with RemoveHexPrefix() to remove the "0x"
            addressFrom, gas, gasPrice, valueAmount, addressTo.RemoveHexPrefix(), value
        );
    }

    // -------------------------------------------------------------------------

    public int DecodeBalance(string balance)
    {
        // We will use this function later to parse the result of encoded balance (Hexadecimal 0x0f)
        // into a decoded output (Integer 15)
        var function = GetBalanceOfFunction();
        var decodedBalance = function.DecodeSimpleTypeOutput<BigInteger>(balance);
        return (int)Nethereum.Util.UnitConversion.Convert.FromWei(decodedBalance, 18);
    }

    public BigInteger DecodePings(string pings)
    {
        // We will use this function later to parse the result of encoded pings (Hexadecimal 0x0f)
        // into a decoded output (Integer 15)
        var function = GetPingsFunction();
        return function.DecodeSimpleTypeOutput<BigInteger>(pings);
    }

    // We are going to use this IEnumerator to loop every 10 seconds and check the 
    // transaction hash, to see if its mined in the blockchain
    public IEnumerator CheckTransactionReceiptIsMined(
        string url, string txHash, System.Action<bool> callback
    )
    {
        var mined = false;
        // we are going to set the tries to 999 for testing purposes
        var tries = 999;
        while (!mined)
        {
            if (tries > 0)
            {
                tries = tries - 1;
            }
            else
            {
                mined = true;
                Debug.Log("Performing last try..");
            }
            Debug.Log("Checking receipt for: " + txHash);
            var receiptRequest = new EthGetTransactionReceiptUnityRequest(url);
            yield return receiptRequest.SendRequest(txHash);
            if (receiptRequest.Exception == null)
            {
                if (receiptRequest.Result != null && receiptRequest.Result.Logs.HasValues)
                {
                    var txType = receiptRequest.Result.Logs[0]["type"].ToString();
                    if (txType == "mined")
                    {
                        // if we have a transaction type == mined we return the callback
                        // and exit the loop
                        mined = true;
                        callback(mined);
                    }
                }
            }
            else
            {
                // If we had an error doing the request
                Debug.Log("Error checking receipt: " + receiptRequest.Exception.Message);
            }
            yield return new WaitForSeconds(10);
        }
    }
}