using UnityEngine;

public class TriggerItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemFader[] fader = other.GetComponentsInChildren<ItemFader>();

        if (fader.Length <= 0) return;
        foreach (var item in fader)
        {
            item.FadeIn();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ItemFader[] fader = other.GetComponentsInChildren<ItemFader>();

        if (fader.Length <= 0) return;
        foreach (var item in fader)
        {
            item.FadeOut();
        }
    }
}
