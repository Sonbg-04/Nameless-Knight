using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sonn.Nameless_Knight
{
    public class ToggleButton : MonoBehaviour
    {
        public Sprite on, off;

        protected bool m_isOn;

        private Button m_btn;

        private void Awake()
        {
            m_btn = GetComponent<Button>();
        }
        protected virtual void Start()
        {
            if (m_btn == null)
            {
                return;
            }
            m_btn.onClick.AddListener(() => BtnClickEvent());
        }
        private void BtnClickEvent()
        {
            ClickEvent();
            UpdateSprite();
        }
        protected void UpdateSprite()
        {
            Image img = m_btn.GetComponent<Image>();
            if (img == null)
            {
                return;
            }
            img.sprite = m_isOn ? on : off;
        }
        public virtual void ClickEvent() {}
    }
}
