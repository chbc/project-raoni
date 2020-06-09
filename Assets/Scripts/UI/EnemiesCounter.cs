using System;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectRaoni
{
    public class EnemiesCounter : MonoBehaviour
    {
        [Serializable]
        public class ActiveEvent : UnityEvent<bool> { }
        
        [Serializable]
        public class EnemiesCountEvent : UnityEvent<string> { }
        
        [SerializeField]
        private EnemiesCountEvent setEnemiesCount;
        [SerializeField]
        private ActiveEvent setActiveEvent;
        
        private void Start()
        {
            EnemiesController.Instance.AddedEnemyDiedListener(OnEnemyDied);
            EnemiesController.Instance.AddEnemiesBeatenListener(OnAllEnemiesDied);
            this.setActiveEvent.Invoke(true);
        }

        public void ShowCounter()
        {
            this.setActiveEvent.Invoke(true);
            EnemiesController enemiesController = EnemiesController.Instance;
            SendCountMessage(enemiesController.currentCount, enemiesController.totalEnemiesCount[enemiesController.currentIndex]);
        }
        
        private void OnEnemyDied(int current, int total)
        {
            SendCountMessage(current, total);
        }

        private void OnAllEnemiesDied(int index)
        {
            this.setActiveEvent.Invoke(false);
        }

        private void SendCountMessage(int current, int total)
        {
            string message = $"{current} / {total}";
            this.setEnemiesCount.Invoke(message);
        }
    }
}
