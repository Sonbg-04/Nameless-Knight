using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class ToggleBtnMusic : ToggleButton, IComponentChecking
    {
        public bool IsComponentNull()
        {
            bool check = AudioManager.Ins == null;
            if (check)
            {
                Debug.LogError("Có component bị null ở " + this.name + "!");
            }    
            return check;
        }
        protected override void Start()
        {
            base.Start();
            m_isOn = Pref.GetBool(GamePref.isMusicOn.ToString(), true);
            UpdateSprite();
        }
        public override void ClickEvent()
        {
            if (IsComponentNull())
            {
                return;
            }
            m_isOn = !m_isOn;
            Pref.SetBool(GamePref.isMusicOn.ToString(), m_isOn);
            AudioManager.Ins.musicSource.mute = !m_isOn;
        }
    }
}
