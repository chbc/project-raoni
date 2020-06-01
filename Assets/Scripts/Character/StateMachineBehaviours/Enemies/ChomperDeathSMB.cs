using UnityEngine;

namespace Gamekit2D
{
    public class ChomperDeathSMB : SceneLinkedSMB<EnemyBehaviour>
    {
        private static readonly int ExitDeath = Animator.StringToHash("ExitDeath");
        
        public override void OnSLStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.DisableDamage ();
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!m_MonoBehaviour.animationController.IsLocked)
                animator.SetTrigger(ExitDeath);
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.gameObject.SetActive(false);
        }
    }
}
