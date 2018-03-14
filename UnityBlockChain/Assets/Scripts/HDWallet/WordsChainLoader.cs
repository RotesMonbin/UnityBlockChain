using Nethereum.HdWallet;
using Nethereum.KeyStore;
using Nethereum.Signer;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class WordsChainLoader : MonoBehaviour {



    [SerializeField]
    private InputField[] Words = new InputField[12];

    [SerializeField]
    private InputField Password;

    // Use this for initialization
    void Start () {
        Words[0].text = "dizzy";
        Words[1].text = "grow";
        Words[2].text = "empower";
        Words[3].text = "decrease";
        Words[4].text = "divert";
        Words[5].text = "gossip";
        Words[6].text = "name";
        Words[7].text = "claw";
        Words[8].text = "cycle";
        Words[9].text = "fruit";
        Words[10].text = "comfort";
        Words[11].text = "gospel";
    }

    public void RetrieveWallet()
    {
        string wordsChain = Words[0].text + " " + Words[1].text + " " + Words[2].text + " " + Words[3].text + " " + Words[4].text + " " + Words[5].text + " " + Words[6].text + " " + Words[7].text + " " + Words[8].text + " " + Words[9].text + " " + Words[10].text + " " + Words[11].text;
        ProviderManager.instance.InstantiateWalletAndSaveOnDisk(wordsChain, Password.text);
        this.gameObject.SetActive(false);
    }



}
