using UnityEngine;
using TMPro;
using System;

public class Keybinds : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] public TextMeshProUGUI buttonLblUp;
    [SerializeField] public TextMeshProUGUI buttonLblDown;
    [SerializeField] public TextMeshProUGUI buttonLblLeft;
    [SerializeField] public TextMeshProUGUI buttonLblRight;


    void Start()
    {
        buttonLblUp.text = PlayerPrefs.GetString("CustomKeyUp");
        buttonLblDown.text = PlayerPrefs.GetString("CustomKeyDown");
        buttonLblLeft.text = PlayerPrefs.GetString("CustomKeyLeft");
        buttonLblRight.text = PlayerPrefs.GetString("CustomKeyRight");
    }

    void Update()
    {
        if (buttonLblUp.text == "x")
        {
            foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(keycode))
                {
                    buttonLblUp.text = keycode.ToString();
                    PlayerPrefs.SetString("CustomKeyUp", keycode.ToString());
                    PlayerPrefs.Save();
                }
            }
        }
        if (buttonLblDown.text == "x")
        {
            foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(keycode))
                {
                    buttonLblDown.text = keycode.ToString();
                    PlayerPrefs.SetString("CustomKeyDown", keycode.ToString());
                    PlayerPrefs.Save();
                }
            }
        }
        if (buttonLblLeft.text == "x")
        {
            foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(keycode))
                {
                    buttonLblLeft.text = keycode.ToString();
                    PlayerPrefs.SetString("CustomKeyLeft", keycode.ToString());
                    PlayerPrefs.Save();
                }
            }
        }
        if (buttonLblRight.text == "x")
        {
            foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(keycode))
                {
                    buttonLblRight.text = keycode.ToString();
                    PlayerPrefs.SetString("CustomKeyRight", keycode.ToString());
                    PlayerPrefs.Save();
                }
            }
        }
    }

        public void ChangeKeyUp()
        {
            buttonLblUp.text = "x";
        }
    public void ChangeKeyDown()
    {
        buttonLblDown.text = "x";
    }
    public void ChangeKeyLeft()
    {
        buttonLblLeft.text = "x";
    }
    public void ChangeKeyRight()
    {
        buttonLblRight.text = "x";
    }


}
