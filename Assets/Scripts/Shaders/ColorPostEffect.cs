﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class ColorPostEffect : MonoBehaviour
{
    const string darkColorName = "_DarkColor";
    const string lightColorName = "_LightColor";

    [SerializeField] Material m_material = null;

    [SerializeField] Color m_darkColor = Color.black;
    [SerializeField] Color m_lightColor = Color.white;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<SetScreenColorEvent>.Subscriber(OnSetColor));

        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Start()
    {
        if (m_material == null || m_material.shader == null || !m_material.shader.isSupported)
        {
            enabled = false;
            return;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_material);
    }

    private void Update()
    {
        m_material.SetColor(darkColorName, m_darkColor);
        m_material.SetColor(lightColorName, m_lightColor);
    }

    void OnSetColor(SetScreenColorEvent e)
    {
        m_darkColor = e.darkColor;
        m_lightColor = e.lightColor;
    }
}