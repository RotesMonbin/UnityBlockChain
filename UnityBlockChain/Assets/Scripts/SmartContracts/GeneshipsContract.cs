using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.Encoders;
using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using UnityEngine;

public class GeneshipsContract
{
    #region ABI
    // We define the ABI of the contract we are going to use.
    public static string ABI = @"[]";
    #endregion

    public struct ShipStructure
    {

    }

    // We define the contract address here
    // (Remember this contract is deployed on the ropsten network)
    private static string contractAddress = "0x0x0x0x0x0x0x0x0x0x0x0x0x0x";

    // We define a new contract (Netherum.Contracts)
    private Contract contract;

    public GeneshipsContract()
    {
        this.contract = new Contract(null, ABI, contractAddress);
    }

    #region Liste des vaisseaux
    public Function Get_shipsList_Function()
    {
        return contract.GetFunction("shipsList");
    }
    public CallInput Create_shipsList_Input(string address)
    {
        var function = Get_shipsList_Function();
        return function.CreateCallInput(address);
    }
    public BigInteger[] Decode_shipsList(string result)
    {
        var function = Get_shipsList_Function();
        return function.DecodeSimpleTypeOutput<BigInteger[]>(result);
    }
    #endregion

    #region Propriétaire d'un vaisseau
    public Function Get_shipOwner_Function()
    {
        return contract.GetFunction("shipOwner");
    }
    public CallInput Create_shipOwner_Input(BigInteger shipID)
    {
        var function = Get_shipOwner_Function();
        return function.CreateCallInput(shipID);
    }
    public string Decode_shipOwner(string result)
    {
        var function = Get_shipOwner_Function();
        return function.DecodeSimpleTypeOutput<string>(result);
    }
    #endregion

    #region Gènes d'un vaisseau
    public Function Get_shipGenes_Function()
    {
        return contract.GetFunction("shipGenes");
    }
    public CallInput Create_shipGenes_Input(BigInteger shipID)
    {
        var function = Get_shipGenes_Function();
        return function.CreateCallInput(shipID);
    }
    public BigInteger Decode_shipGenes(string result)
    {
        var function = Get_shipGenes_Function();
        return function.DecodeSimpleTypeOutput<BigInteger>(result);
    }
    #endregion

    #region Gènes de plusieurs vaisseaux
    public Function Get_shipsGenes_Function()
    {
        return contract.GetFunction("shipsGenes");
    }
    public CallInput Create_shipsGenes_Input(BigInteger[] shipsID)
    {
        var function = Get_shipsGenes_Function();
        return function.CreateCallInput(shipsID);
    }
    public BigInteger[] Decode_shipsGenes(string result)
    {
        var function = Get_shipsGenes_Function();
        return function.DecodeSimpleTypeOutput<BigInteger[]>(result);
    }
    #endregion      
    
    #region Structure de plusieurs vaisseaux
    public Function Get_shipsStructure_Function()
    {
        return contract.GetFunction("shipsStructure");
    }
    public CallInput Create_shipsStructure_Input(BigInteger[] shipsID)
    {
        var function = Get_shipsStructure_Function();
        return function.CreateCallInput(shipsID);
    }
    public ShipStructure[] Decode_shipsStructure(string result)
    {
        var function = Get_shipsStructure_Function();
        return function.DecodeSimpleTypeOutput<ShipStructure[]>(result);
    }
    #endregion
}
