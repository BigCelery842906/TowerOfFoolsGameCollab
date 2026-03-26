//-------------------------------------
//      Property of Dan.
//      Break it and you suffer.
//      Respectfully of course...
//-------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System;

public class PG_PlatformParent : MonoBehaviour
{
    
    public List<GameObject> m_powerupPool;
    public float m_worldScale;
    //[NonSerialized]
    public bool m_hasPowerup = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public bool SpawnPowerup(GameObject room)
    {
        if(m_hasPowerup == true)
        {
            Debug.Log("Platform already has a powerup!");
            return false; ;
        }
        int randomNum = UnityEngine.Random.Range(0, m_powerupPool.Count);
        GameObject selected = m_powerupPool[randomNum];
        Vector3 pos = new Vector3(this.transform.position.x, this.transform.position.y + (m_worldScale * 1.5f), this.transform.position.z);
        GameObject spawned = GameObject.Instantiate(selected, pos, this.transform.rotation);
        spawned.transform.SetParent(this.gameObject.transform);
        m_hasPowerup = true;
        return true;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    public void EnableGravity()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (!rb) Debug.Log("No Rigid Body Attached");

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }
}
