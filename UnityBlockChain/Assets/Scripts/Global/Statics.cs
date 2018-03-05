using UnityEngine;

/// <summary>
/// Script racine pour tous les scripts globaux (popup modulaire, scripts de gestion)
/// </summary>
public class Statics : MonoBehaviour
{
    public static bool isInit = false;

    private void Awake()
    {
        isInit = true;
        DontDestroyOnLoad(gameObject);
    }
}
