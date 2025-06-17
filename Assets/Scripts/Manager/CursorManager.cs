using System;
using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    private Camera MainCamera;
    private Grid currentGrid;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;
    
    private void OnEnable()
    {
        //FIXME:解决event问题
        cursorEnable = false;
    }

    private void Start()
    {
        MainCamera = Camera.main;
        currentGrid = FindFirstObjectByType<Grid>();
    }

    private void Update()
    {
        if(cursorEnable)
            CheckCursorValid();
    }

    private void CheckCursorValid()
    {
        mouseWorldPos = MainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -MainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
    }
}