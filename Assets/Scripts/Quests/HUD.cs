using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

class HUD : MonoBehaviour
{
    [SerializeField] GameObject m_heartPrefab;
    [SerializeField] GameObject m_heartOrigin;
    [SerializeField] float m_heartOffset = 20;

    int m_currentLife = 0;
    int m_currentMaxLife = 0;

    List<HUDHeart> m_hearts = new List<HUDHeart>();

    TMP_Text m_moneyText;

    public static void Open()
    {
        MenuSystem.instance.OpenMenu<HUD>("HUD");
    }

    private void Start()
    {
        m_moneyText = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        UpdateLife();
        UpdateMoney();
    }

    void UpdateLife()
    {
        GetPlayerLifeEvent lifeEvent = new GetPlayerLifeEvent();
        Event<GetPlayerLifeEvent>.Broadcast(lifeEvent);

        if(lifeEvent.maxLife <= 0 && m_currentMaxLife <= 0)
        {
            foreach (var h in m_hearts)
                Destroy(h.gameObject);
            m_hearts.Clear();
            return;
        }

        if(lifeEvent.maxLife <= 0)
            lifeEvent.maxLife = m_currentMaxLife;

        if (lifeEvent.life > lifeEvent.maxLife)
            lifeEvent.life = lifeEvent.maxLife;

        if (m_currentMaxLife != lifeEvent.maxLife)
        {
            int newHealthCount = (lifeEvent.maxLife + 1) / 2;

            for (int i = m_hearts.Count; i < newHealthCount; i++)
            {
                var obj = Instantiate(m_heartPrefab);
                obj.transform.SetParent(m_heartOrigin.transform);
                Vector3 pos = new Vector3(m_heartOffset * i, 0, 0);
                obj.transform.localPosition = pos;
                obj.transform.localScale = Vector3.one;
                m_hearts.Add(obj.GetComponent<HUDHeart>());
            }
            int oldCount = m_hearts.Count;
            for (int i = newHealthCount; i < oldCount; i++)
            {
                int index = m_hearts.Count - 1;
                Destroy(m_hearts[index].gameObject);
                m_hearts.RemoveAt(index);
            }

            m_currentMaxLife = lifeEvent.maxLife;
        }

        m_currentLife = lifeEvent.life;

        for(int i = 0; i < m_hearts.Count; i++)
        {
            int life = (i + 1) * 2;

            HUDHeartState state = HUDHeartState.empty;
            if (m_currentLife >= life)
                state = HUDHeartState.full;
            else if (m_currentLife == life - 1)
                state = HUDHeartState.half;

            m_hearts[i].SetHeartState(state);
        }
    }

    void UpdateMoney()
    {
        GetPlayerMoneyEvent moneyEvent = new GetPlayerMoneyEvent();
        Event<GetPlayerMoneyEvent>.Broadcast(moneyEvent);
        m_moneyText.text = moneyEvent.money.ToString();
    }
}
