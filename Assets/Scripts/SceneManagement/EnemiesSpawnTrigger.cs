using UnityEngine;

namespace ProjectRaoni
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class EnemiesSpawnTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject enemySpawner = null;

        private bool isActive = true; 
        
        private void OnTriggerEnter2D (Collider2D other)
        {
            if (this.isActive)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    this.enemySpawner.SetActive(true);
                    this.isActive = false;
                }
            }
        }
    }
}
