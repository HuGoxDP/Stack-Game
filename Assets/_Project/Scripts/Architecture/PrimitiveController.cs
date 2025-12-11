using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Architecture
{
    public class PrimitiveController : MonoBehaviour
    {
        [SerializeField] private AnimatePrimitive _animator;
        [SerializeField] private PrimitiveSpawner _primitiveSpawner;
        [SerializeField] private FallingPartSpawner _fallingPartSpawner;
        
        [Header("Primitive Size")]
        [field: SerializeField] public float Width { get; private set; }
        [field: SerializeField] public float Height { get; private set; }

        [Header("Animation Settings")] 
        [SerializeField] private float _fromDirectionRadius;
        [SerializeField] private float _toDirectionRadius = 90f;
        
        [Header("Primitive Visual")]
        [SerializeField] private Color[] _colors;
        
        private readonly List<Primitive> _primitives = new();

        private Rigidbody _currentPrimitiveRigidbody;
        private Rigidbody _lastSpawnedPrimitive;
        private float _currentHeight;
        private int _colorIndex;

        
        private void Start() {
            _currentHeight = Height;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                StopPrimitive();
            }

            if (Input.GetKeyDown(KeyCode.S)) {
                GenerateAndAnimatePrimitive();
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }
        }

        private void GenerateAndAnimatePrimitive() {
            var x = transform.position.x;
            var z = transform.position.z;
            var y = transform.position.y + _currentHeight;
            
            var newPrimitive = new Primitive(
                new Vector3(x, y, z), 
                !_lastSpawnedPrimitive ? new Vector3(Width, Height, Width) : _lastSpawnedPrimitive.transform.localScale,
                _colors[_colorIndex++ % _colors.Length]
                );

            _currentPrimitiveRigidbody = _primitiveSpawner?.SpawnPrimitive(newPrimitive);

            _animator?.Animate(_currentPrimitiveRigidbody?.transform, _lastSpawnedPrimitive?.position ?? Vector3.zero);
            _animator?.UpdateDirection(Random.Range(_fromDirectionRadius, _toDirectionRadius));

            _currentHeight += Height;
        }

        private void StopPrimitive() {
            if (!_currentPrimitiveRigidbody)
                return;

            _animator.Stop();

            var oldPrimitive = new Primitive(
                _lastSpawnedPrimitive?.position ?? Vector3.zero,
                _lastSpawnedPrimitive?.transform.localScale ?? new Vector3(Width, Height, Width)
            );

            var newPrimitive = new Primitive(_currentPrimitiveRigidbody.position, _currentPrimitiveRigidbody.transform.localScale);

            if (!Intersects(oldPrimitive,newPrimitive)) {
                _currentPrimitiveRigidbody.useGravity = true;
                _currentPrimitiveRigidbody.isKinematic = false;
                _primitiveSpawner.ReturnToPoolWithDelay(_currentPrimitiveRigidbody, 0.5f);
                _currentPrimitiveRigidbody = null;
                return;
            }

            _primitiveSpawner?.ReturnToPoolWithDelay(_currentPrimitiveRigidbody, 0);
            
            // Find overlap
            var overlap = GetOverlap(oldPrimitive, newPrimitive);
            if (overlap == null) {
                return;
            }

            var overlapPrimitive = _primitiveSpawner?.SpawnPrimitive((Primitive)overlap);
            _primitives.Add((Primitive)overlap);

            //Find falling parts
            _fallingPartSpawner.SpawnFallingPart(newPrimitive, (Primitive)overlap, Height);
            _lastSpawnedPrimitive = overlapPrimitive;
        }

        private Primitive? GetOverlap(in Primitive oldPrimitive, in Primitive currentPrimitive) {
            if (!Intersects(oldPrimitive, currentPrimitive)) {
                return null;
            }

            var overlapMinX = Mathf.Max(oldPrimitive.MinX, currentPrimitive.MinX);
            var overlapMaxX = Mathf.Min(oldPrimitive.MaxX, currentPrimitive.MaxX);
            var overlapMinZ = Mathf.Max(oldPrimitive.MinZ, currentPrimitive.MinZ);
            var overlapMaxZ = Mathf.Min(oldPrimitive.MaxZ, currentPrimitive.MaxZ);

            var overlapSizeX = overlapMaxX - overlapMinX;
            var overlapSizeZ = overlapMaxZ - overlapMinZ;
            var overlapScale = new Vector3(overlapSizeX, currentPrimitive.Scale.y, overlapSizeZ);

            var overlapPositionX = (overlapMinX + overlapMaxX) / 2;
            var overlapPositionZ = (overlapMinZ + overlapMaxZ) / 2;
            var overlapPosition = new Vector3(overlapPositionX, currentPrimitive.Position.y, overlapPositionZ);

            var overlap = new Primitive()
            {
                Position = overlapPosition,
                Scale = overlapScale,
                Color = currentPrimitive.Color,
                
                MinX = overlapMinX,
                MaxX = overlapMaxX,
                MinZ = overlapMinZ,
                MaxZ = overlapMaxZ
            };

            return overlap;
        }

        private bool  Intersects(in Primitive oldPrimitive,in Primitive currentPrimitive) {
            var xOverlap = oldPrimitive.MaxX > currentPrimitive.MinX && oldPrimitive.MinX < currentPrimitive.MaxX;
            var zOverlap = oldPrimitive.MaxZ > currentPrimitive.MinZ && oldPrimitive.MinZ < currentPrimitive.MaxZ;
            return xOverlap && zOverlap;
        }
    }
}