
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Hazard[] prefabs;

    void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, 2f);
    }

    void Spawn()
    {
        if (prefabs == null || prefabs.Length == 0)
            return;

        float x = Random.Range(-4f, 4f);
        float z = Random.Range(-3f, 3f);

        Vector3 position = new Vector3(x, 6f, z);

        Hazard selected = prefabs[Random.Range(0, prefabs.Length)];

        Hazard spawned = Instantiate(
            selected,
            position,
            Quaternion.identity
        );

        Destroy(spawned.gameObject, 7f);
    }
}
