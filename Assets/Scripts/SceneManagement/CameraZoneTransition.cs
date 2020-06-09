using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ProjectRaoni
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CameraZoneTransition : MonoBehaviour
    {
        [SerializeField] private Transform cameraConfiner = null;
        [SerializeField] private Transform nextTransitionTransform = null;
        [SerializeField] private GameObject collision = null;
        [SerializeField] private Transform startSceneCollision = null;
        [SerializeField] private float TRANSITION_SPEED = 0.5f;

        private bool hitTrigger;
        private bool allEnemiesDied;

        private void Start()
        {
            this.hitTrigger = false;
            this.allEnemiesDied = false;
            EnemiesController.Instance.AddEnemiesBeatenListener(OnAllEnemiesDied);
        }

        private void OnAllEnemiesDied(int index)
        {
            if (index != 0)
                return;
            
            EnemiesController.Instance.RemoveEnemiesBeatenListener(OnAllEnemiesDied);
            this.allEnemiesDied = true;

            if (this.hitTrigger)
            {
                StartCoroutine(WaitAndGo());
                this.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        private void OnTriggerEnter2D (Collider2D other)
        {
            this.hitTrigger = true;
            
            if (this.allEnemiesDied && other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(WaitAndGo());
                this.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        private IEnumerator WaitAndGo()
        {
            PlayerCamera.Instance.LimitBack(true);
            
            Vector3 currentCollisionPosition = this.startSceneCollision.position;
            currentCollisionPosition.x = 7.0f;
            this.startSceneCollision.position = currentCollisionPosition;
            
            yield return new WaitForSeconds(1.0f);
            
            Destroy(this.collision);

            Vector3 currentPosition = this.cameraConfiner.position;
            currentCollisionPosition = this.startSceneCollision.position;
            float initialPosition = currentPosition.x;
            float finalPosition = this.nextTransitionTransform.position.x;
            float initialCollisionPosition = this.startSceneCollision.position.x;
            float finalCollisionPosition = 22;
            float t = 0.0f;
            while (t < 1.0f)
            {
                currentPosition.x = Mathf.Lerp(initialPosition, finalPosition, t);
                this.cameraConfiner.position = currentPosition;

                currentCollisionPosition.x = Mathf.Lerp(initialCollisionPosition, finalCollisionPosition, t);
                this.startSceneCollision.position = currentCollisionPosition;                
                
                t += TRANSITION_SPEED * Time.deltaTime;
                
                yield return null;
            }
            
            this.cameraConfiner.position = nextTransitionTransform.position;

            PlayerCamera.Instance.LimitBack(false);
            Destroy(this.gameObject);
        }
    }
}
