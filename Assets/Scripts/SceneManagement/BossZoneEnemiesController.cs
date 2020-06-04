using System.Collections;
using Gamekit2D;
using UnityEngine;

namespace ProjectRaoni
{
    public class BossZoneEnemiesController : MonoBehaviour
    {
        [SerializeField] private int maxEnemies = 5;
        [SerializeField] private Transform[] spawnTransforms = null;
        [SerializeField] private GameObject enemyPrefab = null;

        private int enemiesRemaining;

        private Coroutine instantiationCoroutine;
        
        private void Start()
        {
            this.enemiesRemaining = maxEnemies;
            
            this.instantiationCoroutine = StartCoroutine(WaitAndInstantiateEnemy());
        }

        private void OnDestroy()
        {
            StopCoroutine(this.instantiationCoroutine);
        }

        private IEnumerator WaitAndInstantiateEnemy()
        {
            yield return new WaitForSeconds(5.0f);
            
            int index = (PlayerCharacter.PlayerInstance.transform.position.x > 0) ? 0 : 1;
            GameObject gameObject = Instantiate(this.enemyPrefab, this.spawnTransforms[index]);

            EnemyBehaviour enemyBehaviour = gameObject.GetComponent<EnemyBehaviour>();
            enemyBehaviour.speed = enemyBehaviour.walkSpeed = enemyBehaviour.runSpeed;
            
            Damageable enemy = gameObject.GetComponent<Damageable>();
            enemy.OnDie.AddListener(OnEnemyDie);
        }

        private void OnEnemyDie(Damager damager, Damageable damageable)
        {
            this.enemiesRemaining--;
            if (this.enemiesRemaining > 0)
                this.instantiationCoroutine = StartCoroutine(WaitAndInstantiateEnemy());
        }
    }
}
