using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class GeneratingTower : MonoBehaviour
    {
        private Animator m_anim;
        private void Awake()
        {
            m_anim = GetComponent<Animator>();
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Knight") && 
                Player.Ins.enemyLists.Count == 0)
            {
                m_anim.SetBool("Powering", true);
            }
        }
    }
}
