using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum HUDHeartState
{
    empty,
    half,
    full,
}

public class HUDHeart : MonoBehaviour
{
    [SerializeField] Sprite m_emptyHeart;
    [SerializeField] Sprite m_halfHeart;
    [SerializeField] Sprite m_fullHeart;

    HUDHeartState m_state = HUDHeartState.empty;
    Image m_image;

    private void Awake()
    {
        m_image = GetComponentInChildren<Image>();

        UpdateHeart();
    }

    public void SetHeartState(HUDHeartState state)
    {
        if (m_state == state)
            return;

        m_state = state;
        UpdateHeart();
    }

    public HUDHeartState GetHeartState()
    {
        return m_state;
    }

    void UpdateHeart()
    {
        if (m_image == null)
            return;

        switch(m_state)
        {
            case HUDHeartState.empty:
                m_image.sprite = m_emptyHeart;
                break;
            case HUDHeartState.half:
                m_image.sprite = m_halfHeart;
                break;
            case HUDHeartState.full:
                m_image.sprite = m_fullHeart;
                break;
            default:
                m_image.sprite = null;
                break;
        }
    }
}
