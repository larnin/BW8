using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using NLocalization;
using TMPro;
using UnityEngine.EventSystems;

public class DialogPopup : MonoBehaviour, ISubmitHandler, IEventSystemHandler
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
    [SerializeField] float m_arrowBlinkSpeed = 2;

    DialogObject m_dialog;

    int m_nextLine;
    string m_text;
    string[] m_lines;
    int m_characterIndex;
    float m_currentTextSpeed;
    bool m_started = false;

    Status m_status = Status.idle;
    float m_timer = 0;

    TMP_Text m_textWidget;
    Transform m_box;
    Transform m_arrow;
    Vector3 m_initialPos;

    public static void StartDialog(DialogObject obj)
    {
        var dialog = MenuSystem.instance.OpenMenu<DialogPopup>("Dialog");
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
        m_textWidget = GetComponentInChildren<TMP_Text>();
        m_box = transform.Find("Bubble");
        if(m_box == null)
        {
            Debug.LogError("Can't find the Bubble child");
            gameObject.SetActive(false);
            return;
        }

        m_initialPos = m_box.localPosition;
        m_status = Status.idle;

        m_arrow = m_box.Find("Arrow");
    }

    private void Start()
    {
        UpdatePosition();

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
        UpdateArrow();
    }

    void StartDialog()
    {
        m_status = Status.appear;
        m_timer = 0;
        m_textWidget.text = "";
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
        var lines = m_textWidget.textInfo.lineInfo;
        m_lines = new string[lines.Length];
        for(int i = 0; i < lines.Length; i++)
        {
            int startIndex = lines[i].firstCharacterIndex;
            int endIndex = i == lines.Length - 1 ? m_textWidget.text.Length : lines[i + 1].firstCharacterIndex;
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
            m_status = Status.waitingForInput;
            return;
        }

        string startText = m_text.Substring(0, m_characterIndex + 1);
        string endText = m_text.Substring(m_characterIndex + 1);

        m_textWidget.text = startText + "<color=#00000000>" + endText + "</color>";

        m_characterIndex++;
    }

    void UpdateInputs()
    {
        
    }

    void UpdatePosition()
    {
        switch (m_status)
        {
            case Status.idle:
                {
                    m_box.localPosition = GetHiddenPosition();
                    break;
                }
            case Status.appear:
                {
                    float percent = m_timer / m_appearDuration;
                    m_box.localPosition = GetHiddenPosition() * (1 - percent) + m_initialPos * percent;
                    break;
                }
            case Status.waitingForInput:
            case Status.writing:
                {
                    m_box.localPosition = m_initialPos;
                    break;
                }
            case Status.disappear:
                {
                    float percent = m_timer / m_appearDuration;
                    m_box.localPosition = GetHiddenPosition() * percent + m_initialPos * (1 - percent);
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
                    if (m_timer > duration)
                    {
                        UpdateDisplayText();
                        m_timer = 0;
                    }
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

    void UpdateArrow()
    {
        if (m_arrow == null)
            return;

        if(m_status != Status.waitingForInput)
        {
            m_arrow.gameObject.SetActive(false);
            return;
        }

        m_arrow.gameObject.SetActive(((int)(Time.time * m_arrowBlinkSpeed * 2)) % 2 == 0);
    }

    Vector3 GetHiddenPosition()
    {
        return m_initialPos + new Vector3(0, -m_appearOffset, 0);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Debug.Log("Prout !");
    }
}

