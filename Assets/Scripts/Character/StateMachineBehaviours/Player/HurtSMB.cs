using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    public class HurtSMB : SceneLinkedSMB<PlayerCharacter>
    {
        public override void OnSLStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.SetMoveVector(m_MonoBehaviour.GetHurtDirection() * m_MonoBehaviour.hurtJumpSpeed);
            m_MonoBehaviour.StartFlickering ();

            m_MonoBehaviour.StartCoroutine(WaitAndCheckForGrounded());
        }

        private IEnumerator WaitAndCheckForGrounded()
        {
            yield return new WaitForSeconds(0.5f);
            m_MonoBehaviour.CheckForGrounded();
        }
    }
}