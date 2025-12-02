using System.Collections;
using System.Collections.Generic;
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
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Knight"))
            {
                m_anim.SetBool("Powering", true);
            }
        }
    }
}
