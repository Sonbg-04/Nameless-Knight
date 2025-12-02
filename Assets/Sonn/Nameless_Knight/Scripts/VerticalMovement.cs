using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class VerticalMovement : MonoBehaviour
    {
        public float startY, endY, speedMovement;

        private float m_distance;
        private void Start()
        {
            m_distance = Mathf.Abs(endY - startY);
        }
        private void Update()
        {
            var newY = Mathf.PingPong(Time.time * speedMovement, m_distance) + startY;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}
