using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TXDCL.Astar;
using TXDCL.Character;
using TXDCL.Combat;
using TXDCL.XiuLian.FuShu;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCController : CharacterBase
{
    private static readonly int CastFaShu = Animator.StringToHash("CastFaShu");
    private List<FaShuData> availableFaShu = new();
    private CharacterBase currentEnemy;
    private Vector2Int currentPosition;
    private Vector2Int targetPosition;
    private Stack<MovementStep> movementSteps = new();
    private bool isFirstSelecting;//判断是否为首次选取法术，如首次未有可释放的法术则仅往目标位置移动，否则不移动
    private void OnEnable()
    {
        EventHandler.BeforeCombatBeginEvent += OnBeforeCombatBeginEvent;
        EventHandler.AfterCombatBeginEvent += OnAfterCombatBeginEvent;
        EventHandler.CharacterTurnBeginEvent += OnCharacterTurnBeginEvent;
        EventHandler.AfterFaShuReleasedEvent += OnAfterFaShuReleasedEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeCombatBeginEvent -= OnBeforeCombatBeginEvent;
        EventHandler.AfterCombatBeginEvent -= OnAfterCombatBeginEvent;
        EventHandler.CharacterTurnBeginEvent -= OnCharacterTurnBeginEvent;
        EventHandler.AfterFaShuReleasedEvent -= OnAfterFaShuReleasedEvent;
    }
    
    protected override void OnBeforeCombatBeginEvent()
    {
        base.OnBeforeCombatBeginEvent();
        CombatManager.Instance.RegisterEnemySide(this);
    }
    private void OnAfterCombatBeginEvent()
    {
        Allies.AddRange(CombatManager.Instance.EnemySides);
        Enemies.AddRange(CombatManager.Instance.PlayerSides);
    }

    protected override void OnCharacterTurnBeginEvent(CharacterBase character)
    {
        base.OnCharacterTurnBeginEvent(character);
        if (character != this) return;
        availableFaShu.Clear();
        isFirstSelecting = true;
        SelectEnemy();
        SelectPotentialFaShu();
    }
    private void OnAfterFaShuReleasedEvent(FaShuData fashuData)
    {
        if(availableFaShu.Count == 0) return;
        isFirstSelecting = false;
        SelectEnemy();
        SelectPotentialFaShu();
    }

    private void SelectEnemy()
    {
        if (Enemies.Count == 0) return;
        currentEnemy = Enemies[Random.Range(0, Enemies.Count)];
    }

    private void SelectPotentialFaShu()
    {
        if (availableFaShu.Count == 0)
        {
            //初始化本回合潜在能够法术
            foreach (var fashu in currentFaShuList.Where(fashu =>
                         FaShuManager.Instance.CheckReleaseFaShuConditions(CharacterData, fashu, false) &&
                         CheckEnoughDistance(fashu.ReleaseRange)))
            {
                availableFaShu.Add(fashu);
            }
        }
        else
        {
            //删除不能释放的法术，留下潜在能够释放的法术
            foreach (var fashu in availableFaShu.Where(fashu =>
                         !FaShuManager.Instance.CheckReleaseFaShuConditions(CharacterData, fashu, false) ||
                         !CheckEnoughDistance(fashu.ReleaseRange)).ToList())
            {
                availableFaShu.Remove(fashu);
            }
        }
        //释放法术
        if (availableFaShu.Count != 0)
        {
            StartCoroutine(ReleaseFaShu(availableFaShu[Random.Range(0, availableFaShu.Count - 1)]));
            return;
        }
        
        if (CharacterData.currentMovement > 0 && isFirstSelecting)
        {
            StartCoroutine(Movement());
        }
        else
        {
            EventHandler.CallCharacterTurnEndEvent(this);
            CombatManager.Instance.isCharacterTurnActive = false;
        }
        
    }

    private IEnumerator Movement()
    {
        //执行移动
        movementSteps.Clear();
        var startPos = new Vector2Int((int)(transform.position.x - 0.5f), (int)(transform.position.y - 0.5f));
        var endPos = new Vector2Int((int)(currentEnemy.transform.position.x - 0.5f), (int)(currentEnemy.transform.position.y - 0.5f));
        CombatGridManager.Instance.SetGridObstacle(CombatGridManager.Instance.CharacterPositionsInCombatDict[currentEnemy],false);
        AStar.Instance.BuildPath(SceneManager.GetActiveScene().name, startPos, endPos, movementSteps);
        CombatGridManager.Instance.SetGridObstacle(CombatGridManager.Instance.CharacterPositionsInCombatDict[currentEnemy],true);
        SetCharacterFacingDirection(currentEnemy.transform.position.x - transform.position.x);
        combatMovement.BuildPath(movementSteps, false, 1);
        yield return new WaitUntil(() => combatMovement.arriveTargetPosition);
        EventHandler.CallCharacterTurnEndEvent(this);
        CombatManager.Instance.isCharacterTurnActive = false;
    }
    
    /// <summary>
    /// 根据法术释放范围指定最短移动路径并且等到到达终点后才执行之后的动作
    /// </summary>
    /// <param name="faShuData"></param>
    /// <returns></returns>
    private IEnumerator ReleaseFaShu(FaShuData faShuData)
    {
        //执行移动
        movementSteps.Clear();
        var startPos = new Vector2Int((int)(transform.position.x - 0.5f), (int)(transform.position.y - 0.5f));
        var endPos = new Vector2Int((int)(currentEnemy.transform.position.x - 0.5f), (int)(currentEnemy.transform.position.y - 0.5f));
        CombatGridManager.Instance.SetGridObstacle(CombatGridManager.Instance.CharacterPositionsInCombatDict[currentEnemy],false);
        AStar.Instance.BuildPath(SceneManager.GetActiveScene().name, startPos, endPos, movementSteps);
        CombatGridManager.Instance.SetGridObstacle(CombatGridManager.Instance.CharacterPositionsInCombatDict[currentEnemy],true);
        SetCharacterFacingDirection(currentEnemy.transform.position.x - transform.position.x);
        combatMovement.BuildPath(movementSteps, false, faShuData.ReleaseRange);
        yield return new WaitUntil(() => combatMovement.arriveTargetPosition);
        //执行法术
        //SetCharacterFacingDirection(currentEnemy.transform.position.x - transform.position.x);
        animator.SetTrigger(CastFaShu);
        FaShuManager.Instance.ReleaseFaShu(faShuData, currentEnemy.transform.position,this,
            CombatGridManager.Instance.GetAllGridInCombatDict(CombatGridManager.Instance.FindPotentialPath(
                CombatGridManager.Instance.CharacterPositionsInCombatDict[currentEnemy], faShuData.Range, true)));
    }
    /// <summary>
    /// 检测当前角色企图攻击的目标是否在自己最大可移动范围加上法术范围内
    /// </summary>
    /// <param name="enemy">攻击目标</param>
    /// <param name="FaShuReleaseRange">法术释放范围</param>
    /// <returns></returns>
    private bool CheckEnoughDistance(int FaShuReleaseRange)
    {
        var maxDistance = (FaShuReleaseRange + CharacterData.currentMovement) * 10;
        currentPosition = CombatGridManager.Instance.CharacterPositionsInCombatDict[this];
        targetPosition = CombatGridManager.Instance.CharacterPositionsInCombatDict[currentEnemy];
        var TargetDistance = AStar.Instance.GetDistance(new AStarNode(currentPosition), new AStarNode(targetPosition));
        // Debug.Log("TargetDistance: " + TargetDistance);
        // Debug.Log("MaxDistance:" + maxDistance);
        return TargetDistance <= maxDistance;
    }
}
