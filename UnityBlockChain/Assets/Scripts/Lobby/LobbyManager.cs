using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
	public static LobbyManager instance { get; private set; }

    #region UI

    public GameObject Canvas;
    public Text PlayerName;
    public Image Avatar;

    #endregion

    private void Awake()
	{
		instance = this;
        if(!Statics.isInit)
            SceneManager.LoadScene("Global", LoadSceneMode.Additive);
	}

	public void Activate(string publicAddress, Sprite blocky)
    {
        Canvas.SetActive(true);
        PlayerName.text = publicAddress;
        Avatar.sprite = blocky;
    }
}
