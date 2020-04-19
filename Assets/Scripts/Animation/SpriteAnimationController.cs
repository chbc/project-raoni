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

        public void Play(string animationName, float fadeInTime = -1.0f)
        {
            this.animation.FadeIn(animationName, fadeInTime);
        }
    }
}
