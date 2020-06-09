using System;
using System.Collections;
using UnityEngine;

namespace ProjectRaoni
{
    public class GoController : MonoBehaviour
    {
        [SerializeField] private GameObject goMessage = null;
        [SerializeField] private EnemiesCounter enemiesCounter = null;
        
        private void Start()
        {
            EnemiesController.Instance.AddEnemiesBeatenListener(OnEnemiesBeaten);
        }

        private void OnEnemiesBeaten(int index)
        {
            StartCoroutine(ExecuteGoMessage());
        }

        private IEnumerator ExecuteGoMessage()
        {
            for (int i = 0; i < 3; i++)
            {
                this.goMessage.SetActive(true);
                yield return new WaitForSeconds(0.75f);
                
                this.goMessage.SetActive(false);
                yield return new WaitForSeconds(0.25f);
            }
            
            this.enemiesCounter.ShowCounter();
        }
    }
}
