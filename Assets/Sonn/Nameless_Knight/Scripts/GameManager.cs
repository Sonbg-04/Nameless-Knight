using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonn.Nameless_Knight
{
    public class GameManager : MonoBehaviour, ISingleton, IComponentChecking
    {
        public static GameManager Ins;

        public GameState gameState;
        public bool isPlayerDead;

        private int m_score;

        public int Score { get => m_score; set => m_score = value; }

        private void Awake()
        {
            MakeSingleton();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void Start()
        {
            AudioState();
            StartGame();
        }
        public bool IsComponentNull()
        {
            bool check = AudioManager.Ins == null || FadeTransition.Ins == null;
            if (check)
            {
                Debug.LogError("Có component bị null ở " + this.name + "!");
            }
            return check;
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (IsComponentNull())
            {
                return;
            }

            if (scene.buildIndex >= 2)
            {
                AudioManager.Ins.Play(AudioManager.Ins.musicSource, AudioManager.Ins.musicClips[2]);
            }
            else
            {
                AudioManager.Ins.Play(AudioManager.Ins.musicSource, AudioManager.Ins.musicClips[0]);
            }
        }

        private void AudioState()
        {
            if (IsComponentNull())
            {
                return;
            }
            var isMusicOn = Pref.GetBool(GamePref.isMusicOn.ToString(), true);
            AudioManager.Ins.musicSource.mute = !isMusicOn;
            
            var isSFXOn = Pref.GetBool(GamePref.isSFXOn.ToString(), true);
            AudioManager.Ins.sfxSource.mute = !isSFXOn;
        }    
        public void StartGame()
        {
            if (IsComponentNull())
            {
                return;
            }
            gameState = GameState.Start;
            AudioManager.Ins.Play(AudioManager.Ins.musicSource, AudioManager.Ins.musicClips[1]);
        }
        public void PlayGame()
        {
            if (IsComponentNull())
            {
                return;
            }
            FadeTransition.Ins.FadeToScene(1);
            Time.timeScale = 1f;
            gameState = GameState.Playing;
        
        }
        public void PauseGame()
        {
            if (IsComponentNull())
            {
                return;
            }
            gameState = GameState.Pausing;
            AudioManager.Ins.Pause(AudioManager.Ins.musicSource);
        }
        public void ResumeGame()
        {
            if (IsComponentNull())
            {
                return;
            }
            gameState = GameState.Playing;
            AudioManager.Ins.Resume(AudioManager.Ins.musicSource);
        }
        public void GameWin()
        {
            if (IsComponentNull())
            {
                return;
            }
            gameState = GameState.GameWin;
            AudioManager.Ins.Play(AudioManager.Ins.sfxSource, AudioManager.Ins.sfxClips[2]);
        }
        public void GameOver()
        {
            if (IsComponentNull())
            {
                return;
            }
            gameState = GameState.GameOver;
            AudioManager.Ins.Play(AudioManager.Ins.sfxSource, AudioManager.Ins.sfxClips[5]);
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
    }
}
