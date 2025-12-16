using System.Collections;
using UnityEngine;

namespace Architecture
{
    public class AnimatePrimitive : MonoBehaviour
    {
        [SerializeField] private float _distance;
        [SerializeField] private float _speed;

        private float _direction;
        private bool _isAnimating;
        private Coroutine _coroutine;

        public void Animate(Transform primitive, Vector3 center) {
            if (_isAnimating) return;

            _isAnimating = true;
            _coroutine = StartCoroutine(AnimateCoroutine(primitive, center));
        }

        public void Stop() {
            if (!_isAnimating) return;
            StopCoroutine(_coroutine);
            _isAnimating = false;
        }

        public void UpdateDirection(float newDirection) {
            _direction = newDirection;
        }

        private IEnumerator AnimateCoroutine(Transform primitive, Vector3 center) {
            var rad = _direction * Mathf.Deg2Rad;
            var x = Mathf.Cos(rad) * _distance;
            var z = Mathf.Sin(rad) * _distance;
            var position = new Vector3(x, 0, z);

            var startPosition = center - position;
            var endPosition = center + position;

            startPosition.y = primitive.position.y;
            endPosition.y = primitive.position.y;

            float step = 0;

            while (true) {
                if (step >= 1) {
                    step = 0;
                    (startPosition, endPosition) = (endPosition, startPosition);
                }

                step += Time.deltaTime * _speed;
                primitive.position = Vector3.Lerp(startPosition, endPosition, step);
                yield return null;
            }
        }
    }
}