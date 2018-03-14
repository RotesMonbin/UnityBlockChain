using Nethereum.RPC.Eth.DTOs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignTransactionManager : MonoBehaviour {

    [SerializeField]
    private InputField password;

    private TransactionInput transac;

    public void OpenAndSignTransaction(TransactionInput transac)
    {
        this.transac = transac;
        this.gameObject.SetActive(true);

    }

    public void ConfirmPassword()
    {
        ProviderManager.instance.ConfirmTransactionWithPassword(transac, password.text);
        this.gameObject.SetActive(false);
    }


}
