using System.Collections;
using Gamekit2D;
using UnityEngine;

namespace ProjectRaoni
{
    public class EndGameListener : MonoBehaviour
    {
        [SerializeField] private TransitionPoint transitionPoint = null;
        [SerializeField] private float waitTimeToTransition = 3.0f;
        private void Start()
        {
            EnemiesController.Instance.AddEnemiesBeatenListener(OnAllEnemiesDied);
        }

        private void OnAllEnemiesDied(int index)
        {
            BackgroundMusicPlayer.Instance.Mute(this.waitTimeToTransition);
            StartCoroutine(WaitAndTransition());
        }

        private IEnumerator WaitAndTransition()
        {
            yield return new WaitForSeconds(waitTimeToTransition);
            
            SceneController.TransitionToScene(this.transitionPoint);
        }
    }
}
