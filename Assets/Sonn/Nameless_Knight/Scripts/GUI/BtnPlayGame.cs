using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sonn.Nameless_Knight
{
    public class BtnPlayGame : MonoBehaviour, IComponentChecking
    {
        private Button m_btn;
        private void Awake()
        {
            m_btn = GetComponent<Button>();
        }
        private void Start()
        {
            if (IsComponentNull())
            {
                return;
            }
            m_btn.onClick.AddListener(() => GameManager.Ins.PlayGame());
        }
        public bool IsComponentNull()
        {
            bool check = GameManager.Ins == null;
            if (check)
            {
                Debug.LogError("Có component bị null ở " + this.name + "!");
            }
            return check;
        }
        
    }
}
