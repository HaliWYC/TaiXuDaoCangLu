using System.Collections;
using TMPro;
using TXDCL.Character;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterStatsPanel : Singleton<CharacterStatsPanel>
{
    public GameObject CharaterStats;
    public Image CharacterImage;
    public Image HealthBar;
    public TextMeshProUGUI HealthText;
    public Image ManaBar;
    public TextMeshProUGUI ManaText;
    public Image StaminaBar;
    public TextMeshProUGUI StaminaText;
    public Image ShenShiBar;
    public TextMeshProUGUI ShenShiText;
    
    public IEnumerator UpdateCharacterStats(CharacterBase character)
    {
        var characterData = character.CharacterData;
        CharacterImage.sprite = characterData.characterSprite;
        HealthText.text = characterData.currentHealth + "/" + characterData.maxHealth;
        ManaText.text = characterData.currentMana + "/" + characterData.maxMana;
        StaminaText.text = characterData.currentStamina + "/" + characterData.maxStamina;
        ShenShiText.text = characterData.ShenShi + "/" + characterData.ShenShiStrength;
        HealthBar.fillAmount = characterData.maxHealth == 0 ? 0 : characterData.currentHealth / (float)characterData.maxHealth;
        ManaBar.fillAmount = characterData.maxMana == 0 ? 0 : characterData.currentMana / (float)characterData.maxMana;
        StaminaBar.fillAmount = characterData.maxStamina == 0 ? 0 : characterData.currentStamina / (float)characterData.maxStamina;
        ShenShiBar.fillAmount = characterData.ShenShiStrength == 0 ? 0 : characterData.ShenShi / (float)characterData.ShenShiStrength;
        yield return null;
    }
}
