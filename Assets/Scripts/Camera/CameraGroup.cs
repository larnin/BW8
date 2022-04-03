using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Camera
{
    public class CameraGroup : MonoBehaviour
    {
        static CameraGroup m_instance = null;
        private void Awake()
        {
            if (m_instance != null)
                Destroy(gameObject);

            m_instance = this;

            DontDestroyOnLoad(gameObject);
        }
    }
}