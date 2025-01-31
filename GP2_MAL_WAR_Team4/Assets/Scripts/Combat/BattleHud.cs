using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BattleHUD : MonoBehaviour
{
    public TMP_Text nameText;
    public Slider hpSlider;



    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;

    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }
}
