using TXDCL.Combat;
using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    private Camera MainCamera;
    private Grid currentGrid;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;
    private bool cursorEnable;
    public bool isSelecting;//检测是否选中
    public bool isConfirm;
    
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
        currentGrid = FindFirstObjectByType<Grid>();
    }
    
    private void Start()
    {
        MainCamera = Camera.main;
        cursorEnable = true;
    }

    private void Update()
    {
        if (!cursorEnable) return;
        if (!isSelecting) return;
        CheckCursorValid();
        if (!Input.GetMouseButtonDown(0)) return;
        if (isConfirm)
        {
            MoveConfirmPath();
        }
        //显示确认路径
        CombatGridPath.Instance.CheckInPotentialPath((Vector2Int)mouseGridPos);
    }
    
    private void CheckCursorValid()
    {
        mouseWorldPos = MainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -MainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
    }
    
    private void MoveConfirmPath()
    {
        if (!isConfirm) return;
        //移动
        CombatGridPath.Instance.CheckInConfirmPath((Vector2Int)mouseGridPos);
    }
}