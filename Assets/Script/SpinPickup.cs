using UnityEngine;

public class SpinPickup : MonoBehaviour
{
    private Vector3 origin;

    void Start()
    {
        origin = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, 125f * Time.deltaTime, Space.World);
        transform.position = origin + Vector3.up * Mathf.Sin(Time.time * 3f) * 0.14f;
    }
}
