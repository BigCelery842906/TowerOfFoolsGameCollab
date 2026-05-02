using System;
using UnityEngine;

public class PG_DetectorTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("hit");
        //if (other.gameObject.CompareTag("Player0") || other.gameObject.CompareTag("Player1"))
        if(other.gameObject.CompareTag("Player"))
        {
            PG_TransitionManager.instance.GenerateRoom();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
