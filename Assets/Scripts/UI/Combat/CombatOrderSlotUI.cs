using TXDCL.Character;
using UnityEngine;
using UnityEngine.UI;

public class CombatOrderSlotUI : MonoBehaviour
{
    public Image characterIcon;
    public void SetCharacterIcon(CharacterBase character)
    {
        characterIcon.sprite = character.CharacterData.characterSprite;
    }
}
