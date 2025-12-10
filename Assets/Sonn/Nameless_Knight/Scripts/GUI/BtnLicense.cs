using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sonn.Nameless_Knight
{
    public class BtnLicense : MonoBehaviour
    {
        public Image licenseImg;

        private Button m_btn;

        private void Awake()
        {
            m_btn = GetComponent<Button>();
        }

        private void Start()
        {
            licenseImg.gameObject.SetActive(false);

            if (m_btn == null)
            {
                return;
            }

            m_btn.onClick.RemoveAllListeners();
            m_btn.onClick.AddListener(() => ToggleLicense());
        }

        private void ToggleLicense()
        {
            bool isActive = licenseImg.gameObject.activeSelf;
            licenseImg.gameObject.SetActive(!isActive);
        }
    }
}
