using System;
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

    const string redColorName = "_RedColor";
    const string yellowColorName = "_YellowColor";
    const string greenColorName = "_GreenColor";
    const string cyanColorName = "_CyanColor";
    const string blueColorName = "_BlueColor";
    const string pinkColorName = "_PinkColor";

    [SerializeField] Material m_material = null;

    [SerializeField] Color m_darkColor = Color.black;
    [SerializeField] Color m_lightColor = Color.white;

    [SerializeField] Color m_redColor = Color.red;
    [SerializeField] Color m_yellowColor = Color.yellow;
    [SerializeField] Color m_greenColor = Color.green;
    [SerializeField] Color m_cyanColor = Color.cyan;
    [SerializeField] Color m_blueColor = Color.blue;
    [SerializeField] Color m_pinkColor = Color.magenta;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_subscriberList.Add(new Event<SetScreenColorEvent>.Subscriber(OnSetColor));
        m_subscriberList.Add(new Event<SetCustomScreenColorEvent>.Subscriber(OnSetCustomColor));

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

        m_material.SetColor(redColorName, m_redColor);
        m_material.SetColor(yellowColorName, m_yellowColor);
        m_material.SetColor(greenColorName, m_greenColor);
        m_material.SetColor(cyanColorName, m_cyanColor);
        m_material.SetColor(blueColorName, m_blueColor);
        m_material.SetColor(pinkColorName, m_pinkColor);
    }

    void OnSetColor(SetScreenColorEvent e)
    {
        m_darkColor = e.darkColor;
        m_lightColor = e.lightColor;
    }

    void OnSetCustomColor(SetCustomScreenColorEvent e)
    {
        m_redColor = e.redColor;
        m_yellowColor = e.yellowColor;
        m_greenColor = e.greenColor;
        m_cyanColor = e.cyanColor;
        m_blueColor = e.blueColor;
        m_pinkColor = e.pinkColor;
    }
}