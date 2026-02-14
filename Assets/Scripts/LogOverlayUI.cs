using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class LogOverlayUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text logText;
    [SerializeField] private GameObject panelRoot;

    [Header("Settings")]
    [SerializeField] private int maxLines = 30;
    [SerializeField] private bool includeStackTraceForErrors = false;
    [SerializeField] private KeyCode toggleKey = KeyCode.F3;

    private readonly Queue<string> lines = new();
    private bool isVisible = true;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLogMessage;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLogMessage;
    }

    private void Start()
    {
        RefreshText();
        ApplyVisibility();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isVisible = !isVisible;
            ApplyVisibility();
        }
    }

    private void HandleLogMessage(string condition, string stackTrace, LogType type)
    {
        string prefix = type switch
        {
            LogType.Warning => "[W]",
            LogType.Error => "[E]",
            LogType.Assert => "[A]",
            LogType.Exception => "[X]",
            _ => "[I]"
        };

        AddLine($"{prefix} {condition}");

        if (includeStackTraceForErrors &&
            (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) &&
            !string.IsNullOrWhiteSpace(stackTrace))
        {
            AddLine(stackTrace);
        }
    }

    private void AddLine(string line)
    {
        lines.Enqueue(line);

        while (lines.Count > Mathf.Max(1, maxLines))
        {
            lines.Dequeue();
        }

        RefreshText();
    }

    private void RefreshText()
    {
        if (logText == null) return;

        StringBuilder sb = new();
        foreach (string line in lines)
        {
            sb.AppendLine(line);
        }

        logText.text = sb.ToString();
    }

    private void ApplyVisibility()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(isVisible);
        }
        else if (logText != null)
        {
            logText.gameObject.SetActive(isVisible);
        }
    }
}
