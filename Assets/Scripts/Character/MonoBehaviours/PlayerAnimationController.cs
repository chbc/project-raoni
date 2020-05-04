﻿using UnityEngine;
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
        [SerializeField] private string HIT     = "hit";
        [SerializeField] private string DEATH   = "dead";

        [SerializeField] private Material overrideMaterial = null;
        [SerializeField] private float runSpeedTreshold = 4.0f;
        
        public bool IsFacingLeft { get; private set; }

        private string currentAnimationName;

        private readonly Vector3 LEFT_ORIENTATION = new Vector3(-1.0f, 1.0f, 1.0f);
        private readonly Vector3 RIGHT_ORIENTATION = new Vector3(1.0f, 1.0f, 1.0f);
        
        private Renderer[] meshRenderers;

        private void Awake()
        {
            this.currentAnimationName = IDLE;
            this.IsFacingLeft = false;
        }
        
        protected override void Start()
        {
            base.Start();
            if (this.overrideMaterial != null)
            {
                this.meshRenderers = base.GetComponentsInChildren<Renderer>();
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
                        this.currentAnimationName = RUN;
                        this.PlayAnimation(RUN);
                    }
                }
                else if (this.currentAnimationName != WALK)
                {
                    this.currentAnimationName = WALK;
                    this.PlayAnimation(WALK);
                }
            }
            else if (this.currentAnimationName != IDLE)
            {
                float fadeTime = (this.currentAnimationName == ATTACK1) ? 0.1f : -1.0f;
                
                this.currentAnimationName = IDLE;
                this.PlayAnimation(IDLE, -1, fadeTime);
            }
        }

        public void PlayMeleeAttack()
        {
            this.currentAnimationName = ATTACK1;
            this.PlayAnimation(ATTACK1, 1, 0.05f);
        }

        public void PlayRangedAttack()
        {
            this.currentAnimationName = ATTACK2;
            this.PlayAnimation(ATTACK2, 1);
        }

        public void SetRendererEnabled(bool isEnabled)
        {
            
        }

        public void ToggleRendererVisibility()
        {

        }

        private void PlayAnimation(string animationName, int playTimes = -1, float fadeTime = -1.0f)
        {
            base.Play(animationName, playTimes, fadeTime);
            StartCoroutine(this.UpdateMaterial(fadeTime));
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
