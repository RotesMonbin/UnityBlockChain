using UnityEngine;
using System.Collections.Generic;

public class ModalWindowManager : MonoBehaviour
{
	public static ModalWindowManager instance { get; private set; }

    /// <summary>
    /// Liste des fenêtres ouvertes
    /// </summary>
    private List<ModalWindow> m_List_OpenedWindows = new List<ModalWindow>();

    /// <summary>
    /// Modèle de base repris pour faire toute les fenêtres
    /// </summary>
    public GameObject m_Prefab_WindowModel;

	private void Awake()
	{
		instance = this;
	}

    /// <summary>
    /// Ouvre une popup en instantiant une nouvelle fenêtre
    /// </summary>
	public void Open()
    {
        ModalWindow l_ModalWindow = Instantiate(m_Prefab_WindowModel).GetComponent<ModalWindow>();
        m_List_OpenedWindows.Add(l_ModalWindow);
        l_ModalWindow.Init();
    }

    /// <summary>
    /// Ferme toute les popups d'un coup
    /// </summary>
    public void CloseAll()
    {
        for(int l_i_Index = m_List_OpenedWindows.Count - 1 ; l_i_Index >= 0 ; --l_i_Index)
        //Parcours inverse de la liste parce que les éléments sont retirés au fur et à mesure
        {
            m_List_OpenedWindows[l_i_Index].Close();
        }
    }

    public void RemoveWindow(ModalWindow p_ModalWindow)
    {
        m_List_OpenedWindows.Remove(p_ModalWindow);
    }
}
