using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class Leverage : MonoBehaviour, IComponentChecking
    {
        public float bounciness;

        public bool IsComponentNull()
        {
            bool check = AudioManager.Ins == null;
            if (check)
            {
                Debug.LogError("Có component bị null ở " + this.name + "!");
            }
            return check;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsComponentNull())
            {
                return;
            }    
            if (collision.gameObject.CompareTag(Const.PLAYER_TAG))
            {
                AudioManager.Ins.Play(AudioManager.Ins.sfxSource, AudioManager.Ins.sfxClips[7]);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounciness, ForceMode2D.Impulse);
            }    
        }
    }
}
