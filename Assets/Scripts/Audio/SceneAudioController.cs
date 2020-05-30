using System.Collections;
using Gamekit2D;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectRaoni
{
    public class SceneAudioController : MonoBehaviour
    {
        [SerializeField] private AudioClip _stageMusic = null;
        
        private IEnumerator Start()
        {
            if (BackgroundMusicPlayer.HasInstance)
            {
                BackgroundMusicPlayer.Instance.PushClip(_stageMusic);

                if (SceneManager.GetActiveScene().name == "Zone1")
                {
                    yield return new WaitForSeconds(1.0f);
                    BackgroundMusicPlayer.Instance.PlayZone1Intro();
                }
            }
        }
        
        private void OnDestroy()
        {
            if (BackgroundMusicPlayer.HasInstance)
                BackgroundMusicPlayer.Instance.Stop();
        }
    }
}
