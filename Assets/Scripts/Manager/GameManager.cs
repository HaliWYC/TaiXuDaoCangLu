using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;
using TXDCL.Character;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private CinemachineCamera gameCamera;
    private float gameCameraLenSize;
    public CharacterBase Player;
    
    protected override void Awake()
    {
        base.Awake();
        gameCameraLenSize = gameCamera.Lens.OrthographicSize;
    }
    public void SetGameCameraLenInGridSize(int range)
    {
        if(range <=16) return;
        var offset = gameCameraLenSize + range + 2 - 16;
        SwitchBounds.Instance.Confiner2D.OversizeWindow.Enabled = true;
        DOVirtual.Float(gameCameraLenSize, offset, 0.5f, value => gameCamera.Lens.OrthographicSize = value);
    }

    public void ResetGameCameraLenInGridSize()
    {
        SwitchBounds.Instance.Confiner2D.OversizeWindow.Enabled = false;
        CombatUI.Instance.IgnoreCombatPanel(false);
        gameCamera.Lens.OrthographicSize = gameCameraLenSize;
    }
}
