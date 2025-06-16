using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    private CinemachineConfiner2D confiner2D;

    [Header("EventListener")]
    public VoidEventSO afterSceneLoadedEvent;

    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
    }
    private void OnDisable()
    {
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        GetNewCameraBounds();
    }

    private void GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if(obj == null)
        {
            Debug.Log("’“≤ªµΩ±ﬂΩÁBounds");
            return;
        }
        Debug.Log(obj.name);
        confiner2D.BoundingShape2D = obj.GetComponent<Collider2D>();
        confiner2D.InvalidateBoundingShapeCache();
    }

}
