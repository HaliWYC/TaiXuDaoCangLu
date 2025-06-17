using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    private CinemachineConfiner2D Confiner2D;

    private void Awake()
    {
        Confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }
    
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }

    private void OnAfterSceneLoadEvent()
    {
        SwitchConfinerShape();
    }
    private void SwitchConfinerShape()
    {
        var polygonCollider = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();
        Confiner2D.BoundingShape2D = polygonCollider;
        Confiner2D.InvalidateBoundingShapeCache();
    }
}