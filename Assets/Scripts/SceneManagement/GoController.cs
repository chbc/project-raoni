using System;
using System.Collections;
using UnityEngine;

namespace ProjectRaoni
{
    public class GoController : MonoBehaviour
    {
        [SerializeField] private GameObject goMessage = null;
        
        private Coroutine _coroutine;
        
        private void Start()
        {
            EnemiesController.Instance.AddEnemiesBeatenListener(OnEnemiesBeaten);
        }

        private void OnDestroy()
        {
            StopMessage();
        }

        public void StopMessage()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = null;
        }

        private void OnEnemiesBeaten(int index)
        {
            _coroutine = StartCoroutine(ExecuteGoMessage());
        }

        private IEnumerator ExecuteGoMessage()
        {
            while (true)
            {
                this.goMessage.SetActive(true);
                yield return new WaitForSeconds(0.75f);
                
                this.goMessage.SetActive(false);
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}
