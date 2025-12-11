using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonn.Nameless_Knight
{
    public class CheatCodes : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SceneManager.LoadSceneAsync(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SceneManager.LoadSceneAsync(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SceneManager.LoadSceneAsync(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SceneManager.LoadSceneAsync(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SceneManager.LoadSceneAsync(4);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                if (Player.Ins != null)
                {
                    Player.Ins.SetInvincible(!Player.Ins.IsInvincible);
                }
                else
                {
                    Debug.LogWarning("CheatCodes: Player instance not found; invincibility toggle skipped.");
                }
            }
        }
    }
}
