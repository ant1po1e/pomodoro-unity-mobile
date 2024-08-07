using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PomodoroTimer : MonoBehaviour
{
    [Header("Timer Inputs")]
    public TMP_Text timerText;
    public TMP_Text sessionText;
    public TMP_Text playPauseText;
    public TMP_Dropdown comboBoxSessionType;
    public Slider progressBar;
    public AudioClip alertSound;

    [Header("Timer Settings")]
    public int focusTime = 25;
    public int shortBreakTime = 5;
    public int longBreakTime = 15;
    public int minute = 60;

    [Space]
    [Header("Popup Inputs")]
    public GameObject popupPanel;
    public TMP_Text popupText;

    [Space]
    public Animator anim;

    private AudioSource audioSource;
    private float timeLeft;
    private bool isWorkTime;
    private bool manualChange = true;
    private int sessionCount;
    private bool timerRunning;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        isWorkTime = true;
        timeLeft = focusTime * minute;
        sessionCount = 0;
        timerRunning = false;

        sessionText.text = $"{sessionCount + 1} of 4 sessions";
        progressBar.maxValue = focusTime * minute;
        progressBar.value = timeLeft;

        comboBoxSessionType.options.Clear();
        comboBoxSessionType.options.Add(new TMP_Dropdown.OptionData("Focus"));
        comboBoxSessionType.options.Add(new TMP_Dropdown.OptionData("Short Break"));
        comboBoxSessionType.options.Add(new TMP_Dropdown.OptionData("Long Break"));
        comboBoxSessionType.value = 0; 
        comboBoxSessionType.onValueChanged.AddListener(delegate { ComboBoxSessionType_OnSelectedIndexChanged(); });

        UpdateTimeDisplay();
    }

    private void Update()
    {
        if (timerRunning && timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimeDisplay();
            progressBar.value = timeLeft;
        }
        else if (timerRunning && timeLeft <= 0)
        {
            timerRunning = false;
            audioSource.PlayOneShot(alertSound);
            if (isWorkTime)
            {
                if (sessionCount < 3)
                {
                    MessageBox("Work time is over! Take a short break.");
                    manualChange = false;
                    comboBoxSessionType.value = 1;
                    timeLeft = shortBreakTime * minute; 
                }
                else
                {
                    MessageBox("Work time is over! Take a long break.");
                    manualChange = false;
                    comboBoxSessionType.value = 2; 
                    timeLeft = longBreakTime * minute; 
                    sessionCount = -1;
                }
            }
            else
            {
                MessageBox("Break time is over! Back to work.");
                manualChange = false;
                timeLeft = focusTime * minute; 
                comboBoxSessionType.value = 0; 
                sessionCount++;
            }

            isWorkTime = !isWorkTime;
            progressBar.maxValue = timeLeft;
            progressBar.value = timeLeft;
            timerRunning = true; 
        
            sessionText.text = $"{sessionCount + 1} of 4 sessions";
        }    
    }

    private void UpdateTimeDisplay()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    public void BtnPlayPause_Click()
    {
        timerRunning = !timerRunning;
        if (timerRunning == true)
        {
            playPauseText.text = "Pause";
        } else 
        {
            playPauseText.text = "Start";
        }
    }

    public void BtnReset_Click()
    {
        comboBoxSessionType.value = 0; 
        timerRunning = false;
        playPauseText.text = "Start";
        isWorkTime = true;
        timeLeft = focusTime * minute;
        sessionCount = 0;
        sessionText.text = $"{sessionCount + 1} of 4 sessions";
        UpdateTimeDisplay();
        progressBar.maxValue = timeLeft;
        progressBar.value = timeLeft;
    }

    public void BtnExit_Click()
    {
        StartCoroutine(TransitionQuit());
    }

    public void ComboBoxSessionType_OnSelectedIndexChanged()
    {
        if (manualChange == true)
        {
            switch (comboBoxSessionType.options[comboBoxSessionType.value].text)
            {
                case "Focus":
                    timeLeft = focusTime * minute; 
                    isWorkTime = true;
                    break;
                case "Short Break":
                    timeLeft = shortBreakTime * minute; 
                    isWorkTime = false;
                    break;
                case "Long Break":
                    timeLeft = longBreakTime * minute; 
                    isWorkTime = false;
                    break;
            }
            progressBar.maxValue = timeLeft;
            progressBar.value = timeLeft;
            UpdateTimeDisplay();
        }
        manualChange = true;
    }

    public void MessageBox(string message)
    {
        Time.timeScale = 0f;
        popupPanel.SetActive(true);
        popupText.text = message;
    }

    public void CloseMessageBox()
    {
        Time.timeScale = 1f;
        popupPanel.SetActive(false);
    }

    IEnumerator TransitionQuit()
    {
        anim.SetTrigger("Transition");
        yield return new WaitForSeconds(2f);
        Application.Quit();
    }
}