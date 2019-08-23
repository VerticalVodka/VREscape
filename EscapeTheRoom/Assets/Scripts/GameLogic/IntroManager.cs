using UnityEngine;
using UnityEngine.SceneManagement;

namespace VREscape
{
    public class IntroManager : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKey(KeyCode.L))
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
