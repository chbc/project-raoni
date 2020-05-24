using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectRaoni
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private Image lifeBar = null;
        [SerializeField] private Image manaBar = null;
        [SerializeField] private float TOTAL_MANA = 4.0f;
        [SerializeField] private float RECHARGE_SPEED = 0.1f;

        public int ManaCount { get; private set; }
        public bool HasMana => ManaCount > 0;
        
        public static PlayerHUD Instance { get; private set; }

        private void OnEnable()
        {
            Instance = this;
        }

        private void Awake()
        {
            ManaCount = Mathf.RoundToInt(TOTAL_MANA);
        }

        private void Update()
        {
            float fillAmount = this.manaBar.fillAmount;
            if (fillAmount < 1.0f)
            {
                fillAmount += Time.deltaTime * RECHARGE_SPEED;

                this.ManaCount = Mathf.FloorToInt(TOTAL_MANA * fillAmount);
                this.manaBar.fillAmount = fillAmount;
            }
        }
        
        public void UpdateLifeBar(float amount)
        {
            this.lifeBar.fillAmount = amount;
        }

        public void DecrementMana()
        {
            this.manaBar.fillAmount -= 1.0f / TOTAL_MANA;
        }
    }
}
