using UnityEngine;

namespace ProjectRaoni
{
    public class SceneCollider : MonoBehaviour
    {
        private void Start()
        {
            EnemiesController.Instance.AddEnemiesBeatenListener(OnAllEnemiesBeaten);
        }

        private void OnAllEnemiesBeaten()
        {
            EnemiesController.Instance.RemoveEnemiesBeatenListener(OnAllEnemiesBeaten);
            Destroy(base.gameObject);
        }
    }
}
