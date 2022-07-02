using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MenuSystem : MonoBehaviour
{
    [Serializable]
    class MenuData
    {
        public string name;
        public GameObject menu;
    }

    [SerializeField] List<MenuData> m_Menus = new List<MenuData>();
    
    List<MenuData> m_openMenus = new List<MenuData>();

    static MenuSystem m_instance = null;
    public static MenuSystem instance { get { return m_instance; } }

    private void Awake()
    {
        if(m_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public T OpenMenu<T>(string name, bool overrideOpen = false)
    {
        GameObject openMenu = null;
        foreach(var w in m_openMenus)
        {
            if (w.name == name)
            {
                openMenu = w.menu;
                break;
            }
        }

        if(openMenu != null)
        {
            if (overrideOpen)
                Destroy(openMenu);
            else return default(T);
        }

        GameObject prefab = null;
        foreach(var m in m_Menus)
        {
            if(m.name == name)
            {
                prefab = m.menu;
                break;
            }
        }
        if(prefab == null)
        {
            Debug.LogError("No menu named " + name);
            return default(T);
        }

        GetUICameraEvent camEvent = new GetUICameraEvent();
        Event<GetUICameraEvent>.Broadcast(camEvent);

        var menu = Instantiate(prefab, transform);

        var canvas = menu.GetComponent<Canvas>();
        if(canvas != null)
            canvas.worldCamera = camEvent.camera;
        canvas.planeDistance = 0.5f;

        T comp = menu.GetComponentInChildren<T>();
        if (comp == null)
        {
            Debug.LogError("No component " + typeof(T).Name + " in menu " + name);
            Destroy(menu);
            return comp;
        }

        bool found = false;
        foreach(var m in m_openMenus)
        {
            if(m.name == name)
            {
                m.menu = menu;
                found = true;
                break;
            }
        }
        if(!found)
        {
            MenuData data = new MenuData();
            data.name = name;
            data.menu = menu;
            m_openMenus.Add(data);
        }

        return comp;
    }

    public T GetOpenMenu<T>(string name)
    {
        GameObject openMenu = null;
        foreach (var w in m_openMenus)
        {
            if (w.name == name)
            {
                openMenu = w.menu;
                break;
            }
        }

        if (openMenu == null)
            return default(T);

        T comp = openMenu.GetComponentInChildren<T>();
        if (comp == null)
            Debug.LogError("No component " + typeof(T).Name + " in menu " + name);
        return comp;
    }

    public bool IsMenuOpened(string name)
    {
        foreach (var w in m_openMenus)
        {
            if (w.name == name)
                return w.menu != null;
        }

        return false;
    }
}
