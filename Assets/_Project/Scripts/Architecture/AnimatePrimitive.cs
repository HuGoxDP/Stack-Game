using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Architecture
{
    public class AnimatePrimitive : MonoBehaviour
    {
        [SerializeField] private float _speed;

        [Header("Points")] 
        [SerializeField] private Transform _startPosition;
        [SerializeField] private Transform _endPosition;

        private bool _isAnimating;
        private Coroutine _coroutine;

        public void Animate(Transform primitive)
        {
            if (_isAnimating) return;

            _isAnimating = true;
            _coroutine = StartCoroutine(AnimateCoroutine(primitive));
        }

        public void Stop()
        {
            if (!_isAnimating) return;
            StopCoroutine(_coroutine);
            _isAnimating = false;
        }

        private IEnumerator AnimateCoroutine(Transform primitive)
        {
            var startPosition = new Vector3(_startPosition.position.x, primitive.position.y, _startPosition.position.z);
            var endPosition = new Vector3(_endPosition.position.x, primitive.position.y, _endPosition.position.z);
            float step = 0;
            
            while (true)
            {
                step += Time.deltaTime * _speed;
                primitive.position = Vector3.Lerp(startPosition, endPosition, step);
                yield return null;
            }
        }
    }
}