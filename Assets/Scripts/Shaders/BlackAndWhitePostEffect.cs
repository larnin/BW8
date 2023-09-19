using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class BlackAndWhitePostEffect : MonoBehaviour
{
    const string darkLevelName = "_DarkLevel";
    const string lightLevelName = "_LightLevel";
    const string offsetName = "_offset";

    [SerializeField] Material m_material = null;

    [SerializeField] [Range(0, 1)] float m_darkLevel = 0;
    [SerializeField] [Range(0, 1)] float m_lightLevel = 1;

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
        m_material.SetFloat(darkLevelName, m_darkLevel);
        m_material.SetFloat(lightLevelName, m_lightLevel);

        m_material.SetVector(offsetName, transform.position * 16); // because 16 pixel / blocs on low res camera
    }
}