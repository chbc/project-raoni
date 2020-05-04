using DragonBones;
using UnityEngine;

namespace ProjectRaoni
{
    public class SpriteAnimationController : MonoBehaviour
    {
        private DragonBones.Animation animation;

        protected virtual void Start()
        {
            UnityArmatureComponent armature = base.GetComponentInChildren<UnityArmatureComponent>();
            this.animation = armature.animation;
        }

        public void Play(string animationName, int playTimes, float fadeTime = -1.0f)
        {
            this.animation.FadeIn(animationName, fadeTime, playTimes);
        }
    }
}
