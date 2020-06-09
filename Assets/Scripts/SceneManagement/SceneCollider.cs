using UnityEngine;

namespace ProjectRaoni
{
    public class SceneCollider : MonoBehaviour
    {
        [SerializeField] private int index = 0;
        
        private void Start()
        {
            EnemiesController.Instance.AddEnemiesBeatenListener(OnAllEnemiesBeaten);
        }

        private void OnAllEnemiesBeaten(int index)
        {
            if (this.index == index)
            {
                EnemiesController.Instance.RemoveEnemiesBeatenListener(OnAllEnemiesBeaten);
                Destroy(base.gameObject);
            }
        }
    }
}
