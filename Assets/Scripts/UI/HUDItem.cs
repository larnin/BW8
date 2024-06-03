using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDItem : MonoBehaviour
{
    [SerializeField] Image m_texture;
    [SerializeField] TMP_Text m_text;
    [SerializeField] bool m_displayTextWhenUnique = true;

    ItemType m_item;
    int m_count;

    private void Awake()
    {
        m_texture.sprite = null;
        m_text.text = "";
    }

    public void Set(ItemType item, int count)
    {
        m_item = item;
        m_count = count;

        UpdateRender();
    }

    public ItemType GetItem()
    {
        return m_item;
    }

    public int Count()
    {
        return m_count;
    }

    void UpdateRender()
    {
        m_texture.sprite = World.lootType.GetSprite(m_item);
        if (!m_displayTextWhenUnique || m_count > 1)
            m_text.text = m_count.ToString();
        else m_text.text = "";
    }
}
