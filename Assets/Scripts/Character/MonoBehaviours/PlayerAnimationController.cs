using UnityEngine;
using System;
using System.Collections;

namespace ProjectRaoni
{
    public class PlayerAnimationController : SpriteAnimationController
    {
        [SerializeField] private string IDLE    = "idle";
        [SerializeField] private string WALK    = "run";
        [SerializeField] private string RUN     = "run";
        [SerializeField] private string ATTACK1 = "atk1";
        [SerializeField] private string ATTACK2 = "atk2";
        [SerializeField] private string ATTACK3 = "Punho_atk_3_chutao";
        [SerializeField] private string DASH    = "Dash_";
        [SerializeField] private string HIT     = "hit";
        [SerializeField] private string DEATH   = "dead";

        [SerializeField] private Material overrideMaterial = null;
        [SerializeField] private float runSpeedTreshold = 4.0f;

        public bool IsLocked => isLocked;
        private bool isLocked;
        
        public bool IsFacingLeft { get; private set; }

        private string currentAnimationName;

        private readonly Vector3 LEFT_ORIENTATION = new Vector3(-1.0f, 1.0f, 1.0f);
        private readonly Vector3 RIGHT_ORIENTATION = new Vector3(1.0f, 1.0f, 1.0f);
        
        private Renderer[] meshRenderers;

        private void Awake()
        {
            this.currentAnimationName = IDLE;
            this.IsFacingLeft = false;
            this.isLocked = false;
        }
        
        protected override void Start()
        {
            base.Start();
            this.meshRenderers = base.GetComponentsInChildren<Renderer>();
            
            if (this.overrideMaterial != null)
            {
                StartCoroutine(this.UpdateMaterial(-1.0f));
            }
        }
        
        public void SetOrientation(bool left, Action<float> meleeOrientationCallback)
        {
            if (left && !this.IsFacingLeft)
            {
                this.transform.localScale = LEFT_ORIENTATION;
                this.IsFacingLeft = true;
                meleeOrientationCallback(-1.0f);
            }
            else if (!left && this.IsFacingLeft)
            {
                this.transform.localScale = RIGHT_ORIENTATION;
                this.IsFacingLeft = false;
                meleeOrientationCallback(1.0f);
            }
        }

        public void UpdateGroundedAnimation(float movementSpeed)
        {
            movementSpeed = Mathf.Abs(movementSpeed); 
            
            if (movementSpeed > 0.01f)
            {
                if (movementSpeed > this.runSpeedTreshold)
                {
                    if (this.currentAnimationName != RUN)
                    {
                        this.PlayAnimation(RUN);
                    }
                }
                else if (this.currentAnimationName != WALK)
                {
                    this.PlayAnimation(WALK);
                }
            }
            else if (this.currentAnimationName != IDLE)
            {
                float fadeTime = (this.currentAnimationName == ATTACK1) ? 0.1f : -1.0f;
                this.PlayAnimation(IDLE, -1, fadeTime);
            }
        }

        public void PlayMeleeAttack()
        {
            this.PlayAnimation(ATTACK1, 1, 0.05f);
            StartCoroutine(LockWaitAndUnlockAnimation());
        }

        public void PlaySecondAttack()
        {
            this.PlayAnimation(ATTACK3, 1, 0.05f);
            StartCoroutine(LockWaitAndUnlockAnimation());
        }

        public void PlayRangedAttack()
        {
            this.PlayAnimation(ATTACK2, 1);
            StartCoroutine(LockWaitAndUnlockAnimation());
        }

        public void PlayDash()
        {
            this.PlayAnimation(DASH, 1, 0.1f);
            StartCoroutine(LockWaitAndUnlockAnimation(0.5f));
        }

        public void PlayHit()
        {
            this.PlayAnimation(HIT, 1);
            StartCoroutine(LockWaitAndUnlockAnimation(false));
        }
        
        public void PlayDie()
        {
            base.Stop();
            this.isLocked = false;
            this.PlayAnimation(DEATH, 1);
            StartCoroutine(LockWaitAndUnlockAnimation(false));
        }

        public void SetRendererEnabled(bool isEnabled)
        {
            for (int i = 0; i < this.meshRenderers.Length; i++)
            {
                this.meshRenderers[i].enabled = true;
            }
        }

        public void ToggleRendererVisibility()
        {
            for (int i = 0; i < this.meshRenderers.Length; i++)
            {
                this.meshRenderers[i].enabled = !this.meshRenderers[i].enabled;
            }
        }

        private IEnumerator LockWaitAndUnlockAnimation(bool playIdleAfterUnlock = true)
        {
            this.isLocked = true;
            yield return null;

            while (base.IsPlaying())
            {
                yield return null;
            }

            this.isLocked = false;
            
            if (playIdleAfterUnlock)
                this.PlayAnimation(IDLE, 1, 0.1f);
        }

        private IEnumerator LockWaitAndUnlockAnimation(float time)
        {
            this.isLocked = true;
            yield return new WaitForSeconds(time);
            this.isLocked = false;
            this.PlayAnimation(IDLE);
        }

        private void PlayAnimation(string animationName, int playTimes = -1, float fadeTime = -1.0f)
        {
            if (!this.isLocked)
            {
                this.currentAnimationName = animationName;
                
                base.Play(animationName, playTimes, fadeTime);
                StartCoroutine(this.UpdateMaterial(fadeTime));
            }
        }
        
        private IEnumerator UpdateMaterial(float fadeTime)
        {
            if (this.overrideMaterial == null)
                yield break;
            
            if (fadeTime > 0)
                yield return new WaitForSeconds(fadeTime);
            
            yield return new WaitForEndOfFrame();
            
            for (int i = 0; i < this.meshRenderers.Length; i++)
            {
                Material currentMaterial = this.meshRenderers[i].material;
                if (currentMaterial.shader.name != this.overrideMaterial.shader.name)
                {
                    Texture texture = currentMaterial.mainTexture;
                    this.meshRenderers[i].material = overrideMaterial;
                    this.meshRenderers[i].material.mainTexture = texture;
                }
            }
        }
    }
}
