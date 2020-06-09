using System.Collections;
using UnityEngine;

namespace ProjectRaoni
{
    public delegate void OnAllEnemiesBeaten(int index);
    public delegate void OnEnemiesUpdated(int current, int total); 
    
    public class EnemiesController : MonoBehaviour
    {
        [SerializeField]
        public int[] totalEnemiesCount = null;

        public int currentCount;
        public int currentIndex;
        public static EnemiesController Instance { get; private set; }

        private event OnAllEnemiesBeaten allEnemiesDiedListeners;
        private event OnEnemiesUpdated enemiesUpdatedListeners;
        
        private void Awake()
        {
            Instance = this;

            this.currentCount = 0;
            this.currentIndex = 0;
        }

        private IEnumerator Start()
        {
            yield return null;
            this.enemiesUpdatedListeners?.Invoke(this.currentCount, this.totalEnemiesCount[this.currentIndex]);
        }

        private void OnDestroy()
        {
            this.allEnemiesDiedListeners = null;
            this.enemiesUpdatedListeners = null;
        }

        public void OnEnemyDied()
        {
            this.currentCount++;
            this.enemiesUpdatedListeners?.Invoke(this.currentCount, this.totalEnemiesCount[this.currentIndex]);
            
            if (this.currentCount >= this.totalEnemiesCount[this.currentIndex])
            {
                this.allEnemiesDiedListeners?.Invoke(this.currentIndex);
                if (this.currentIndex < this.totalEnemiesCount.Length)
                    this.currentIndex++;

                this.currentCount = 0;
            } 
        }

        public void AddEnemiesBeatenListener(OnAllEnemiesBeaten listener)
        {
            this.allEnemiesDiedListeners += listener;
        }

        public void RemoveEnemiesBeatenListener(OnAllEnemiesBeaten listener)
        {
            this.allEnemiesDiedListeners -= listener;
        }
        
        public void AddedEnemyDiedListener(OnEnemiesUpdated listener)
        {
            this.enemiesUpdatedListeners += listener;
        }
    }
}
