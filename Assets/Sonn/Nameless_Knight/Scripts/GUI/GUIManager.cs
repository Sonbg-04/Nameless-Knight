using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sonn.Nameless_Knight
{
    public class GUIManager : MonoBehaviour, IComponentChecking, ISingleton
    {
        public static GUIManager Ins;

        public Image bgEndgame, menuEndgame, currentImg;
        public List<Sprite> iconImgs;
        public TextMeshProUGUI titleWingame, titleLosegame, 
                               titlePausegame, countTxt;
        public Button btnNextlv, btnReplay, btnBack, 
                      btnResume, btnExit, btnPause;

        private void Awake()
        {
            MakeSingleton();
        }
        private void Start()
        {
            NonActiveObject();
        }
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
        private void OnSceneLoaded(Scene scene, LoadSceneMode lcm)
        {
            if (scene.buildIndex == 0)
            {
                if (Ins == this)
                {
                    Destroy(gameObject);
                    Ins = null;
                }
                return;
            }
            UpdateIcon(scene.buildIndex);
        }
        public void MakeSingleton()
        {
            if (Ins == null)
            {
                Ins = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
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
        public void UpdateIcon(int sceneId)
        {
            if (iconImgs.Count == 0)
            {
                return;
            }    
            if (sceneId == 1)
            {
                currentImg.sprite = iconImgs[0];
                btnNextlv.image.sprite = iconImgs[2];
            }
            else if (sceneId == 2)
            {
                currentImg.sprite = iconImgs[1];
                btnNextlv.image.sprite = iconImgs[3];
            }
            else if (sceneId == 3)
            {
                currentImg.sprite = iconImgs[1];
                btnNextlv.image.sprite = iconImgs[4];
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
        public void ActiveFinalGameGUI()
        {
            if (IsComponentNull())
            {
                return;
            }
            bgEndgame.gameObject.SetActive(true);
            menuEndgame.sprite = iconImgs[5];
            menuEndgame.gameObject.SetActive(true);
            currentImg.gameObject.SetActive(false);
            countTxt.gameObject.SetActive(false);
            btnNextlv.gameObject.SetActive(false);
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
            btnReplay.gameObject.SetActive(false);
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
            LoadSceneSafe(SceneManager.GetActiveScene().buildIndex + 1);
            GameManager.Ins.PlayGame();
            Player.Ins.SetOriginalPos();
            NonActiveObject();
        }
        public void ReplayGameEvent()
        {
            if (IsComponentNull())
            {
                return;
            }
            if (!GameManager.Ins.isPlayerDead)
            {
                Player.Ins.SetOriginalPos();
                GameManager.Ins.PlayGame();
                NonActiveObject();
            }    
            LoadSceneSafe(1);
            Player.Ins.SetOriginalPos();
            Player.Ins.SetStateOnLoadScene();
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
            NonActiveObject();
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
