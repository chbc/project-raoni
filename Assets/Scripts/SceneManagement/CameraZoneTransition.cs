using System.Collections;
using UnityEngine;

namespace ProjectRaoni
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CameraZoneTransition : MonoBehaviour
    {
        [SerializeField] private Transform cameraConfiner = null;
        [SerializeField] private Transform nextTransitionTransform = null;
        [SerializeField] private GameObject goMessage = null;
        
        private void OnTriggerEnter2D (Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(WaitAndGo());
            }
        }

        private IEnumerator WaitAndGo()
        {
            for (int i = 0; i < 3; i++)
            {
                this.goMessage.SetActive(true);
                yield return new WaitForSeconds(0.75f);
                
                this.goMessage.SetActive(false);
                yield return new WaitForSeconds(0.25f);
            }
            
            this.cameraConfiner.position = nextTransitionTransform.position;
        }
    }
}
