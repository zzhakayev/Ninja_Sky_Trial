
using UnityEngine;

public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private int lastWidth;
    private int lastHeight;
    
    void Update()
    {
        if (Screen.safeArea != lastSafeArea ||
        Screen.width != lastWidth ||
        Screen.height != lastHeight)
        {
            ApplySafeArea();
            }
    }


    void ApplySafeArea()
    {
        Rect area = Screen.safeArea;

        Vector2 min = area.position;
        Vector2 max = area.position + area.size;

        min.x /= Screen.width;
        min.y /= Screen.height;

        max.x /= Screen.width;
        max.y /= Screen.height;

        rectTransform.anchorMin = min;
        rectTransform.anchorMax = max;

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        lastSafeArea = area;
        lastWidth = Screen.width;
        lastHeight = Screen.height;

    }
}
