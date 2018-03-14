using Nethereum.Hex.HexTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransactionTest : MonoBehaviour {

    private NexiumContract m_nexiumContract = new NexiumContract();

    public void SendTestTransaction()
    {
        var transactionInput = m_nexiumContract.Create_approve_TransactionInput(
            ProviderManager.instance.accountAddress,
            "0x81b7e08f65bdf5648606c89998a9cc8164397647",
            10,
            new HexBigInteger(50000),
            new HexBigInteger(10),
            new HexBigInteger(0)
        );

        ProviderManager.instance.SendTransaction(transactionInput);
    }
}
