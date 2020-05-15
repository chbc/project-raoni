using UnityEngine;

namespace ProjectRaoni
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class EnemiesSpawnTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemySpawners = null;

        private bool isActive = true; 
        
        private void OnTriggerEnter2D (Collider2D other)
        {
            if (this.isActive)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    foreach (GameObject item in this.enemySpawners)
                        item.SetActive(true);
                    
                    this.isActive = false;
                }
            }
        }
    }
}
