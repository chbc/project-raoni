using UnityEngine;
using System;

namespace ProjectRaoni
{
    public class PlayerAnimationController : SpriteAnimationController
    {
        [SerializeField] private string IDLE    = "idle";
        [SerializeField] private string RUN     = "run";
        [SerializeField] private string ATTACK1 = "atk1";
        [SerializeField] private string ATTACK2 = "atk2";
        [SerializeField] private string HIT     = "hit";
        [SerializeField] private string DEATH   = "dead";

        public bool IsFacingLeft { get; private set; }

        private string currentAnimationName;

        private readonly Quaternion LEFT_ORIENTATION = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        private readonly Quaternion RIGHT_ORIENTATION = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        private void Awake()
        {
            this.currentAnimationName = IDLE;
            this.IsFacingLeft = false;
        }

        public void SetOrientation(bool left, Action<float> meleeOrientationCallback)
        {
            if (left && !this.IsFacingLeft)
            {
                this.transform.rotation = LEFT_ORIENTATION;
                this.IsFacingLeft = true;
                meleeOrientationCallback(-1.0f);
            }
            else if (!left && this.IsFacingLeft)
            {
                this.transform.transform.rotation = RIGHT_ORIENTATION;
                this.IsFacingLeft = false;
                meleeOrientationCallback(1.0f);
            }
        }

        public void UpdateGroundedAnimation(float movementSpeed)
        {
            if (Mathf.Abs(movementSpeed) > 0.01f)
            {
                if (this.currentAnimationName != RUN)
                {
                    this.currentAnimationName = RUN;
                    base.Play(RUN);
                }
            }
            else if (this.currentAnimationName != IDLE)
            {
                this.currentAnimationName = IDLE;
                base.Play(IDLE);
            }
        }

        public void PlayMeleeAttack()
        {
            this.currentAnimationName = ATTACK1;
            base.Play(ATTACK1);
        }

        public void PlayRangedAttack()
        {
            this.currentAnimationName = ATTACK2;
            base.Play(ATTACK2);
        }

        public void SetRendererEnabled(bool isEnabled)
        {
            
        }

        public void ToggleRendererVisibility()
        {

        }
    }
}
