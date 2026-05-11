using Unity.VisualScripting;
using UnityEngine;

public class c_CameraCloud : MonoBehaviour
{
    [Range(0, 15)] [SerializeField] private float minCloudMovement = 2.0f;

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.localPosition;
        position.y += minCloudMovement * Time.deltaTime;
        transform.localPosition = position;
    }
}
