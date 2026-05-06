using Unity.VisualScripting;
using UnityEngine;

public class c_CloudMovement : MonoBehaviour
{
    [Range(0, 15)] [SerializeField] private float minCloudMovement = 2.0f;
    [Range(0, 15)] [SerializeField] private float maxCloudMovement = 2.0f;
    private float m_CloudMovement = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       PickRandomSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.localPosition;
        position.y -= m_CloudMovement * Time.deltaTime;
        transform.localPosition = position;
        
        if (transform.localPosition.y < -120.0f)
        {
            MoveCloudToTop();
            PickRandomSpeed();
        }
    }

    void PickRandomSpeed()
    {
        m_CloudMovement = Random.Range(Mathf.Min(minCloudMovement, maxCloudMovement), Mathf.Max(minCloudMovement, maxCloudMovement));

        if (Mathf.FloorToInt(m_CloudMovement) % 2 == 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    void MoveCloudToTop()
    {
        Vector3 position = transform.localPosition;
        position.y = 140.0f;
        transform.localPosition = position;
    }
}
