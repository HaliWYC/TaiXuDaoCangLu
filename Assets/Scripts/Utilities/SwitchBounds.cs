using System;
using Unity.Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    private void Start()
    {
        SwitchConfinerShape();
    }

    private void SwitchConfinerShape()
    {
        PolygonCollider2D polygonCollider = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();
        CinemachineConfiner2D confiner = GetComponent<CinemachineConfiner2D>();
        confiner.BoundingShape2D = polygonCollider;
        
        confiner.InvalidateBoundingShapeCache();
    }
}
