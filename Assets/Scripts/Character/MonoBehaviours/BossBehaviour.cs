using System.Collections;
using Gamekit2D;
using UnityEngine;

namespace ProjectRaoni
{
    public class BossBehaviour : EnemyBehaviour
    {
        public override void Hit(Damager damager, Damageable damageable)
        {
            if (damageable.CurrentHealth <= 0)
                return;

            m_Animator.SetTrigger(m_HashHitPara);

            Vector2 throwVector = new Vector2(0, 3.0f);
            Vector2 damagerToThis = damager.transform.position - transform.position;

            throwVector.x = Mathf.Sign(damagerToThis.x) * -2.0f;
            m_MoveVector = throwVector;

            if (m_FlickeringCoroutine != null)
            {
                StopCoroutine(m_FlickeringCoroutine);
            }

            m_FlickeringCoroutine = StartCoroutine(Flicker(damageable));
            
            CameraShaker.Shake(0.15f, 0.3f);
            
            this.PlayHitEffect();
            this.hitAudio.PlayRandomSound();
        }

        protected IEnumerator Flicker(Damageable damageable)
        {
            float timer = 0f;

            while (timer < damageable.invulnerabilityDuration)
            {
                this.animationController.ToggleRendererVisibility();
                yield return new WaitForSeconds(base.flickeringDuration);
                timer += flickeringDuration;
            }

            this.animationController.SetRendererEnabled(true);
        }

        protected override IEnumerator WaitAndEnableDamage()
        {
            yield return new WaitForSeconds(0.5f);
            yield return base.WaitAndEnableDamage();
        }
    }
}
