using UnityEngine;

namespace Gamekit2D
{
    public class MeleeAttackSMB : SceneLinkedSMB<PlayerCharacter>
    {
        int m_HashAirborneMeleeAttackState = Animator.StringToHash ("AirborneMeleeAttack");
        private static readonly int ExitAttack = Animator.StringToHash("ExitAttack");

        public override void OnSLStatePostEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.ForceNotHoldingGun();
            m_MonoBehaviour.EnableMeleeAttack();
        }

        public override void OnSLStateNoTransitionUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!m_MonoBehaviour.CheckForGrounded ())
                animator.Play (m_HashAirborneMeleeAttackState, layerIndex, stateInfo.normalizedTime);
            
            if (!m_MonoBehaviour.IsAnimationOngoing())
                animator.SetTrigger(ExitAttack);
        }

        public override void OnSLStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.DisableMeleeAttack();
            m_MonoBehaviour.ResetDamagerOffset();
        }
    }
}
