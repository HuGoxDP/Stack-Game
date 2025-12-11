using System;
using UnityEngine;

namespace _Project.Scripts.Architecture
{
    public class CameraMover : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private void Start() {
            if (_camera == null) {
                _camera = Camera.main;
            }
        }

        public void MoveUp(float height) {
            if (_camera == null) {
                return;
            }

            var position = _camera.transform.position;
            position.y += height;
            _camera.transform.position = position;
        }
    }
}