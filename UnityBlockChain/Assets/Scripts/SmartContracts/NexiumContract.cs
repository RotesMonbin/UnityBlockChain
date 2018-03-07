﻿using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using Nethereum.Hex.HexTypes;


public class NexiumContract
{
    #region ABI
    // We define the ABI of the contract we are going to use.
    public static string ABI = @"[
    {
      ""constant"": true,
      ""inputs"": [],
      ""name"": ""name"",
      ""outputs"": [
        {
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""payable"": false,
      ""type"": ""function""
    },
    {
      ""constant"": false,
      ""inputs"": [
        {
          ""name"": ""_spender"",
          ""type"": ""address""
        },
        {
          ""name"": ""_value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""approve"",
      ""outputs"": [
        {
          ""name"": ""success"",
          ""type"": ""bool""
        }
      ],
      ""payable"": false,
      ""type"": ""function""
    },
    {
      ""constant"": false,
      ""inputs"": [],
      ""name"": ""totalSupply"",
      ""outputs"": [
        {
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""payable"": false,
      ""type"": ""function""
    },
    {
      ""constant"": false,
      ""inputs"": [
        {
          ""name"": ""_from"",
          ""type"": ""address""
        },
        {
          ""name"": ""_to"",
          ""type"": ""address""
        },
        {
          ""name"": ""_value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""transferFrom"",
      ""outputs"": [
        {
          ""name"": ""success"",
          ""type"": ""bool""
        }
      ],
      ""payable"": false,
      ""type"": ""function""
    },
    {
      ""constant"": true,
      ""inputs"": [],
      ""name"": ""decimals"",
      ""outputs"": [
        {
          ""name"": """",
          ""type"": ""uint8""
        }
      ],
      ""payable"": false,
      ""type"": ""function""
    },
    {
      ""constant"": true,
      ""inputs"": [],
      ""name"": ""initialSupply"",
      ""outputs"": [
        {
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""payable"": false,
      ""type"": ""function""
    },
    {
      ""constant"": true,
      ""inputs"": [
        {
          ""name"": """",
          ""type"": ""address""
        }
      ],
      ""name"": ""balanceOf"",
      ""outputs"": [
        {
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""payable"": false,
      ""type"": ""function""
    },
    {
      ""constant"": true,
      ""inputs"": [],
      ""name"": ""burnAddress"",
      ""outputs"": [
        {
          ""name"": """",
          ""type"": ""address""
        }
      ],
      ""payable"": false,
      ""type"": ""function""
    },
    {
      ""constant"": true,
      ""inputs"": [],
      ""name"": ""symbol"",
      ""outputs"": [
        {
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""payable"": false,
      ""type"": ""function""
    },
    {
      ""constant"": false,
      ""inputs"": [
        {
          ""name"": ""_to"",
          ""type"": ""address""
        },
        {
          ""name"": ""_value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""transfer"",
      ""outputs"": [
        {
          ""name"": ""success"",
          ""type"": ""bool""
        }
      ],
      ""payable"": false,
      ""type"": ""function""
    },
    {
      ""constant"": false,
      ""inputs"": [
        {
          ""name"": ""_spender"",
          ""type"": ""address""
        },
        {
          ""name"": ""_value"",
          ""type"": ""uint256""
        },
        {
          ""name"": ""_extraData"",
          ""type"": ""bytes""
        }
      ],
      ""name"": ""approveAndCall"",
      ""outputs"": [
        {
          ""name"": ""success"",
          ""type"": ""bool""
        }
      ],
      ""payable"": false,
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
          ""type"": ""address""
        }
      ],
      ""name"": ""allowance"",
      ""outputs"": [
        {
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""payable"": false,
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""payable"": false,
      ""type"": ""constructor""
    },
    {
      ""payable"": false,
      ""type"": ""fallback""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""name"": ""from"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""indexed"": false,
          ""name"": ""value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""Transfer"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""name"": ""from"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""name"": ""spender"",
          ""type"": ""address""
        },
        {
          ""indexed"": false,
          ""name"": ""value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""Approval"",
      ""type"": ""event""
    }
    ]";
    #endregion
    
    // We define the contract address here
    // (Remember this contract is deployed on the ropsten network)
    private static string contractAddress = "0xab904d9c85388653e6fb1c75b41fbc0465d39b90";

    // We define a new contract (Netherum.Contracts)
    private Contract contract;

    public NexiumContract()
    {
        this.contract = new Contract(null, ABI, contractAddress);
    }

    #region Balance Of
    public Function Get_balanceOf_Function()
    {
        return contract.GetFunction("balanceOf");
    }
    public CallInput Create_balanceOf_Input(string address)
    {
        // For this transaction to the contract we dont need inputs,
        // its only to retreive the quantity of Nexium on the address
        var function = Get_balanceOf_Function();
        return function.CreateCallInput(address);
    }
    public BigInteger Decode_balanceOf(string balance)
    {
        var function = Get_balanceOf_Function();
        return function.DecodeSimpleTypeOutput<BigInteger>(balance);
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


    #region event - Transfer
    public Nethereum.Contracts.Event GetTransfertEvent()
    {
        return contract.GetEvent("Transfer");
    }


    public NewFilterInput CreateTransferInput()
    {
        var evt = GetTransfertEvent();
        // for this event, when creating the input, we need to define the blockFrom and the blockTo parameters.
        // we send the first block and the last block in this case, so we retrieve the transfert event
        // transactions in all the blocks
        return evt.CreateFilterInput(Nethereum.RPC.Eth.DTOs.BlockParameter.CreateEarliest(), Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
    }
    #endregion
}