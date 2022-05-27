using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using NLocalization;

public class DialogPopup : MonoBehaviour
{
    enum Status
    {
        idle,
        appear,
        waitingForInput,
        writing,
        disappear
    }

    [SerializeField] float m_appearOffset = 50;
    [SerializeField] float m_appearDuration = 0.5f;
    [SerializeField] float m_textSpeed = 5;
    [SerializeField] float m_fasterSpeed = 30;

    DialogObject m_dialog;

    int m_nextLine;
    string m_text;
    string[] m_lines;
    int m_characterIndex;
    float m_currentTextSpeed;
    bool m_started = false;

    Status m_status;
    float m_timer;

    Text m_textWidget;
    Transform m_box;
    Vector3 m_initialPos;

    public static void StartDialog(DialogObject obj)
    {
        var dialog = MenuSystem.instance.InstantiateMenu<DialogPopup>("Dialog");
        if (dialog == null)
            return;

        dialog.Set(obj);
    }

    void Set(DialogObject obj)
    {
        if (m_status != Status.idle)
            return;

        m_dialog = obj;
        if (m_started)
            StartDialog();
    }
    private void Awake()
    {
        m_textWidget = GetComponentInChildren<Text>();
        m_box = transform.Find("Bubble");
        if(m_box == null)
        {
            Debug.LogError("Can't find the Bubble child");
            gameObject.SetActive(false);
            return;
        }

        m_initialPos = m_box.position;
        m_status = Status.idle;
    }

    private void Start()
    {
        m_started = true;
        if (m_dialog != null)
            StartDialog();
    }

    private void Update()
    {
        if (!m_started)
            return;

        UpdatePosition();
        UpdateStatus();
    }

    void StartDialog()
    {
        m_status = Status.appear;
        m_timer = 0;
    }

    void StartFirstLine()
    {
        m_status = Status.writing;
        m_nextLine = 0;

        StartNextLine();
    }

    void StartNextLine()
    {
        if(m_nextLine > m_dialog.GetNbTexts())
        {
            m_status = Status.disappear;
            m_timer = 0;
            return;
        }

        m_status = Status.writing;

        m_text = m_dialog.GetText(m_nextLine).GetText();
        m_characterIndex = 0;
        m_currentTextSpeed = m_textSpeed;

        m_nextLine++;
        UpdateText();
    }

    void UpdateText()
    {
        m_textWidget.text = m_text;
        Canvas.ForceUpdateCanvases();
        var lines = m_textWidget.cachedTextGenerator.lines;
        m_lines = new string[lines.Count];
        for(int i = 0; i < lines.Count; i++)
        {
            int startIndex = lines[i].startCharIdx;
            int endIndex = i == lines.Count - 1 ? m_textWidget.text.Length : lines[i + 1].startCharIdx;
            int len = endIndex - startIndex;
            m_lines[i] = m_textWidget.text.Substring(startIndex, len);
        }
        UpdateDisplayText();
    }

    void UpdateDisplayText()
    {
        if (m_characterIndex >= m_text.Length)
        {
            m_textWidget.text = m_text;
            return;
        }

        string startText = m_text.Substring(0, m_characterIndex + 1);
        string endText = m_text.Substring(m_characterIndex + 1);

        m_textWidget.text = startText + "<color=#00000000>" + endText + "</color>";

        m_characterIndex++;
    }

    void UpdateInputs()
    {
        //todo
    }

    void UpdatePosition()
    {
        switch (m_status)
        {
            case Status.idle:
                {
                    m_box.position = GetHiddenPosition();
                    break;
                }
            case Status.appear:
                {
                    float percent = m_timer / m_appearDuration;
                    m_box.position = GetHiddenPosition() * percent + m_initialPos * (1 - percent);
                    break;
                }
            case Status.waitingForInput:
            case Status.writing:
                {
                    m_box.position = m_initialPos;
                    break;
                }
            case Status.disappear:
                {
                    float percent = m_timer / m_appearDuration;
                    m_box.position = GetHiddenPosition() * (1 - percent) + m_initialPos * percent;
                    break;
                }
            default:
                break;
        }
    }

    void UpdateStatus()
    {
        m_timer += Time.deltaTime;
        switch (m_status)
        {
            case Status.idle:
                break;
            case Status.appear:
                {
                    if (m_timer >= m_appearDuration)
                        StartFirstLine();
                    break;
                }
            case Status.waitingForInput:
                {
                    UpdateInputs();
                    break;
                }
            case Status.writing:
                {
                    UpdateInputs();
                    float duration = 1 / m_currentTextSpeed;
                    if(m_timer > duration)
                        UpdateDisplayText();
                    break;
                }
            case Status.disappear:
                {
                    if (m_timer >= m_appearDuration)
                        Destroy(gameObject);
                    break;
                }
            default:
                break;
        }
    }

    Vector3 GetHiddenPosition()
    {
        return m_initialPos + new Vector3(0, -m_appearOffset, 0);
    }
}

