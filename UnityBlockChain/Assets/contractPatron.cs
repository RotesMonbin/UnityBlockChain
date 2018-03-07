using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

public class NameContract
{

    #region ABI
    // We define the ABI of the contract we are going to use.
    public static string ABI = @"";
    #endregion

    // We define the contract address here
    // (Remember this contract is deployed on the ropsten network)
    private static string contractAddress = "";

    // We define a new contract
    private Contract contract;

    public NameContract()
    {
        this.contract = new Contract(null, ABI, contractAddress);
    }

    #region CALL 
    public Function Get_Function()
    {
        // Name of your contract =>  v
        return contract.GetFunction(" ");
    }

    // You can put a parameter, if you have one in your contract
    public CallInput Create_Call_Input()
    {
        // if you want more than 2 parameter => object[] array = new[] { first, seconde };
        return Get_Function().CreateCallInput();
    }

    // Recover the result in the string output
    public int Get_Result(string output)
    {
        return Get_Function().DecodeSimpleTypeOutput<int>(output);
    }
    #endregion


    #region event
    public Nethereum.Contracts.Event Get_Event()
    {
        // Name of your event =>  v
        return contract.GetEvent(" ");
    }


    public NewFilterInput Create_Input_Event()
    {
        var evt = Get_Event();
        // for this event, when creating the input, we need to define the blockFrom and the blockTo parameters.
        // we send the first block and the last block in this case, so we retrieve the transfert event
        // transactions in all the blocks
        return evt.CreateFilterInput(Nethereum.RPC.Eth.DTOs.BlockParameter.CreateEarliest(), Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
    }
    #endregion
}


