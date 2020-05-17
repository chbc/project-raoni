using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectRaoni
{
    public class ZoneDescription : MonoBehaviour
    {
        [SerializeField] private GameObject[] descriptions = null;
        [SerializeField] private float appearenceTime = 5.0f;
        
        private IEnumerator Start()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            GameObject description = null;

            switch (currentScene)
            {
                case "Zone1":
                    description = this.descriptions[0];
                    break;
                case "Zone2":
                    description = this.descriptions[1];
                    break;
                case "BossZone":
                    description = this.descriptions[2];
                    break;
            }

            if (description != null)
            {
                description.SetActive(true);
                yield return new WaitForSeconds(this.appearenceTime);
                description.SetActive(false);
            }
        }
    }
}
