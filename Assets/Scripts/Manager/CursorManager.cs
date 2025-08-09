using TXDCL.Combat;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : Singleton<CursorManager>
{
    private Camera MainCamera;
    private Grid currentGrid;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;
    private bool cursorEnable;
    public bool isSelecting;//检测是否选中
    public bool isConfirm;
    public bool isCastingFaShu;//检测是否释放法术中
    
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CombatGridManager.Instance.DisplayCharactersMovementPath();
            DaoCangPanelUI.Instance.ResetDaoCangPanelUI();
        }
        if (!Input.GetMouseButtonDown(0) || InteractWithUI()) return;
        if (isCastingFaShu)
        {
            if (isConfirm)
            {
                //获得目标范围内所有目标角色并执行法术
                CombatGridManager.Instance.CheckFaShuConfirmTargets((Vector2Int)mouseGridPos);
                return;
            }
            //显示确认范围
            CombatGridManager.Instance.DisplayFaShuConfirmPath((Vector2Int)mouseGridPos);
        }
        else
        {
            if (isConfirm)
            {
                MoveConfirmPath();
                return;
            }
            //显示确认路径
            CombatGridManager.Instance.CheckInPotentialMovementPath((Vector2Int)mouseGridPos);
        }
    }
    
    private void CheckCursorValid()
    {
        mouseWorldPos = MainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -MainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
    }
    
    private void MoveConfirmPath()
    {
        //移动
        CombatGridManager.Instance.CheckInConfirmMovementPath((Vector2Int)mouseGridPos);
    }

    /// <summary>
    /// 是否与UI互动
    /// </summary>
    /// <returns></returns>
    public bool InteractWithUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}