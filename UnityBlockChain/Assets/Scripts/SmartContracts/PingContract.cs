using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.Encoders;
using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.ABI.FunctionEncoding.Attributes;

public class PingContract
{
    [FunctionOutput]
    public class GetDataDTO
    {
        [Parameter("uint8", "testA", 1)]
        public uint testA { get; set; }

        [Parameter("string", "text", 2)]
        public string text { get; set; }
    }

    string output;

    #region ABI
    // We define the ABI of the contract we are going to use.
    public static string ABI = @"[
	{
		""constant"": true,
		""inputs"": [],
		""name"": ""pings"",
		""outputs"": [
			{
				""name"": """",
				""type"": ""uint256""

            }
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""constant"": false,
		""inputs"": [
			{
				""name"": ""value"",
				""type"": ""uint256""
			}
		],
		""name"": ""ping"",
		""outputs"": [],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	},
	{
		""constant"": false,
		""inputs"": [],
		""name"": ""structReturn"",
		""outputs"": [
			{
				""components"": [
					{
						""name"": ""testA"",
						""type"": ""uint8""
					},
					{
						""name"": ""text"",
						""type"": ""string""
					},
					{
						""name"": ""ping"",
						""type"": ""uint256""
					}
				],
				""name"": ""a"",
				""type"": ""tuple""
			}
		],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	},
	{
		""anonymous"": false,
		""inputs"": [
			{
				""indexed"": false,
				""name"": ""pong"",
				""type"": ""uint256""
			}
		],
		""name"": ""Pong"",
		""type"": ""event""
	}
]";
    #endregion

    // We define the contract address here
    // (Remember this contract is deployed on the ropsten network)
    private static string contractAddress = "0xdcbffeb602452d2a222e9c94a9461846bf62e36a";

    // We define a new contract (Netherum.Contracts)
    private Contract contract;

    public PingContract()
    {
        this.contract = new Contract(null, ABI, contractAddress);
    }

    #region Balance Of
    public Function Get_balanceOf_Function()
    {
        return contract.GetFunction("structReturn");
    }
    public CallInput Create_balanceOf_Input(string address)
    {
        // For this transaction to the contract we dont need inputs,
        // its only to retreive the quantity of Nexium on the address
        var function = Get_balanceOf_Function();
        return function.CreateCallInput(address);
    }
    public void decode_Struct_ping()
    {
        Debug.Log("Decode");
        var function = Get_balanceOf_Function();

        var test = function.DecodeDTOTypeOutput<GetDataDTO>(output);

        Debug.Log(test.testA);
        Debug.Log(test.text);

    }
    #endregion

    #region Approve
    public Function Get_approve_Function()
    {
        return contract.GetFunction("approve");
    }
    public TransactionInput Create_approve_TransactionInput(
    // For this transaction to the contract we are going to use
    // the address which is excecuting the transaction (addressFrom), 
    // the private key of that address (privateKey),
    // the ping value we are going to send to this contract (pingValue),
    // the maximum amount of gas to consume,
    // the price you are willing to pay per each unit of gas consumed, (higher the price, faster the tx will be included)
    // and the valueAmount in ETH to send to this contract.
    // IMPORTANT: the PingContract doesn't accept eth transfers so this must be 0 or it will throw an error.
    string addressFrom,
    string privateKey,
    string spender,
    BigInteger value,
    HexBigInteger gas = null,
    HexBigInteger gasPrice = null,
    HexBigInteger valueAmount = null)
    {
        var function = Get_approve_Function();
        return function.CreateTransactionInput(addressFrom, gas, gasPrice, valueAmount, spender, value);
    }
    #endregion
}