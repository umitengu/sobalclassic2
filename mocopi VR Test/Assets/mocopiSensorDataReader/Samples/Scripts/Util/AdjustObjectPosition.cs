//
// Copyright 2023 Sony Corporation
//
using UnityEngine;

namespace Mocopi.Sensor.DataReader.Sample
{
    public class AdjustObjectPosition : MonoBehaviour
    {
        public RectTransform TargetUIElement;
        public GameObject ObjectToPlace;
        public float BaseDistanceBelowUI = 300f;

        private Vector2 _baseResolution = new Vector2(1080, 1920);

        private void Start()
        {
            PlaceObject();
        }

        private void PlaceObject()
        {
            float resolutionScale = Mathf.Min(Screen.width / _baseResolution.x, Screen.height / _baseResolution.y);
            float scaledDistanceBelowUI = BaseDistanceBelowUI * resolutionScale;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, TargetUIElement.position);
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y - (TargetUIElement.rect.height / 2) - scaledDistanceBelowUI, Camera.main.nearClipPlane));
            ObjectToPlace.transform.position = new Vector3(worldPoint.x, worldPoint.y, ObjectToPlace.transform.position.z);
        }
    }
}
