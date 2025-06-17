using System;
using UnityEngine;
using DG.Tweening;
public class FadePanel : Singleton<FadePanel>
{
    [SerializeField] private CanvasGroup Icon;

    protected override void Awake()
    {
        base.Awake();
        Icon = GetComponent<CanvasGroup>();
    }

    public void FadeIn(float duration)
    {
        DOVirtual.Float(0f,1f,duration,value=>Icon.alpha = value).SetEase(Ease.InQuad);
    }

    public void FadeOut(float duration)
    {
        DOVirtual.Float(1f,0f,duration,value=>Icon.alpha = value).SetEase(Ease.InQuad);
    }
}