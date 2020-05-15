using UnityEngine;

namespace ProjectRaoni
{
    public delegate void OnAllEnemiesBeaten(); 
    
    public class EnemiesController : MonoBehaviour
    {
        [SerializeField]
        private int enemiesCount = 10;
        public static EnemiesController Instance { get; private set; }

        private event OnAllEnemiesBeaten enemiesBeatenListeners;
        
        private void Awake()
        {
            Instance = this;
        }

        public void DecrementEnemies()
        {
            this.enemiesCount--;

            if (this.enemiesCount <= 0)
            {
                this.enemiesBeatenListeners?.Invoke();
            }
        }

        public void AddEnemiesBeatenListener(OnAllEnemiesBeaten listener)
        {
            this.enemiesBeatenListeners += listener;
        }

        public void RemoveEnemiesBeatenListener(OnAllEnemiesBeaten listener)
        {
            this.enemiesBeatenListeners -= listener;
        }
    }
}
