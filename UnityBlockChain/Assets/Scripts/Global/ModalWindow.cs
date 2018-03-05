using UnityEngine;
using System.Collections.Generic;

public class ModalWindow : MonoBehaviour
{
    /// <summary>
    /// Initialise la popup avec un titre, un contenu et les actions à effectuer sur les boutons
    /// </summary>
    public void Init()
    {
        Debug.Log("TODO (ModalWindow)");
    }

    public void Close()
    {
        ModalWindowManager.instance.RemoveWindow(this);
        Destroy(gameObject);
    }
}
