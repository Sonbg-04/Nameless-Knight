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

        private void Awake()
        {
            MakeSingleton();
        }
        private void Start()
        {
            AudioState();
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
        private void AudioState()
        {
            if (IsComponentNull())
            {
                return;
            }
            var isMusicOn = Pref.GetBool(GamePref.isMusicOn.ToString(), true);
            var isSFXOn = Pref.GetBool(GamePref.isSFXOn.ToString(), true);
            AudioManager.Ins.musicSource.mute = !isMusicOn;
            AudioManager.Ins.sfxSource.mute = !isSFXOn;
            AudioManager.Ins.Play(AudioManager.Ins.musicSource, AudioManager.Ins.musicClips[1]);
        }    
        public void PlayGame()
        {
            if (IsComponentNull())
            {
                return;
            }
            SceneManager.LoadSceneAsync(1);
            gameState = GameState.Playing;
            AudioManager.Ins.Play(AudioManager.Ins.musicSource, AudioManager.Ins.musicClips[0]);
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
