using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResizer : MonoBehaviour
{
    private float targetHeight;
    
    public float baseOrthographicSize = 8.5f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetHeight = 2000;
        AdjustSize();
    }

    void AdjustSize()
    {
        float targetAspect = targetHeight / 1080f;
        float currentAspect = (float)Screen.height / Screen.width;

        float orthographicSize;

        if (currentAspect >= targetAspect)
        {
            orthographicSize = baseOrthographicSize * (currentAspect / targetAspect);
        }
        else
        {
            orthographicSize = baseOrthographicSize / (targetAspect / currentAspect);
        }

        cam.orthographicSize = orthographicSize;
    }

    
}
