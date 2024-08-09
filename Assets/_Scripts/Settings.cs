using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [Header("Placeholders")]
    public TMP_Text focusPlaceholder;
    public TMP_Text shortBreakPlaceholder;
    public TMP_Text longBreakPlaceholder;

    [Space]
    [Header("Input")]    
    public TMP_InputField focusInput;
    public TMP_InputField shortBreakInput;
    public TMP_InputField longBreakInput;

    public Animator settingTransition;

    private PomodoroTimer pomodoro;

    void Start()
    {
        pomodoro = GetComponent<PomodoroTimer>();
    }
    
    public void OpenSetting()
    {
        focusInput.text = pomodoro.focusTime.ToString();
        shortBreakInput.text = pomodoro.shortBreakTime.ToString();
        longBreakInput.text = pomodoro.longBreakTime.ToString();

        settingTransition.SetBool("isSetting", true);
    }

    public void ApplySettings()
    {
        pomodoro.focusTime = Convert.ToInt32(focusInput.text);
        pomodoro.shortBreakTime = Convert.ToInt32(shortBreakInput.text);
        pomodoro.longBreakTime = Convert.ToInt32(longBreakInput.text);

        pomodoro.comboBoxSessionType.value = 0; 
        pomodoro.BtnReset_Click();
        
        settingTransition.SetBool("isSetting", false);
    }
}
