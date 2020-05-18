using UnityEngine;
using UnityEngine.UI;

namespace ProjectRaoni
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private Image lifeBar = null;

        public static PlayerHUD Instance { get; private set; }

        private void OnEnable()
        {
            Instance = this;
        }

        public void UpdateLifeBar(float amount)
        {
            this.lifeBar.fillAmount = amount;
        }
    }
}
