using System;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void FadeIn()
    {
        Color targetColor = new Color(1, 1, 1, Settings.TargetAlpha);
        spriteRenderer.DOColor(targetColor, Settings.FadeDuration);
    }
    
    public void FadeOut()
    {
        Color targetColor = new Color(1, 1, 1, 1);
        spriteRenderer.DOColor(targetColor, Settings.FadeDuration);
    }
    
}
