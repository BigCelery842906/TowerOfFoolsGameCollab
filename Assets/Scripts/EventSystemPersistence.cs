using System;
using UnityEngine;

public class EventSystemPersistence : MonoBehaviour
{
    private static EventSystemPersistence instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this) 
        {
            instance = null;
        }
    }
}
