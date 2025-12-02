using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sonn.Nameless_Knight
{
    public class GUIManager_1 : MonoBehaviour, IComponentChecking
    {
        public Image bgEndgame, menuEndgame;
        public TextMeshProUGUI titleWingame, titleLosegame, countTxt, titlePausegame;
        public Button btnNextlv, btnReplay, btnBack, btnResume, btnExit, btnPause;

        private void Start()
        {
            NonActiveObject();
        }
        public bool IsComponentNull()
        {
            bool check = AudioManager.Ins == null;
            if (check)
            {
                Debug.LogError("Có component bị null ở " + this.name + "!");
            }
            return check;
        }
        private void NonActiveObject()
        {
            bgEndgame.gameObject.SetActive(false);
            menuEndgame.gameObject.SetActive(false);
            titleWingame.gameObject.SetActive(false);
            titleLosegame.gameObject.SetActive(false);
            countTxt.gameObject.SetActive(false);
            btnNextlv.gameObject.SetActive(false);
            btnReplay.gameObject.SetActive(false);
            btnBack.gameObject.SetActive(false);
            titlePausegame.gameObject.SetActive(false);
            btnResume.gameObject.SetActive(false);
            btnExit.gameObject.SetActive(false);
            btnPause.gameObject.SetActive(true);
        }
        public void ActiveWingameGUI()
        {
            if (IsComponentNull())
            {
                return;
            }
            bgEndgame.gameObject.SetActive(true);
            menuEndgame.gameObject.SetActive(true);
            titleWingame.gameObject.SetActive(true);
            countTxt.gameObject.SetActive(true);
            btnNextlv.gameObject.SetActive(true);
            btnReplay.gameObject.SetActive(true);
            btnBack.gameObject.SetActive(true);
            btnPause.gameObject.SetActive(false);
        }
        public void ActiveLosegameGUI()
        {
            if (IsComponentNull())
            {
                return;
            }
            bgEndgame.gameObject.SetActive(true);
            menuEndgame.gameObject.SetActive(true);
            titleLosegame.gameObject.SetActive(true);
            countTxt.gameObject.SetActive(true);
            btnReplay.gameObject.SetActive(true);
            btnBack.gameObject.SetActive(true);
            btnPause.gameObject.SetActive(false);
        }
    }
}
