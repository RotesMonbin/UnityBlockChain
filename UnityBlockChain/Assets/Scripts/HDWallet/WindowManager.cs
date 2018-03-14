using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour {

    [SerializeField]
    private WordsChainLoader WordsChainCanvas;

    [SerializeField]
    private MainBoard MainBoard;

    public void OpenClick()
    {
        if (ProviderManager.instance.usingIntegratedProvider)
        {
            OpenMainBoard();
            Debug.Log("using metamask");
        }
        else
        {
            Debug.Log("Not using metamask");
            if (ProviderManager.instance.LoadJSONFromDisk())
            {
                OpenMainBoard();
            }
            else
            {
                OpenWordsChainCanvas();
            }
        }
    }

    public void OpenWordsChainCanvas()
    {
        WordsChainCanvas.gameObject.SetActive(true);
        MainBoard.gameObject.SetActive(false);
    }

    public void OpenMainBoard()
    {
        WordsChainCanvas.gameObject.SetActive(false);
        MainBoard.gameObject.SetActive(true);
        if (!ProviderManager.instance.usingIntegratedProvider)
        {
            MainBoard.setAddress(ProviderManager.instance.accountAddress);
            MainBoard.setBalance(ProviderManager.instance.ethereumBalance);
        }
    }
}
