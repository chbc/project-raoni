using System.Collections;
using UnityEngine;

namespace ProjectRaoni
{
    public class GameControlPopup : MonoBehaviour
    {
        [SerializeField] private float appearenceTime = 5.0f;
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(this.appearenceTime);
            gameObject.SetActive(false);
        }
    }
}
