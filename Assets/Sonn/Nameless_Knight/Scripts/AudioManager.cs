using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class AudioManager : MonoBehaviour, ISingleton
    {
        public static AudioManager Ins;

        public AudioSource musicSource, sfxSource;
        public List<AudioClip> musicClips, sfxClips;

        private void Awake()
        {
            MakeSingleton();
        }
        public void Play(AudioSource audioSource, AudioClip audioClip)
        {
            if (!audioSource || !audioClip)
            {
                Debug.LogWarning("AudioSource hoặc AudioClip không tồn tại!");
                return;
            }
            audioSource.clip = audioClip;
            audioSource.Play();
            if (audioSource == sfxSource)
            {
                audioSource.PlayOneShot(audioSource.clip);
            }    
        }
        public void Stop(AudioSource audioSource)
        {
            if (!audioSource)
            {
                Debug.LogWarning("AudioSource không tồn tại!");
                return;
            }    
            audioSource.Stop();
        }   
        public void Resume(AudioSource audioSource)
        {
            if (!audioSource)
            {
                return;
            }
            if (!audioSource.isPlaying)
            {
                audioSource.UnPause();
            }
        }    
        public void Pause(AudioSource audioSource)
        {
            if (!audioSource)
            {
                return;
            }
            if (audioSource.isPlaying)
            {
                musicSource.Pause();
            }
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
