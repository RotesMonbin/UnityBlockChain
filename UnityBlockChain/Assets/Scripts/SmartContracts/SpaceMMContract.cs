using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.Encoders;
using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using UnityEngine;


public class SpaceMMContract
{
    #region ABI
    // We define the ABI of the contract we are going to use.
    public static string ABI = @"[]";
    #endregion

    // We define the contract address here
    // (Remember this contract is deployed on the ropsten network)
    private static string contractAddress = "0x0x0x0x0x0x0x0x0x0x0x0x0x";

    // We define a new contract (Netherum.Contracts)
    private Contract contract;

    public SpaceMMContract()
    {
        this.contract = new Contract(null, ABI, contractAddress);
    }

    #region P1 Create duel request
    public Function Get_requestDuel_Function()
    {
        return contract.GetFunction("requestDuel");
    }
    public TransactionInput Create_requestDuel_TransactionInput(
    string addressFrom,
    string privateKey,
    HexBigInteger gas = null,
    HexBigInteger gasPrice = null,
    HexBigInteger valueAmount = null)
    {
        var function = Get_requestDuel_Function();
        return function.CreateTransactionInput(addressFrom, gas, gasPrice, valueAmount);
    }
    #endregion

    #region P2 Answer duel request
    public Function Get_answerDuel_Function()
    {
        return contract.GetFunction("answerDuel");
    }
    public TransactionInput Create_answerDuel_TransactionInput(
    string addressFrom,
    string privateKey,
    BigInteger duelID,
    string teamHashed,
    HexBigInteger gas = null,
    HexBigInteger gasPrice = null,
    HexBigInteger valueAmount = null)
    {
        var function = Get_requestDuel_Function();
        return function.CreateTransactionInput(addressFrom, gas, gasPrice, valueAmount, duelID, teamHashed);
    }
    #endregion

    #region P1 Set Team One request
    public Function Get_setTeamOne_Function()
    {
        return contract.GetFunction("setTeamOne");
    }
    public TransactionInput Create_setTeamOne_TransactionInput(
    string addressFrom,
    string privateKey,
    BigInteger duelID,
    BigInteger[] shipsID,
    HexBigInteger gas = null,
    HexBigInteger gasPrice = null,
    HexBigInteger valueAmount = null)
    {
        var function = Get_setTeamOne_Function();
        return function.CreateTransactionInput(addressFrom, gas, gasPrice, valueAmount, duelID, shipsID);
    }
    #endregion

    #region P2 Validate duel request
    public Function Get_validateDuel_Function()
    {
        return contract.GetFunction("validateDuel");
    }
    public TransactionInput Create_validateDuel_TransactionInput(
    string addressFrom,
    string privateKey,
    BigInteger duelID,
    BigInteger[] shipsID,
    string salt,
    HexBigInteger gas = null,
    HexBigInteger gasPrice = null,
    HexBigInteger valueAmount = null)
    {
        var function = Get_validateDuel_Function();
        return function.CreateTransactionInput(addressFrom, gas, gasPrice, valueAmount, duelID, shipsID, salt);
    }
    #endregion
    
    #region Liste des duels disponibles
    public Function Get_availableDuels_Function()
    {
        return contract.GetFunction("availableDuels");
    }
    public CallInput Create_availableDuels_Input()
    {
        var function = Get_availableDuels_Function();
        return function.CreateCallInput();
    }
    public BigInteger[] Decode_shipsList(string result)
    {
        var function = Get_availableDuels_Function();
        return function.DecodeSimpleTypeOutput<BigInteger[]>(result);
    }
    #endregion

    #region Cancellation
    public Function Get_cancellation_Function()
    {
        return contract.GetFunction("cancellation");
    }
    public TransactionInput Create_cancellation_TransactionInput(
    string addressFrom,
    string privateKey,
    BigInteger duelID,
    HexBigInteger gas = null,
    HexBigInteger gasPrice = null,
    HexBigInteger valueAmount = null)
    {
        var function = Get_cancellation_Function();
        return function.CreateTransactionInput(addressFrom, gas, gasPrice, valueAmount, duelID);
    }
    #endregion

}