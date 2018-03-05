using UnityEngine;
using System.Collections.Generic;

public class LobbyTabManager : MonoBehaviour
{
	public static LobbyTabManager instance { get; private set; }
    
	private void Awake()
	{
		instance = this;
	}

    private enum Tab
    {
        First,
        Second,
        Third,
        Fourth
    }

    private Tab m_CurrentTab = Tab.First;

    private void ClosePreviousTab()
    {
        switch(m_CurrentTab)
        {
            case Tab.First:
                CloseFirstTab();
                break;
            case Tab.Second:
                CloseSecondTab();
                break;
            case Tab.Third:
                CloseThirdTab();
                break;
            case Tab.Fourth:
                CloseFourthTab();
                break;
        }
    }

    #region First
    
    public void OpenFirstTab()
    {
        ClosePreviousTab();
        m_CurrentTab = Tab.First;
    }

    public void CloseFirstTab()
    {

    }
    #endregion

    #region Second
    
    public void OpenSecondTab()
    {
        ClosePreviousTab();
        m_CurrentTab = Tab.Second;
    }

    public void CloseSecondTab()
    {

    }
    #endregion

    #region Third

    internal void OpenThirdTab()
    {
        ClosePreviousTab();
        m_CurrentTab = Tab.Third;
    }

    internal void CloseThirdTab()
    {

    }
    #endregion

    #region Fourth

    internal void OpenFourthTab()
    {
        ClosePreviousTab();
        m_CurrentTab = Tab.Fourth;
    }

    internal void CloseFourthTab()
    {

    }
    #endregion
}
