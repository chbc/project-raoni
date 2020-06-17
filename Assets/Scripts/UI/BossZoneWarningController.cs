using System.Collections;
using Gamekit2D;
using UnityEngine;

namespace ProjectRaoni
{
    public class BossZoneWarningController : MonoBehaviour
    {
        [SerializeField] private GameObject message;
        [SerializeField] private int duration;
        [SerializeField] private AudioClip _alarmAudio;
        [SerializeField] private AudioClip _backgroundMusic;
        
        private IEnumerator Start()
        {
            if (BackgroundMusicPlayer.HasInstance)
            {
                yield return null;
                BackgroundMusicPlayer.Instance.PushClip(_alarmAudio, true);
                yield return ExecuteGoMessage();
                BackgroundMusicPlayer.Instance.PushClip(_backgroundMusic, true);
            }
        }
        
        private IEnumerator ExecuteGoMessage()
        {
            for (int i = 0; i < this.duration; i++)
            {
                this.message.SetActive(true);
                yield return new WaitForSeconds(0.75f);
                
                this.message.SetActive(false);
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}
