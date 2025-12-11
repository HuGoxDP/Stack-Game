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

        private Rigidbody _newPrimitive;
        private Rigidbody _oldPrimitive;
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
            
            _newPrimitive = _primitiveSpawner?.SpawnPrimitive(
            !_oldPrimitive ? new Vector3(Width, Height, Width) : _oldPrimitive.transform.localScale,
            new Vector3(x, y, z));

            _animator?.Animate(_newPrimitive?.transform, _oldPrimitive?.position ?? Vector3.zero);
            _animator?.UpdateDirection(Random.Range(_fromDirectionRadius, _toDirectionRadius));

            _currentHeight += Height;
        }

        private void StopPrimitive() {
            if (!_newPrimitive)
                return;

            _animator.Stop();

            var oldPrimitive = new Primitive(
                _oldPrimitive?.position ?? Vector3.zero,
                _oldPrimitive?.transform.localScale ?? new Vector3(Width, Height, Width)
            );

            var newPrimitive = new Primitive(_newPrimitive.position, _newPrimitive.transform.localScale);

            if (!Intersects(oldPrimitive,newPrimitive)) {
                _newPrimitive.useGravity = true;
                _newPrimitive.isKinematic = false;
                _primitiveSpawner.ReturnToPoolWithDelay(_newPrimitive, 0.5f);
                _newPrimitive = null;
                return;
            }

            _primitiveSpawner?.ReturnToPoolWithDelay(_newPrimitive, 0);
            
            // Find overlap
            var overlap = GetOverlap(oldPrimitive, newPrimitive);
            if (overlap == null) {
                return;
            }

            var overlapPrimitive = _primitiveSpawner?.SpawnPrimitive((Primitive)overlap);
            _primitives.Add((Primitive)overlap);

            //Find falling parts
            _fallingPartSpawner.SpawnFallingPart(newPrimitive, (Primitive)overlap, Height);
            _oldPrimitive = overlapPrimitive;
        }

        private Primitive? GetOverlap(Primitive oldPrimitive, Primitive newPrimitive) {
            if (!Intersects(oldPrimitive, newPrimitive)) {
                return null;
            }

            var overlapMinX = Mathf.Max(oldPrimitive.MinX, newPrimitive.MinX);
            var overlapMaxX = Mathf.Min(oldPrimitive.MaxX, newPrimitive.MaxX);
            var overlapMinZ = Mathf.Max(oldPrimitive.MinZ, newPrimitive.MinZ);
            var overlapMaxZ = Mathf.Min(oldPrimitive.MaxZ, newPrimitive.MaxZ);

            var overlapSizeX = overlapMaxX - overlapMinX;
            var overlapSizeZ = overlapMaxZ - overlapMinZ;
            var overlapScale = new Vector3(overlapSizeX, newPrimitive.Scale.y, overlapSizeZ);

            var overlapPositionX = (overlapMinX + overlapMaxX) / 2;
            var overlapPositionZ = (overlapMinZ + overlapMaxZ) / 2;
            var overlapPosition = new Vector3(overlapPositionX, newPrimitive.Position.y, overlapPositionZ);

            var overlap = new Primitive()
            {
                Position = overlapPosition,
                Scale = overlapScale,

                MinX = overlapMinX,
                MaxX = overlapMaxX,
                MinZ = overlapMinZ,
                MaxZ = overlapMaxZ
            };

            return overlap;
        }

        private bool  Intersects(Primitive oldPrimitive, Primitive newPrimitive) {
            var xOverlap = oldPrimitive.MaxX > newPrimitive.MinX && oldPrimitive.MinX < newPrimitive.MaxX;
            var zOverlap = oldPrimitive.MaxZ > newPrimitive.MinZ && oldPrimitive.MinZ < newPrimitive.MaxZ;
            return xOverlap && zOverlap;
        }
    }
}