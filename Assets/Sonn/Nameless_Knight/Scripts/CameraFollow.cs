using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform camFollow;
        private void Start()
        {
            camFollow = Camera.main.transform;
        }
        private void Update()
        {
            transform.position = new Vector3(camFollow.position.x, camFollow.position.y, 0);
        }
    }
}
