using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.Encoders;
using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.ABI.FunctionEncoding.Attributes;

public class BoostContract {

    #region ABI
    // We define the ABI of the contract we are going to use.
    public static string ABI = @"[
	{
		""constant"": true,
		""inputs"": [
			{
				""name"": """",
				""type"": ""uint256""

            }
		],
		""name"": ""retrieveCost"",
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
		""constant"": true,
		""inputs"": [
			{
				""name"": """",
				""type"": ""address""
			},
			{
				""name"": """",
				""type"": ""uint256""
			}
		],
		""name"": ""playerNumberOfBoost"",
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
		""constant"": true,
		""inputs"": [],
		""name"": ""owner"",
		""outputs"": [
			{
				""name"": """",
				""type"": ""address""
			}
		],
		""payable"": false,
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""anonymous"": false,
		""inputs"": [
			{
				""indexed"": true,
				""name"": """",
				""type"": ""address""
			},
			{
				""indexed"": false,
				""name"": ""idBoost"",
				""type"": ""uint256""
			},
			{
				""indexed"": false,
				""name"": ""numberOfBoost"",
				""type"": ""uint256""
			}
		],
		""name"": ""Purchase"",
		""type"": ""event""
	},
	{
		""constant"": false,
		""inputs"": [
			{
				""name"": ""_idBoost"",
				""type"": ""uint256""
			},
			{
				""name"": ""_cost"",
				""type"": ""uint256""
			}
		],
		""name"": ""createOrModifBoost"",
		""outputs"": [],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	},
	{
		""constant"": false,
		""inputs"": [
			{
				""name"": ""_address"",
				""type"": ""address""
			},
			{
				""name"": ""_value"",
				""type"": ""uint256""
			},
			{
				""name"": ""_token"",
				""type"": ""address""
			},
			{
				""name"": ""_extraData"",
				""type"": ""bytes""
			}
		],
		""name"": ""receiveApproval"",
		""outputs"": [],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	},
	{
		""inputs"": [
			{
				""name"": ""_nexiumAdress"",
				""type"": ""address""
			}
		],
		""payable"": false,
		""stateMutability"": ""nonpayable"",
		""type"": ""constructor""
	}
]";
    #endregion

    // We define the contract address here
    // (Remember this contract is deployed on the ropsten network)
    private static string contractAddress = "0x1129c0721a4200b3d0839e2a6079992e0b685959";

    // We define a new contract (Netherum.Contracts)
    private Contract contract;

    public BoostContract()
    {
        this.contract = new Contract(null, ABI, contractAddress);
    }

    #region GET Retrieve Cost
    public Function Get_Cost_Function()
    {
        return contract.GetFunction("retrieveCost");
    }

    public CallInput Create_Call_Cost(string id)
    {
        // For this transaction to the contract we dont need inputs,
        // its only to retreive the quantity of Nexium on the address
        var function = Get_Cost_Function();
        return function.CreateCallInput(id);
    }

    public int Get_Cost(string cost)
    {
        var function = Get_Cost_Function();
        return function.DecodeSimpleTypeOutput<int>(cost);
    }
    #endregion


    #region GET Player Number boost
    public Function Get_PlayerNumberBoost_Function()
    {
        return contract.GetFunction("playerNumberOfBoost");
    }

    public CallInput Create_Call_PlayerNumberOfBoost(string adresse)
    {
        var function = Get_PlayerNumberBoost_Function();

        return function.CreateCallInput(adresse);
    }

    public int Get_PlayerOfBoost(string numberOfBoost)
    {
        var function = Get_PlayerNumberBoost_Function();
        return function.DecodeSimpleTypeOutput<int>(numberOfBoost);
    }
    #endregion


    #region Create Modify Boost
    public Function Get_ModifyBoost_Function()
    {
        return contract.GetFunction("createOrModifBoost");
    }

    public TransactionInput Create_BoostOrModify(
    int idBoost,
    int costBoost,
    string addressFrom,
    HexBigInteger gas = null,
    HexBigInteger gasPrice = null,
    HexBigInteger valueAmount = null)
    {
        var function = Get_ModifyBoost_Function();
        return function.CreateTransactionInput(addressFrom, gas, gasPrice, valueAmount, idBoost, costBoost);
    }
    #endregion
}
