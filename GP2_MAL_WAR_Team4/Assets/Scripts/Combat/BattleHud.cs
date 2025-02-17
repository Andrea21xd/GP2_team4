using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BattleHUD : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text speedText;
    public TMP_Text LuckText;
    public Slider hpSlider;



    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        speedText.text = "Speed: " + unit.speed.ToString();
        LuckText.text = "Luck: " + unit.luck.ToString();
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;

    }

    public void UpdateStats(Unit unit)
    {
        speedText.text = "Speed: " + unit.speed;
        LuckText.text = "Luck: " + unit.luck;
        hpSlider.value = unit.currentHP;
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }
}
