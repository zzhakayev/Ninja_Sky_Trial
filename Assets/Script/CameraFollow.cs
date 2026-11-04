
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Transform arenaCenter;
    public float referenceAspect = 16f / 9f;
    public float portraitZoomStrength = 0.6f;

    private Vector3 offset;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null || cam == null)
            return;

        bool isPortrait = cam.aspect < 1f;

        Vector3 focus = target.position;

        if (isPortrait && arenaCenter != null)
        {
            focus = arenaCenter.position;
        }

        float ratio = referenceAspect / cam.aspect;

        float zoom = 1f + Mathf.Max(0f, ratio - 1f)
            * portraitZoomStrength;

        transform.position = focus + offset * zoom;
    }
}
