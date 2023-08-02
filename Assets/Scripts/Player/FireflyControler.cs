using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NRand;

public class FireflyControler : MonoBehaviour
{
    const float m_appearDuration = 1.0f;

    [SerializeField] float m_moveRadius = 0.5f;
    [SerializeField] float m_moveSpeed = 1.0f;

    Transform m_render;
    float m_startTime;
    float m_timeOffset;

    private void Awake()
    {
        m_render = transform.Find("Render");
    }

    private void OnEnable()
    {
        m_timeOffset = new UniformFloatDistribution(0, 100).Next(new StaticRandomGenerator<MT19937>());
        m_startTime = Time.time;
    }

    private void OnDisable()
    {
        transform.position = new Vector3(m_render.position.x, m_render.position.y, transform.position.z);
        m_render.localPosition = new Vector3(0, m_render.localPosition.y, 0);
    }

    private void Update()
    {
        float t = (Time.time + m_timeOffset) * m_moveSpeed;
        float distMultiplier = t / m_appearDuration;
        if (distMultiplier > 1)
            distMultiplier = 1;

        float x = (Mathf.Cos(t * 3) + Mathf.Sin(t * 2)) / 2 * m_moveRadius;
        float y = (Mathf.Sin(t * 2.5f) + Mathf.Cos(t * 1.5f)) / 2 * m_moveRadius;
        float z = m_render.localPosition.z;

        m_render.localPosition = new Vector3(x, y, z);
    }
}
