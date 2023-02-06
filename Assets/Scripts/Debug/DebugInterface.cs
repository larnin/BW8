using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum DebugWindowType
{
    Window_Log,
    Window_Quest,
    Window_Inventory,
}

public class DebugInterface : MonoBehaviour
{
    const string saveName = "Debug.save";
    const float minWindowSize = 100;
    const float handleSize = 25;
    const float toolbarHeight = 30;

    static DebugInterface m_instance;

    SavedData m_save = new SavedData();
    float m_lastSaveUpdate = 0;

    class ResizeInfo
    {
        public int id;
        public bool resizeHandleClicked;
        public Vector2 clickedPos;
        public Rect originalRect;

        public ResizeInfo()
        {
            id = -1;

            resizeHandleClicked = false;
            clickedPos = Vector2.zero;
            originalRect = Rect.zero;
        }
    }

    class WindowInfo
    {
        public string name;
        public int id;
        public Rect rect;
        public bool enabled;

        public WindowInfo(int _id, string _name)
        {
            name = _name;
            id = _id;
            rect = new Rect(0, toolbarHeight, 200, 200);
            enabled = false;
        }
    }

    bool m_enabled = false;

    SubscriberList m_subscriberList = new SubscriberList();

    List<WindowInfo> m_windows = new List<WindowInfo>();
    ResizeInfo m_resize = new ResizeInfo();
    List<bool> m_popupOpen = new List<bool>();

    private void Awake()
    {
        if (m_instance != null)
            Destroy(gameObject);
        m_instance = this;
        DontDestroyOnLoad(gameObject);

        m_subscriberList.Add(new Event<IsDebugEnabledEvent>.Subscriber(DebugEnabled));
        m_subscriberList.Subscribe();

        int nbWindow = Enum.GetValues(typeof(DebugWindowType)).Length;
        m_windows.Clear();
        for (int i = 0; i < nbWindow; i++)
            m_windows.Add(new WindowInfo(i, ((DebugWindowType)i).ToString()));
    }

