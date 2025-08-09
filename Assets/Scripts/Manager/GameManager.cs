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
        var offset = gameCameraLenSize + range - 16;
        DOVirtual.Float(gameCameraLenSize,offset,(range-16)/2f, value => gameCamera.Lens.OrthographicSize = value);
    }

    public void ResetGameCameraLenInGridSize()
    {
        gameCamera.Lens.OrthographicSize = gameCameraLenSize;
    }
}
