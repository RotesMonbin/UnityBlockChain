using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainBoard : MonoBehaviour
{

    [SerializeField]
    private Text etherBalance;

    [SerializeField]
    private Text address;


    public void setBalance(float balance)
    {
        this.etherBalance.text = balance + "";
    }

    public void setAddress(string address)
    {
        this.address.text = address;
        StartCoroutine(ProviderManager.instance.getAccountBalance((balance) =>
        {
            this.etherBalance.text = balance + "";
        }));
    }
}
