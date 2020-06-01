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
                yield return new WaitForSeconds(3.0f);
            }

            SceneController.TransitionToScene(this.transitionPoint);
        }
    }
}
