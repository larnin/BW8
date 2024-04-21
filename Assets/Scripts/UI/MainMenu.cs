using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    Canvas m_canvas;

    private void Awake()
    {
        m_canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        if (m_canvas.worldCamera == null)
        {
            GetUICameraEvent camEvent = new GetUICameraEvent();
            Event<GetUICameraEvent>.Broadcast(camEvent);
            m_canvas.worldCamera = camEvent.camera; 
            m_canvas.planeDistance = 0.5f;
        }
    }
}
