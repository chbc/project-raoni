using UnityEngine;

namespace ProjectRaoni
{
    public class LoadingBar : MonoBehaviour
    {
        [SerializeField]
        private Transform _foregroundTransform = null;
        [SerializeField]
        private float _totalTime = 5.0f;

        private float _currentTime;
        
        private void Start()
        {
            _currentTime = 0.0f;
        }
        
        private void Update()
        {
            _currentTime += Time.deltaTime;
            Vector3 scale = _foregroundTransform.localScale;
            scale.x = Mathf.Min(_currentTime / _totalTime, 1.0f);
            _foregroundTransform.localScale = scale;
        }
    }
}
