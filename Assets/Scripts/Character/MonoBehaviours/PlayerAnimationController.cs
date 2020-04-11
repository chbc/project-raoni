using UnityEngine;

namespace ProjectRaoni
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private SpriteAnimationController animationController;

        [SerializeField] private string IDLE    = "idle";
        [SerializeField] private string RUN     = "run";
        [SerializeField] private string ATTACK1 = "atk1";
        [SerializeField] private string ATTACK2 = "atk2";
        [SerializeField] private string HIT     = "hit";
        [SerializeField] private string DEATH   = "dead";

        private string currentAnimationName;

        private void Awake()
        {
            this.currentAnimationName = IDLE;
        }

        public void UpdateGroundedAnimation(float movementSpeed)
        {
            if (Mathf.Abs(movementSpeed) > 0.01f)
            {
                if (this.currentAnimationName != RUN)
                {
                    this.currentAnimationName = RUN;
                    this.animationController.Play(RUN);
                }
            }
            else if (this.currentAnimationName != IDLE)
            {
                this.currentAnimationName = IDLE;
                this.animationController.Play(IDLE);
            }
        }

        public void SetOrientation(bool left)
        {
            this.animationController.setOrientation(left);
        }
    }
}
