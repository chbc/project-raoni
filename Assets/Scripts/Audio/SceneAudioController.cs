using Gamekit2D;
using UnityEngine;

namespace ProjectRaoni
{
    public class SceneAudioController : MonoBehaviour
    {
        [SerializeField] private AudioClip _stageMusic = null;
        
        private void Start()
        {
            BackgroundMusicPlayer.Instance.PushClip(_stageMusic);
        }

        private void OnDestroy()
        {
            if (BackgroundMusicPlayer.HasInstance)
                BackgroundMusicPlayer.Instance.Stop();
        }
    }
}
