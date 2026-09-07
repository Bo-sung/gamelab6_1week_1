using System.Collections.Generic;
using System;
using UnityEngine;
using static GameScene;

public class UIMiniMap : MonoBehaviour, IMiniMap
{
    List<IMapElement> mapElements = new List<IMapElement>();
    List<UIMapMarker> mapMarkers = new List<UIMapMarker>();

    [SerializeField]
    RectTransform poolTransform;
    [SerializeField]
    Transform centerTarget;

    [SerializeField]
    float zoomScale;
    [SerializeField]
    float maxDistance;

    private float updateInterval = 0.05f;

    [SerializeField]
    UIMapMarker markerOrigin;

    float updateTimer = 0f;
    void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer < updateInterval)
            return;
        updateTimer = 0f;
        UpdateMapPositions();
    }

    private void UpdateMapPositions()
    {
        if (centerTarget == null)
            return;

        for (int i = 0; i < mapElements.Count; i++)
        {
            IMapElement mapElement = mapElements[i];
            UIMapMarker marker = mapMarkers[i];

            RectTransform markerRect = marker.transform as RectTransform;

            UpdateMapPosition(mapElement.GetTransform(), markerRect, centerTarget);
        }
    }

    private void UpdateMapPosition(Transform target, RectTransform markerRect, Transform center)
    {
        if (!target.gameObject.activeInHierarchy)
        {
            markerRect.gameObject.SetActive(false);
            return;
        }
        else if (!markerRect.gameObject.activeInHierarchy)
        {
            markerRect.gameObject.SetActive(true);
        }

        Vector3 targetPos = target.position;
        Vector3 centerPos = center.position;

        Vector3 dir = centerPos - targetPos;

        Quaternion inverseRotation = Quaternion.Euler(0f, -center.eulerAngles.y, 0f);

        Vector3 rotatedDir = inverseRotation * dir;

        Vector2 mapPosition = new Vector2(rotatedDir.x, rotatedDir.z) * zoomScale;

        // 미니맵 반경 밖으로 나가지 않도록 제한
        if (mapPosition.sqrMagnitude > maxDistance * maxDistance)
        {
            mapPosition = mapPosition.normalized * maxDistance;
        }

        markerRect.anchoredPosition = mapPosition;
    }

    public void Init(Transform center)
    {
        centerTarget = center;
    }

    public void AddElement(IMapElement element)
    {
        mapElements.Add(element);
        var obj = Instantiate(markerOrigin.gameObject, poolTransform);
        var marker = obj.GetComponent<UIMapMarker>();
        marker.SetData(element);
        mapMarkers.Add(marker);
    }
}
