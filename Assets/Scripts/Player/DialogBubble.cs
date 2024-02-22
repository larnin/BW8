using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DialogBubble : MonoBehaviour
{
    [SerializeField] float m_textSpeed = 5;
    [SerializeField] float m_delayToNextBubble = 2;
    [SerializeField] float m_maxWidth = 7;
    [SerializeField] float m_extraWide = 12;
    [SerializeField] float m_maxLines = 4;

    SubscriberList m_subscriberList = new SubscriberList();

    DialogObject m_dialog;
    GameObject m_target;

    SpriteRenderer m_background;
    SpriteRenderer m_tail;
    TMP_Text m_textWidget;

    int m_nextText = 0;
    bool m_writing = false;
    float m_timer = 0;
    float m_initialWidth = 0;

    string m_text;
    List<string> m_lines = new List<string>();
    int m_characterIndex = 0;

    bool m_ended = false;

    List<DialogBubble> m_instancedBubbles = new List<DialogBubble>();

    private void Awake()
    {
        m_subscriberList.Add(new Event<StartDialogEvent>.LocalSubscriber(StartDialog, gameObject));
        m_subscriberList.Add(new Event<EndDialogEvent>.LocalSubscriber(EndDialog, gameObject));
        m_subscriberList.Add(new Event<IsDialogEndedEvent>.LocalSubscriber(IsEnded, gameObject));

        m_subscriberList.Subscribe();

        m_background = transform.Find("Background")?.GetComponent<SpriteRenderer>();
        m_tail = transform.Find("Tail")?.GetComponent<SpriteRenderer>();
        m_textWidget = transform.GetComponentInChildren<TMP_Text>();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();

        m_instancedBubbles.Remove(this);
    }

    void StartDialog(StartDialogEvent e)
    {
        m_dialog = e.dialog;
        m_target = e.target;

        if(!m_instancedBubbles.Contains(this))
            m_instancedBubbles.Add(this);

        StartFirstLine();
    }

    void EndDialog(EndDialogEvent e)
    {
        EndBubble();
    }

    void IsEnded(IsDialogEndedEvent e)
    {
        e.ended = m_ended;
    }

    void StartFirstLine()
    {
        m_nextText = 0;
        StartNextLine();
        UpdatePosition();
    }

    void StartNextLine()
    {
        if (m_nextText >= m_dialog.GetNbTexts())
        {
            m_timer = 0;
            EndBubble();
            return;
        }

        m_text = m_dialog.GetText(m_nextText).GetText();

        m_writing = true;
        m_characterIndex = 0;
        UpdateText();
    }

    void UpdateText()
    {
        var transform = m_textWidget.rectTransform;
        var rect = transform.sizeDelta;
        rect.x = m_maxWidth;
        m_initialWidth = m_maxWidth;
        transform.sizeDelta = rect;

        m_textWidget.text = m_text;
        Canvas.ForceUpdateCanvases();
        var lines = m_textWidget.textInfo.lineInfo;
        if(lines.Length > m_maxLines)
        {
            rect.x = m_extraWide;
            m_initialWidth = m_extraWide;
            transform.sizeDelta = rect;
            Canvas.ForceUpdateCanvases();
            lines = m_textWidget.textInfo.lineInfo;
        }

        m_lines.Clear();
        for (int i = 0; i < lines.Length; i++)
        {
            int startIndex = lines[i].firstCharacterIndex;
            int len = lines[i].characterCount;
            if (len == 0)
                continue;
            m_lines.Add(m_textWidget.text.Substring(startIndex, len));
        }

        m_nextText++;
        m_writing = true;

        UpdateDisplayText();

        UpdatePosition();
    }

    void UpdateDisplayText()
    {
        if (m_characterIndex >= m_text.Length)
        {
            m_textWidget.text = m_text;
            m_writing = false;
            m_timer = 0;
            return;
        }

        string startText = m_text.Substring(0, m_characterIndex + 1);
        string endText = m_text.Substring(m_characterIndex + 1);

        m_textWidget.text = startText + "<color=#00000000>" + endText + "</color>";

        m_characterIndex++;
    }

    private void Update()
    {
        m_timer += Time.deltaTime;
        if(m_writing)
        {
            float duration = 1 / m_textSpeed;
            if (m_timer > duration)
            {
                UpdateDisplayText();
                m_timer = 0;
            }
        }
        else
        {
            if (m_timer >= m_delayToNextBubble)
                StartNextLine();
        }
    }

    void UpdatePosition()
    {
        var bounds = m_textWidget.textBounds;
        
        float offsetX = m_initialWidth - bounds.size.x;
        var textPos = m_textWidget.transform.localPosition;
        textPos.x = offsetX / 2;
        m_textWidget.transform.localPosition = textPos;

        m_background.size = new Vector2(bounds.size.x + 0.7f, bounds.size.y + 0.7f);

        m_tail.transform.localPosition = new Vector3(0, -bounds.size.y / 2 - 0.6f, 0);

        var pos = m_target.transform.position;
        pos.y += bounds.size.y / 2 + 2;

        transform.position = pos;
    }

    void EndBubble()
    {
        m_ended = true;
        gameObject.SetActive(false);
    }
}
