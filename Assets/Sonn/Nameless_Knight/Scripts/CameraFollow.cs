using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.Nameless_Knight
{
    public class CameraFollow : MonoBehaviour
    {
        private Transform m_camFollow;

        private void Awake()
        {
            var camObj = GameObject.FindGameObjectWithTag(Const.VIRTUAL_CAMERA_TAG);
            if (camObj == null)
            {
                Debug.LogWarning("Không tìm thấy GameObject nào có tag là " + Const.VIRTUAL_CAMERA_TAG + "!");
                return;
            }    
            m_camFollow = camObj.transform;
        }
        private void LateUpdate()
        {
            if (m_camFollow == null)
            {
                return;
            }    
            transform.position = new Vector3(m_camFollow.position.x, m_camFollow.position.y, 0);
        }
    }
}
