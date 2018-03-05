using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public static Loader instance { get; private set; }

    void Awake()
    {
        instance = this;
    }

    public void StartLoad()
    {
        gameObject.SetActive(true);
    }

    public void StopLoad()
    {
        gameObject.SetActive(false);
    }
}
