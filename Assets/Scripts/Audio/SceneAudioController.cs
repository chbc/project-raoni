using System.Collections;
using Gamekit2D;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectRaoni
{
    public class SceneAudioController : MonoBehaviour
    {
        [SerializeField] private AudioClip _stageMusic = null;
        [SerializeField] private bool _playAtNightsOverRiversAndBridges = false;
        [SerializeField] private bool _resetAudio = false;
        
        private IEnumerator Start()
        {
            if (BackgroundMusicPlayer.HasInstance)
            {
                BackgroundMusicPlayer.Instance.PushClip(_stageMusic, _resetAudio);

                if (_playAtNightsOverRiversAndBridges)
                {
                    yield return new WaitForSeconds(1.0f);
                    BackgroundMusicPlayer.Instance.PlayZone1Intro();
                }
            }
        }
    }
}