    private void Start()
    {
        Load();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Update()
    {
        if (Keyboard.current.f5Key.wasPressedThisFrame)
            m_enabled = !m_enabled;

        TrySave();
    }

    private void OnGUI()
    {
        if (m_enabled)
            DrawToolbar();

        ResizeWindow();

        foreach(var w in m_windows)
        {
            if (w.enabled)
            {
                w.rect = GUI.Window(w.id, w.rect, DrawWindow, w.name);
            }
        }
    }

    void DebugEnabled(IsDebugEnabledEvent e)
    {
        e.enabled = m_enabled;
    }

    void DrawWindow(int id)
    {
        var w = GetWindow(id);
        if (w == null)
            return;

        GUI.Box(new Rect(w.rect.width - handleSize, w.rect.height - handleSize, handleSize, handleSize), "");

        Event<DrawDebugWindowEvent>.Broadcast(new DrawDebugWindowEvent((DebugWindowType)w.id, w.id, w.rect));

        if(m_resize.id != id)
            GUI.DragWindow();
    }

    WindowInfo GetWindow(int id)
    {
        foreach(var w in m_windows)
        {
            if (w.id == id)
                return w;
        }

        return null;
    }

    void ResizeWindow()
    {
        var mousePos = Mouse.current.position.ReadValue();
        mousePos.y = Screen.height - mousePos.y;

        var w = GetWindow(m_resize.id);

        if(w == null && Mouse.current.leftButton.wasPressedThisFrame && !m_resize.resizeHandleClicked)
        {
            foreach(var window in m_windows)
            {
                var windowHandle = new Rect(window.rect.x + window.rect.width - handleSize, window.rect.y + window.rect.height - handleSize, handleSize, handleSize);

                if(windowHandle.Contains(mousePos))
                {
                    m_resize.resizeHandleClicked = true;
                    m_resize.clickedPos = mousePos;
                    m_resize.originalRect = window.rect;
                    m_resize.id = window.id;
                    break;
                }
            }
        }

        w = GetWindow(m_resize.id);
        if (w != null)
        {
            var windowHandle = new Rect(w.rect.x + w.rect.width - handleSize, w.rect.y + w.rect.height - handleSize, handleSize, handleSize);

            if (m_resize.resizeHandleClicked)
            {
                if (Mouse.current.leftButton.isPressed)
                {
                    w.rect.width = Mathf.Max(m_resize.originalRect.width + (mousePos.x - m_resize.clickedPos.x), minWindowSize);
                    w.rect.height = Mathf.Max(m_resize.originalRect.height + (mousePos.y - m_resize.clickedPos.y), minWindowSize);
                }
                else
                {
                    m_resize.resizeHandleClicked = false;
                    m_resize.id = -1;
                }
            }
            else w.id = -1;
        }
    }

    void DrawToolbar()
    {
        const float labelWidth = 150;
        const float labelspacing = 10;

        GUI.Box(new Rect(0, 0, Screen.width, toolbarHeight), "");

        PopupData[] windowsDatas = new PopupData[Enum.GetValues(typeof(DebugWindowType)).Length];
        for (int i = 0; i < windowsDatas.Length; i++)
            windowsDatas[i] = new PopupData(((DebugWindowType)i).ToString(), m_windows[i].enabled);

        bool selected = GetPopupOpen(popupID_Debug);
        int clicked = DrawPopup(new Rect(labelspacing + (labelspacing + labelWidth) * popupID_Debug, 2, labelWidth, toolbarHeight - 4), ref selected, "Windows", windowsDatas);
        if (selected)
            CloseOthersPopup(popupID_Debug);
        SetPopupOpen(popupID_Debug, selected);
        
        if(clicked >= 0)
            m_windows[clicked].enabled = !m_windows[clicked].enabled;
    }

    class PopupData
    {
        public string name;
        public bool selected;

        public PopupData(string _name, bool _selected = false)
        {
            name = _name;
            selected = _selected;
        }
    }

    const int popupID_Debug = 0;

    //return new selected state
    int DrawPopup(Rect rect, ref bool selected, string label, PopupData[] datas)
    {
        int returnValue = -1;

        const float buttonSpacing = 2;
        const float buttonHeight = 20;

        if (GUI.Button(rect, label))
            selected = !selected;

        if (selected)
        {
            float height = datas.Length * buttonHeight + buttonSpacing + buttonSpacing;
            GUI.Box(new Rect(rect.x, rect.y + rect.height, rect.width, height), "");

            for (int i = 0; i < datas.Length; i++)
            {
                float y = buttonSpacing + i * (buttonSpacing + buttonHeight);
                if (GUI.Button(new Rect(rect.x + buttonSpacing, rect.y + rect.height + y, rect.width - 2 * buttonSpacing, buttonHeight), datas[i].name))
                {
                    selected = false;
                    returnValue = i;
                }

                if(datas[i].selected)
                {
                    GUI.Label(new Rect(rect.x + rect.width - buttonSpacing - buttonHeight, rect.y + rect.height + y, buttonHeight, buttonHeight), "✓");
                }
            }
        }

        return returnValue;
    }

    bool GetPopupOpen(int index)
    {
        if (m_popupOpen.Count <= index)
            return false;
        return m_popupOpen[index];
    }

    void SetPopupOpen(int index, bool value)
    {
        while (m_popupOpen.Count <= index)
            m_popupOpen.Add(false);
        m_popupOpen[index] = value;
    }

    void CloseOthersPopup(int index)
    {
        for (int i = 0; i < m_popupOpen.Count; i++)
            if (i != index)
                m_popupOpen[i] = false;
    }

    void OnLoad()
    {
        foreach (var w in m_windows)
            LoadWindow(w);
    }

    void LoadWindow(WindowInfo w)
    {
        w.rect.position = m_save.GetVector2(w.name + "_Pos", w.rect.position);
        w.rect.size = m_save.GetVector2(w.name + "_Size", w.rect.size);
    }

    void TrySave()
    {
        if (Time.time - m_lastSaveUpdate < 1)
            return;
        m_lastSaveUpdate = Time.time;

        bool needToSave = false;
        foreach(var w in m_windows)
            needToSave |= TrySaveWindow(w);

        if (needToSave)
            Save();
    }

    bool TrySaveWindow(WindowInfo w)
    {
        bool changed = TryUpdateValue(w.name + "_Pos", w.rect.position);
        changed |= TryUpdateValue(w.name + "_Size", w.rect.size); ;

        return changed;
    }

    void Load()
    {
        var bytes = SaveSystem.LoadFile(SaveSystem.GetSavePath(saveName));

        if (bytes != null)
        {
            var data = new SaveReadData();
            data.SetData(bytes, bytes.Length);

            m_save.Load(data);
        }

        OnLoad();
    }

    void Save()
    {
        var data = new SaveWriteData();
        m_save.Save(data);

        SaveSystem.SaveFile(SaveSystem.GetSavePath(saveName), data.GetData());
    }

    bool TryUpdateValue(string name, Vector2 value)
    {
        Vector2 oldValue = m_save.GetVector2(name);
        if((oldValue - value).sqrMagnitude > 0.01f)
        {
            m_save.Set(name, value);
            return true;
        }
        return false;
    }

    bool TryUpdateValue(string name, string value)
    {
        string oldValue = m_save.GetString(name);
        if(oldValue != value)
        {
            m_save.Set(name, value);
            return true;
        }
        return false;
    }

    bool TryUpdateValue(string name, float value)
    {
        float oldValue = m_save.GetFloat(name);
        if(oldValue != value)
        {
            m_save.Set(name, value);
            return true;
        }
        return false;
    }
}