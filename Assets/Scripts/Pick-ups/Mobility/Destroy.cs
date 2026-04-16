using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(PickupDespawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator PickupDespawn()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
