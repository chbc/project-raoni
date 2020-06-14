using System;
using System.Collections;
using Gamekit2D;
using UnityEngine;
using UnityEngine.Video;

namespace ProjectRaoni
{
    public class CutsceneController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer = null;
        [SerializeField] private TransitionPoint transitionPoint = null;
        [SerializeField] private float timeToWait = 3.0f;

        private IEnumerator Start()
        {
            BackgroundMusicPlayer.Instance.Stop();
            
            yield return null;
            
            if (this.videoPlayer != null)
            {
                this.videoPlayer.Play();

                yield return null;

                while (this.videoPlayer.isPlaying)
                    yield return null;
            }
            else
            {
                yield return new WaitForSeconds(this.timeToWait);
            }

            SceneController.TransitionToScene(this.transitionPoint);
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                StopAllCoroutines();
                SceneController.TransitionToScene(this.transitionPoint);
            }
        }
    }
}
