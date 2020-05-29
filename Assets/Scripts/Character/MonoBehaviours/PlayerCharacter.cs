using System.Collections;
using System.Collections.Generic;
using BTAI;
using UnityEngine;
using UnityEngine.Tilemaps;
using ProjectRaoni;
using UnityEngine.SceneManagement;

namespace Gamekit2D
{
    [RequireComponent(typeof(CharacterController2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerCharacter : MonoBehaviour
    {
        static protected PlayerCharacter s_PlayerInstance;
        static public PlayerCharacter PlayerInstance { get { return s_PlayerInstance; } }

        public InventoryController inventoryController
        {
            get { return m_InventoryController; }
        }

        public Damageable damageable;
        public Damager meleeDamager;
        public Transform facingLeftBulletSpawnPoint;
        public Transform facingRightBulletSpawnPoint;
        public BulletPool bulletPool;
        public Transform cameraFollowTarget;

        public float groundAcceleration = 100f;
        public float groundDeceleration = 100f;
        public float maxRunSpeed = 8.0f;
        [Range(0f, 1f)] public float pushingSpeedProportion;

        [Range(0f, 1f)] public float airborneAccelProportion;
        [Range(0f, 1f)] public float airborneDecelProportion;
        public float gravity = 50f;
        public float jumpSpeed = 20f;
        public float jumpAbortSpeedReduction = 100f;

        [Range(k_MinHurtJumpAngle, k_MaxHurtJumpAngle)] public float hurtJumpAngle = 45f;
        public float hurtJumpSpeed = 5f;
        public float flickeringDuration = 0.1f;

        public float meleeAttackDashSpeed = 5f;
        public bool dashWhileAirborne = false;

        public RandomAudioPlayer hitAudioPlayer;
        public RandomAudioPlayer hurtAudioPlayer;
        public RandomAudioPlayer meleeAttackAudioPlayer;
        public RandomAudioPlayer rangedAttackAudioPlayer;
        public RandomAudioPlayer dashAudioPlayer;

        public float shotsPerSecond = 1f;
        public float bulletSpeed = 5f;
        public float holdingGunTimeoutDuration = 10f;
        public bool rightBulletSpawnPointAnimated = true;

        public float cameraHorizontalFacingOffset;
        public float cameraHorizontalSpeedOffset;
        public float cameraVerticalInputOffset;
        public float maxHorizontalDeltaDampTime;
        public float maxVerticalDeltaDampTime;
        public float verticalCameraOffsetDelay;

        public bool spriteOriginallyFacesLeft;

        protected CharacterController2D m_CharacterController2D;
        protected Animator m_Animator;
        protected CapsuleCollider2D m_Capsule;
        protected Transform m_Transform;
        protected Vector2 m_MoveVector;
        protected List<Pushable> m_CurrentPushables = new List<Pushable>(4);
        protected Pushable m_CurrentPushable;
        protected float m_TanHurtJumpAngle;
        protected WaitForSeconds m_FlickeringWait;
        protected Coroutine m_FlickerCoroutine;
        protected Transform m_CurrentBulletSpawnPoint;
        protected float m_ShotSpawnGap;
        protected TileBase m_CurrentSurface;
        protected float m_CamFollowHorizontalSpeed;
        protected float m_CamFollowVerticalSpeed;
        protected float m_VerticalCameraOffsetTimer;
        protected InventoryController m_InventoryController;

        protected Checkpoint m_LastCheckpoint = null;
        protected Vector2 m_StartingPosition = Vector2.zero;
        protected bool m_StartingFacingLeft = false;

        protected bool m_InPause = false;

        protected readonly int m_HashHorizontalSpeedPara = Animator.StringToHash("HorizontalSpeed");
        protected readonly int m_HashVerticalSpeedPara = Animator.StringToHash("VerticalSpeed");
        protected readonly int m_HashGroundedPara = Animator.StringToHash("Grounded");
        protected readonly int m_HashCrouchingPara = Animator.StringToHash("Crouching");
        protected readonly int m_HashPushingPara = Animator.StringToHash("Pushing");
        protected readonly int m_HashTimeoutPara = Animator.StringToHash("Timeout");
        protected readonly int m_HashRespawnPara = Animator.StringToHash("Respawn");
        protected readonly int m_HashDeadPara = Animator.StringToHash("Dead");
        protected readonly int m_HashHurtPara = Animator.StringToHash("Hurt");
        protected readonly int m_HashForcedRespawnPara = Animator.StringToHash("ForcedRespawn");
        protected readonly int m_HashMeleeAttackPara = Animator.StringToHash("MeleeAttack");
        protected readonly int m_HashHoldingGunPara = Animator.StringToHash("HoldingGun");

        protected const float k_MinHurtJumpAngle = 0.001f;
        protected const float k_MaxHurtJumpAngle = 89.999f;
        protected const float k_GroundedStickingVelocityMultiplier = 3f;    // This is to help the character stick to vertically moving platforms.

        //used in non alloc version of physic function
        protected ContactPoint2D[] m_ContactsBuffer = new ContactPoint2D[16];

        private PlayerAnimationController animationController;
        
        [SerializeField] private ParticleSystem rangedInitialEffect = null;        

        // MonoBehaviour Messages - called by Unity internally.
        void Awake()
        {
            s_PlayerInstance = this;

            m_CharacterController2D = GetComponent<CharacterController2D>();
            m_Animator = GetComponent<Animator>();
            m_Capsule = GetComponent<CapsuleCollider2D>();
            m_Transform = transform;
            m_InventoryController = GetComponent<InventoryController>();

            m_CurrentBulletSpawnPoint = spriteOriginallyFacesLeft ? facingLeftBulletSpawnPoint : facingRightBulletSpawnPoint;
        }

        void Start()
        {
            hurtJumpAngle = Mathf.Clamp(hurtJumpAngle, k_MinHurtJumpAngle, k_MaxHurtJumpAngle);
            m_TanHurtJumpAngle = Mathf.Tan(Mathf.Deg2Rad * hurtJumpAngle);
            m_FlickeringWait = new WaitForSeconds(flickeringDuration);

            meleeDamager.DisableDamage();

            m_ShotSpawnGap = 1f / shotsPerSecond;

            if (!Mathf.Approximately(maxHorizontalDeltaDampTime, 0f))
            {
                float maxHorizontalDelta = maxRunSpeed * cameraHorizontalSpeedOffset + cameraHorizontalFacingOffset;
                m_CamFollowHorizontalSpeed = maxHorizontalDelta / maxHorizontalDeltaDampTime;
            }

            if (!Mathf.Approximately(maxVerticalDeltaDampTime, 0f))
            {
                float maxVerticalDelta = cameraVerticalInputOffset;
                m_CamFollowVerticalSpeed = maxVerticalDelta / maxVerticalDeltaDampTime;
            }

            SceneLinkedSMB<PlayerCharacter>.Initialise(m_Animator, this);

            this.animationController = GetComponentInChildren<PlayerAnimationController>();
            this.SetOrientation(this.spriteOriginallyFacesLeft);

            m_StartingPosition = transform.position;
            m_StartingFacingLeft = GetFacing() < 0.0f;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Pushable pushable = other.GetComponent<Pushable>();
            if (pushable != null)
            {
                m_CurrentPushables.Add(pushable);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            Pushable pushable = other.GetComponent<Pushable>();
            if (pushable != null)
            {
                if (m_CurrentPushables.Contains(pushable))
                    m_CurrentPushables.Remove(pushable);
            }
        }

        void Update()
        {
            if (PlayerInput.Instance.Pause.Down)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif

                /* ###
                if (!m_InPause)
                {
                    if (ScreenFader.IsFading)
                        return;

                    PlayerInput.Instance.ReleaseControl(false);
                    PlayerInput.Instance.Pause.GainControl();
                    m_InPause = true;
                    Time.timeScale = 0;
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UIMenus", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                }
                else
                {
                    Unpause();
                }
                */
            }
            UpdateCameraFollowTargetPosition();
        }

        void FixedUpdate()
        { 
            m_CharacterController2D.Move(m_MoveVector * Time.deltaTime);
            m_Animator.SetFloat(m_HashHorizontalSpeedPara, m_MoveVector.x);
            m_Animator.SetFloat(m_HashVerticalSpeedPara, m_MoveVector.y);
            UpdateBulletSpawnPointPositions();
        }

        public void Unpause()
        {
            //if the timescale is already > 0, we 
            if (Time.timeScale > 0)
                return;

            StartCoroutine(UnpauseCoroutine());
        }

        protected IEnumerator UnpauseCoroutine()
        {
            Time.timeScale = 1;
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("UIMenus");
            PlayerInput.Instance.GainControl();
            //we have to wait for a fixed update so the pause button state change, otherwise we can get in case were the update
            //of this script happen BEFORE the input is updated, leading to setting the game in pause once again
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            m_InPause = false;
        }

        // Protected functions.
        protected void UpdateBulletSpawnPointPositions()
        {
            if (rightBulletSpawnPointAnimated)
            {
                Vector2 leftPosition = facingRightBulletSpawnPoint.localPosition;
                leftPosition.x *= -1f;
                facingLeftBulletSpawnPoint.localPosition = leftPosition;
            }
            else
            {
                Vector2 rightPosition = facingLeftBulletSpawnPoint.localPosition;
                rightPosition.x *= -1f;
                facingRightBulletSpawnPoint.localPosition = rightPosition;
            }
        }

        protected void UpdateCameraFollowTargetPosition()
        {
            float newLocalPosX;
            float newLocalPosY = 0f;

            float desiredLocalPosX = (spriteOriginallyFacesLeft ^ this.animationController.IsFacingLeft ? -1f : 1f) * cameraHorizontalFacingOffset;
            desiredLocalPosX += m_MoveVector.x * cameraHorizontalSpeedOffset;
            if (Mathf.Approximately(m_CamFollowHorizontalSpeed, 0f))
                newLocalPosX = desiredLocalPosX;
            else
                newLocalPosX = Mathf.Lerp(cameraFollowTarget.localPosition.x, desiredLocalPosX, m_CamFollowHorizontalSpeed * Time.deltaTime);

            bool moveVertically = false;
            if (!Mathf.Approximately(PlayerInput.Instance.Vertical.Value, 0f))
            {
                m_VerticalCameraOffsetTimer += Time.deltaTime;

                if (m_VerticalCameraOffsetTimer >= verticalCameraOffsetDelay)
                    moveVertically = true;
            }
            else
            {
                moveVertically = true;
                m_VerticalCameraOffsetTimer = 0f;
            }

            if (moveVertically)
            {
                float desiredLocalPosY = PlayerInput.Instance.Vertical.Value * cameraVerticalInputOffset;
                if (Mathf.Approximately(m_CamFollowVerticalSpeed, 0f))
                    newLocalPosY = desiredLocalPosY;
                else
                    newLocalPosY = Mathf.MoveTowards(cameraFollowTarget.localPosition.y, desiredLocalPosY, m_CamFollowVerticalSpeed * Time.deltaTime);
            }

            cameraFollowTarget.localPosition = new Vector2(newLocalPosX, newLocalPosY);
        }

        protected IEnumerator Flicker()
        {
            float timer = 0f;

            while (timer < damageable.invulnerabilityDuration)
            {
                this.animationController.ToggleRendererVisibility();
                yield return m_FlickeringWait;
                timer += flickeringDuration;
            }

            this.animationController.SetRendererEnabled(true);
        }

        protected IEnumerator SpawnBullet()
        {
            yield return new WaitForSeconds(0.35f);

            BulletObject bullet = bulletPool.Pop(m_CurrentBulletSpawnPoint.position);
            bool facingLeft = m_CurrentBulletSpawnPoint == facingLeftBulletSpawnPoint;
            bullet.rigidbody2D.velocity = new Vector2(facingLeft ? -bulletSpeed : bulletSpeed, 0f);
            bullet.spriteRenderer.flipX = facingLeft ^ bullet.bullet.spriteOriginallyFacesLeft;

            rangedAttackAudioPlayer.PlayRandomSound();
        }

        // Public functions - called mostly by StateMachineBehaviours in the character's Animator Controller but also by Events.
        public void SetMoveVector(Vector2 newMoveVector)
        {
            m_MoveVector = newMoveVector;
        }

        public void SetHorizontalMovement(float newHorizontalMovement)
        {
            m_MoveVector.x = newHorizontalMovement;
        }

        public void SetVerticalMovement(float newVerticalMovement)
        {
            // m_MoveVector.y = newVerticalMovement;
        }

        public void IncrementMovement(Vector2 additionalMovement)
        {
            m_MoveVector += additionalMovement;
        }

        public void IncrementHorizontalMovement(float additionalHorizontalMovement)
        {
            m_MoveVector.x += additionalHorizontalMovement;
        }

        public void IncrementVerticalMovement(float additionalVerticalMovement)
        {
            m_MoveVector.y += additionalVerticalMovement;
        }

        public void GroundedVerticalMovement()
        {
            /*
            m_MoveVector.y -= gravity * Time.deltaTime;

            if (m_MoveVector.y < -gravity * Time.deltaTime * k_GroundedStickingVelocityMultiplier)
            {
                m_MoveVector.y = -gravity * Time.deltaTime * k_GroundedStickingVelocityMultiplier;
            }
            */
        }

        public Vector2 GetMoveVector()
        {
            return m_MoveVector;
        }

        public bool IsFalling()
        {
            return m_MoveVector.y < 0f && !m_Animator.GetBool(m_HashGroundedPara);
        }

        public void UpdateFacing()
        {
            bool faceLeft = PlayerInput.Instance.Horizontal.Value < 0f;
            bool faceRight = PlayerInput.Instance.Horizontal.Value > 0f;

            if (faceLeft)
            {
                SetOrientation(true);
                m_CurrentBulletSpawnPoint = facingLeftBulletSpawnPoint;
            }
            else if (faceRight)
            {
                SetOrientation(false);
                m_CurrentBulletSpawnPoint = facingRightBulletSpawnPoint;
            }
        }

        public void UpdateFacing(bool faceLeft)
        {
            SetOrientation(faceLeft);
            m_CurrentBulletSpawnPoint = faceLeft ? facingLeftBulletSpawnPoint : facingRightBulletSpawnPoint;
        }

        public float GetFacing()
        {
            return this.animationController.IsFacingLeft != spriteOriginallyFacesLeft ? -1f : 1f;
        }

        public void GroundedHorizontalMovement(bool useInput, float speedScale = 1f)
        {
            PlayerInput playerInput = PlayerInput.Instance;
            float maxSpeedResult = maxRunSpeed;
            
            float desiredSpeed = useInput ? playerInput.Horizontal.Value * maxSpeedResult * speedScale : 0f;
            float acceleration = useInput && playerInput.Horizontal.ReceivingInput ? groundAcceleration : groundDeceleration;
            m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);

            if (useInput)
                this.animationController.UpdateGroundedAnimation(m_MoveVector.x);
        }

        public void CheckForCrouching()
        {
            m_Animator.SetBool(m_HashCrouchingPara, PlayerInput.Instance.Vertical.Value < 0f);
        }

        public bool CheckForGrounded()
        {
            bool wasGrounded = m_Animator.GetBool(m_HashGroundedPara);
            bool grounded = m_CharacterController2D.IsGrounded;

            if (grounded)
            {
                FindCurrentSurface();
            }
            else
                m_CurrentSurface = null;

            m_Animator.SetBool(m_HashGroundedPara, grounded);

            return grounded;
        }

        public void FindCurrentSurface()
        {
            Collider2D groundCollider = m_CharacterController2D.GroundColliders[0];

            if (groundCollider == null)
                groundCollider = m_CharacterController2D.GroundColliders[1];

            if (groundCollider == null)
                return;

            TileBase b = PhysicsHelper.FindTileForOverride(groundCollider, transform.position, Vector2.down);
            if (b != null)
            {
                m_CurrentSurface = b;
            }
        }

        public void CheckForPushing()
        {
            bool pushableOnCorrectSide = false;
            Pushable previousPushable = m_CurrentPushable;

            m_CurrentPushable = null;

            if (m_CurrentPushables.Count > 0)
            {
                bool movingRight = PlayerInput.Instance.Horizontal.Value > float.Epsilon;
                bool movingLeft = PlayerInput.Instance.Horizontal.Value < -float.Epsilon;

                for (int i = 0; i < m_CurrentPushables.Count; i++)
                {
                    float pushablePosX = m_CurrentPushables[i].pushablePosition.position.x;
                    float playerPosX = m_Transform.position.x;
                    if (pushablePosX < playerPosX && movingLeft || pushablePosX > playerPosX && movingRight)
                    {
                        pushableOnCorrectSide = true;
                        m_CurrentPushable = m_CurrentPushables[i];
                        break;
                    }
                }

                if (pushableOnCorrectSide)
                {
                    Vector2 moveToPosition = movingRight ? m_CurrentPushable.playerPushingRightPosition.position : m_CurrentPushable.playerPushingLeftPosition.position;
                    moveToPosition.y = m_CharacterController2D.Rigidbody2D.position.y;
                    m_CharacterController2D.Teleport(moveToPosition);
                }
            }

            if(previousPushable != null && m_CurrentPushable != previousPushable)
            {//we changed pushable (or don't have one anymore), stop the old one sound
                previousPushable.EndPushing();
            }

            m_Animator.SetBool(m_HashPushingPara, pushableOnCorrectSide);
        }

        public void MovePushable()
        {
            //we don't push ungrounded pushable, avoid pushing floating pushable or falling pushable.
            if (m_CurrentPushable && m_CurrentPushable.Grounded)
                m_CurrentPushable.Move(m_MoveVector * Time.deltaTime);
        }

        public void StartPushing()
        {
            if (m_CurrentPushable)
                m_CurrentPushable.StartPushing();
        }

        public void StopPushing()
        {
            if(m_CurrentPushable)
                m_CurrentPushable.EndPushing();
        }

        public void UpdateJump()
        {
            /*
            if (!PlayerInput.Instance.Dash.Held && m_MoveVector.y > 0.0f)
            {
                m_MoveVector.y -= jumpAbortSpeedReduction * Time.deltaTime;
            }
            */
        }

        public void AirborneHorizontalMovement()
        {
            float desiredSpeed = PlayerInput.Instance.Horizontal.Value * maxRunSpeed;

            float acceleration;

            if (PlayerInput.Instance.Horizontal.ReceivingInput)
                acceleration = groundAcceleration * airborneAccelProportion;
            else
                acceleration = groundDeceleration * airborneDecelProportion;

            m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * Time.deltaTime);
        }

        public void AirborneVerticalMovement()
        {
            /*
            if (Mathf.Approximately(m_MoveVector.y, 0f) || m_CharacterController2D.IsCeilinged && m_MoveVector.y > 0f)
            {
                m_MoveVector.y = 0f;
            }
            m_MoveVector.y -= gravity * Time.deltaTime;
            */
        }

        public bool CheckForDashInput()
        {
            return (PlayerInput.Instance.Dash.Down && !this.animationController.IsLocked);
        }

        public void Dash()
        {
            const float DASH_MULTIPLIER = 2.0f;
            float direction = this.animationController.IsFacingLeft ? -1.0f : 1.0f;
            m_MoveVector.x = jumpSpeed * DASH_MULTIPLIER * direction;
            this.animationController.PlayDash();

            m_Capsule.enabled = false;
            this.dashAudioPlayer.PlayRandomSound();

            StartCoroutine(WaitAndEnableCollider());
        }

        private IEnumerator WaitAndEnableCollider()
        {
            yield return new WaitForSeconds(0.5f);
            m_Capsule.enabled = true;
        }

        public bool CheckForFallInput()
        {
            return PlayerInput.Instance.Vertical.Value < -float.Epsilon && PlayerInput.Instance.Dash.Down;
        }

        public bool MakePlatformFallthrough()
        {
            int colliderCount = 0;
            int fallthroughColliderCount = 0;
        
            for (int i = 0; i < m_CharacterController2D.GroundColliders.Length; i++)
            {
                Collider2D col = m_CharacterController2D.GroundColliders[i];
                if(col == null)
                    continue;

                colliderCount++;

                if (PhysicsHelper.ColliderHasPlatformEffector (col))
                    fallthroughColliderCount++;
            }

            if (fallthroughColliderCount == colliderCount)
            {
                for (int i = 0; i < m_CharacterController2D.GroundColliders.Length; i++)
                {
                    Collider2D col = m_CharacterController2D.GroundColliders[i];
                    if (col == null)
                        continue;

                    PlatformEffector2D effector;
                    PhysicsHelper.TryGetPlatformEffector (col, out effector);
                    FallthroughReseter reseter = effector.gameObject.AddComponent<FallthroughReseter>();
                    reseter.StartFall(effector);
                    //set invincible for half a second when falling through a platform, as it will make the player "standup"
                    StartCoroutine(FallThroughtInvincibility());
                }
            }

            return fallthroughColliderCount == colliderCount;
        }

        IEnumerator FallThroughtInvincibility()
        {
            damageable.EnableInvulnerability(true);
            yield return new WaitForSeconds(0.5f);
            damageable.DisableInvulnerability();
        }

        public void CheckForHoldingGun()
        {
            if (!this.animationController.IsLocked)
                m_Animator.SetBool(m_HashHoldingGunPara, false);
        }
        
        public void CheckAndFireGun()
        {
            if (PlayerInput.Instance.RangedAttack.Down && PlayerHUD.Instance.HasMana && !this.animationController.IsLocked)
            {
                PlayerHUD.Instance.DecrementMana();
                
                m_MoveVector.x = 0;
                m_Animator.SetBool(m_HashHoldingGunPara, true);
                this.animationController.PlayRangedAttack();

                this.PlayInitialRangedEffect();
                StartCoroutine(SpawnBullet());
            }
        }

        private void PlayInitialRangedEffect()
        {
            if (!this.rangedInitialEffect.gameObject.activeSelf)
                this.rangedInitialEffect.gameObject.SetActive(true);

            float x = this.animationController.IsFacingLeft ? -1.0f : 1.0f;

            Transform effectTransform = this.rangedInitialEffect.transform; 
            Vector3 position = effectTransform.localPosition;
            position.x = x;

            effectTransform.localPosition = position;
            this.rangedInitialEffect.Play();
        }

        public void ForceNotHoldingGun()
        {
            m_Animator.SetBool(m_HashHoldingGunPara, false);
        }

        public void EnableInvulnerability()
        {
            damageable.EnableInvulnerability();
        }

        public void DisableInvulnerability()
        {
            damageable.DisableInvulnerability();
        }

        public Vector2 GetHurtDirection()
        {
            Vector2 damageDirection = damageable.GetDamageDirection();

            if (damageDirection.y < 0f)
                return new Vector2(Mathf.Sign(damageDirection.x), 0f);

            float y = Mathf.Abs(damageDirection.x) * m_TanHurtJumpAngle;

            return new Vector2(damageDirection.x, 0).normalized;
        }

        public void OnHurt(Damager damager, Damageable damageable)
        {
            //if the player don't have control, we shouldn't be able to be hurt as this wouldn't be fair
            if (!PlayerInput.Instance.HaveControl)
                return;

            UpdateFacing(damageable.GetDamageDirection().x > 0f);
            damageable.EnableInvulnerability();

            this.hitAudioPlayer.PlayRandomSound();
            m_Animator.SetTrigger(m_HashHurtPara);
            this.animationController.PlayHit();

            //we only force respawn if helath > 0, otherwise both forceRespawn & Death trigger are set in the animator, messing with each other.
            if(damageable.CurrentHealth > 0 && damager.forceRespawn)
                m_Animator.SetTrigger(m_HashForcedRespawnPara);

            m_Animator.SetBool(m_HashGroundedPara, false);
            hurtAudioPlayer.PlayRandomSound();

            this.UpdateLife();
            
            //if the health is < 0, mean die callback will take care of respawn
            if(damager.forceRespawn && damageable.CurrentHealth > 0)
            {
                StartCoroutine(DieRespawnCoroutine());
            }
        }

        public void OnDie()
        {
            m_Animator.SetTrigger(m_HashDeadPara);
            this.animationController.PlayDie();

            StartCoroutine(DieRespawnCoroutine());
        }

        IEnumerator DieRespawnCoroutine()
        {
            PlayerInput.Instance.ReleaseControl(true);
            yield return new WaitForSeconds(0.5f); //wait one second before respawing

            SceneController.RestartZone();

            /*
            yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.GameOver));
            
            yield return new WaitForSeconds(2.0f);

            SceneController.RestartZone();
            
            // SceneManager.LoadScene("Start");

            while (SceneController.Transitioning)
                yield return null;
            
            yield return StartCoroutine(ScreenFader.FadeSceneIn());
            */
        }

        public void StartFlickering()
        {
            m_FlickerCoroutine = StartCoroutine(Flicker());
        }

        public void StopFlickering()
        {
            StopCoroutine(m_FlickerCoroutine);
            this.animationController.SetRendererEnabled(true);
        }

        public bool CheckForMeleeAttackInput()
        {
            return PlayerInput.Instance.MeleeAttack.Down;
        }
        
        public bool CheckForSecondAttackInput()
        {
            return PlayerInput.Instance.SecondAttack.Down;
        }

        public void MeleeAttack()
        {
            m_Animator.SetTrigger(m_HashMeleeAttackPara);
            this.animationController.PlayMeleeAttack();
        }

        public bool IsAttackOngoing()
        {
            return this.animationController.IsLocked;
        }

        public void SecondAttack()
        {
            if (!this.animationController.IsLocked)
            {
                StartCoroutine(WaitAndAnimateSecondAttack());
            }
        }

        private IEnumerator WaitAndAnimateSecondAttack()
        {
            this.animationController.PlaySecondAttack();
            
            yield return new WaitForSeconds(0.25f);
            
            this.meleeDamager.SetXOffset(3.0f);
            m_Animator.SetTrigger(m_HashMeleeAttackPara);
        }

        public void ResetDamagerOffset()
        {
            this.meleeDamager.SetXOffset(1.5f);
        }

        public void EnableMeleeAttack()
        {
            meleeDamager.EnableDamage();
            meleeDamager.disableDamageAfterHit = true;
            meleeAttackAudioPlayer.PlayRandomSound();

            m_MoveVector.x = 0;
        }

        public void DisableMeleeAttack()
        {
            meleeDamager.DisableDamage();
        }

        public void TeleportToColliderBottom()
        {
            Vector2 colliderBottom = m_CharacterController2D.Rigidbody2D.position + m_Capsule.offset + Vector2.down * m_Capsule.size.y * 0.5f;
            m_CharacterController2D.Teleport(colliderBottom);
        }

        public void PlayFootstep()
        {
            var footstepPosition = transform.position;
            footstepPosition.z -= 1;
            VFXController.Instance.Trigger("DustPuff", footstepPosition, 0, false, null, m_CurrentSurface);
        }

        public void Respawn(bool resetHealth, bool useCheckpoint)
        {
            if (resetHealth)
                damageable.SetHealth(damageable.startingHealth);

            //we reset the hurt trigger, as we don't want the player to go back to hurt animation once respawned
            m_Animator.ResetTrigger(m_HashHurtPara);
            if (m_FlickerCoroutine != null)
            {//we stop flcikering for the same reason
                StopFlickering();
            }

            m_Animator.SetTrigger(m_HashRespawnPara);

            if (useCheckpoint && m_LastCheckpoint != null)
            {
                UpdateFacing(m_LastCheckpoint.respawnFacingLeft);
                GameObjectTeleporter.Teleport(gameObject, m_LastCheckpoint.transform.position);
            }
            else
            {
                UpdateFacing(m_StartingFacingLeft);
                GameObjectTeleporter.Teleport(gameObject, m_StartingPosition);
            }
        }

        public void SetChekpoint(Checkpoint checkpoint)
        {
            m_LastCheckpoint = checkpoint;
        }

        //This is called by the inventory controller on key grab, so it can update the Key UI.
        public void KeyInventoryEvent()
        {
            if (KeyUI.Instance != null) KeyUI.Instance.ChangeKeyUI(m_InventoryController);
        }

        public void UpdateLife()
        {
            this.damageable.UpdatePlayerLife();
        }

        private void SetOrientation(bool left)
        {
            if (this.animationController != null)
                this.animationController.SetOrientation(left, this.meleeDamager.SetXDirection);
        }
    }
}
