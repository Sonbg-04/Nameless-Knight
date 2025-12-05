using UnityEngine;
using UnityEngine.UI;

namespace Sonn.Nameless_Knight
{
    public class BtnClickSFX : MonoBehaviour, IComponentChecking
    {
        private Button m_btn;

        private void Awake()
        {
            m_btn = GetComponent<Button>();
        }
        public bool IsComponentNull()
        {
            bool check = AudioManager.Ins == null || m_btn == null;
            if (check)
            {
                Debug.LogError("Có component bị null ở " + this.name + "!");
            }
            return check;
        }
        private void Start()
        {
            if (IsComponentNull())
            {
                return;
            }
            m_btn.onClick.AddListener(() => ClickSFXEvent());
        }
        private void ClickSFXEvent()
        {
            if (IsComponentNull())
            {
                return;
            }
            AudioManager.Ins.Play(AudioManager.Ins.sfxSource, AudioManager.Ins.sfxClips[4]);
        }    
    }
}
