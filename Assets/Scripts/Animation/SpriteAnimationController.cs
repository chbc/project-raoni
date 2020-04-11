using DragonBones;
using UnityEngine;

namespace ProjectRaoni
{
    public class SpriteAnimationController : MonoBehaviour
    {
        private DragonBones.Animation animation;

        private Quaternion leftOrientation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        private Quaternion rightOrientation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        private void Start()
        {
            UnityArmatureComponent armature = base.GetComponentInChildren<UnityArmatureComponent>();
            this.animation = armature.animation;
        }

        public void setOrientation(bool left)
        {
            if (left)
                this.transform.rotation = leftOrientation;
            else
                this.transform.transform.rotation = rightOrientation;
        }

        public void Play(string animationName)
        {
            this.animation.FadeIn(animationName);
        }
    }
}