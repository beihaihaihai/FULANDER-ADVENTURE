using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class UIManager : MonoBehaviour
{
    public GameObject pausePanel;
    public TMP_Text TipsLabel;

    [Header("EventListener")]
    public TextEventSO textEventSO;

    private void Update()
    {
        // Toggle pause panel with Escape key
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            TogglePausePanel();
        }
    }

    private void OnEnable()
    {
        textEventSO.TextEvent += OnTextEvent;
    }

    private void OnDisable()
    {
        textEventSO.TextEvent -= OnTextEvent;
    }

    private void OnTextEvent(string text)
    {
        TipsLabel.text = text;
    }

    private void TogglePausePanel()
    {
        Debug.Log("Toggling Pause Panel");
        if (pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pausePanel.SetActive(true); 
            Time.timeScale = 0;
        }
    }
}
