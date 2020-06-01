using UnityEngine;

namespace Gamekit2D
{
    public class ChomperAttackSMB : SceneLinkedSMB<EnemyBehaviour>
    {
        private static readonly int ExitAttack = Animator.StringToHash("ExitAttack");
        
        public override void OnSLStateNoTransitionUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!m_MonoBehaviour.IsAttackOngoing())
                animator.SetTrigger(ExitAttack);
        }
        
        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateExit(animator, stateInfo, layerIndex);

            m_MonoBehaviour.SetHorizontalSpeed(0);
            m_MonoBehaviour.EndAttack();
        }
    }
}
