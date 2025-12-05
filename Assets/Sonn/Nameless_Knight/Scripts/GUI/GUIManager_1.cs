using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sonn.Nameless_Knight
{
    public class GUIManager_1 : MonoBehaviour, IComponentChecking
    {
        public Image bgEndgame, menuEndgame;
        public TextMeshProUGUI titleWingame, titleLosegame, titlePausegame, countTxt;
        public Button btnNextlv, btnReplay, btnBack, btnResume, btnExit, btnPause;

        private void Start()
        {
            NonActiveObject();
        }
        public bool IsComponentNull()
        {
            bool check = AudioManager.Ins == null || GameManager.Ins == null ||
                         Player.Ins == null || FadeTransition.Ins == null;
            if (check)
            {
                Debug.LogError("Có component bị null ở " + this.name + "!");
            }
            return check;
        }
        private void Update()
        {
            if (IsComponentNull())
            {
                return;
            }    
            countTxt.text = GameManager.Ins.Score.ToString();
            if (Input.GetKeyDown(KeyCode.P))
            {
                PauseGameEvent();
            }
        }
        private void NonActiveObject()
        {
            bgEndgame.gameObject.SetActive(false);
            menuEndgame.gameObject.SetActive(false);
            titleWingame.gameObject.SetActive(false);
            titleLosegame.gameObject.SetActive(false);
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
            btnReplay.gameObject.SetActive(true);
            btnBack.gameObject.SetActive(true);
            btnPause.gameObject.SetActive(false);
        }
        private void LoadSceneSafe(int index)
        {
            FadeTransition.Ins.FadeToScene(index);
            Time.timeScale = 1f;
        }
        public void NextLevelEvent()
        {
            if (IsComponentNull())
            {
                return;
            }
            LoadSceneSafe(2);
            GameManager.Ins.PlayGame();
            Player.Ins.SetStateOnLoadScene();
        }
        public void ReplayGameEvent()
        {
            if (IsComponentNull())
            {
                return;
            }
            LoadSceneSafe(SceneManager.GetActiveScene().buildIndex);
            Player.Ins.SetOriginalPos();
            GameManager.Ins.PlayGame();
        }    
        public void PauseGameEvent()
        {
            if (IsComponentNull())
            {
                return;
            }    
            Time.timeScale = 0f;
            GameManager.Ins.PauseGame();
            ActivePausegameGUI();
        }    
        public void ResumeGameEvent()
        {
            if (IsComponentNull())
            {
                return;
            }
            Time.timeScale = 1f;
            GameManager.Ins.ResumeGame();
            NonActiveObject();
        }
        public void ExitGameEvent()
        {
            if (IsComponentNull())
            {
                return;
            }
            LoadSceneSafe(0);
            GameManager.Ins.StartGame();
            Player.Ins.SetStateOnLoadScene();
        }
        private void ActivePausegameGUI()
        {
            if (IsComponentNull())
            {
                return;
            }
            bgEndgame.gameObject.SetActive(true);
            menuEndgame.gameObject.SetActive(true);
            titlePausegame.gameObject.SetActive(true);
            btnReplay.gameObject.SetActive(true);
            btnResume.gameObject.SetActive(true);
            btnExit.gameObject.SetActive(true);
            btnPause.gameObject.SetActive(false);
        }
    }
}
