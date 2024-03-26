using UnityEngine;

namespace Tools
{
    public class CameraResizer : MonoBehaviour
    {
        private float _targetHeight;
    
        public float baseOrthographicSize = 8.5f;

        private Camera _cam;

        void Start()
        {
            _cam = GetComponent<Camera>();
            _targetHeight = 2000;
            AdjustSize();
        }

        void AdjustSize()
        {
            float targetAspect = _targetHeight / 1080f;
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

            _cam.orthographicSize = orthographicSize;
        }

    
    }
}
