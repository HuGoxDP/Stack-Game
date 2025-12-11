using UnityEngine;

namespace _Project.Scripts.Architecture
{
    public class FallingPartSpawner : MonoBehaviour
    {
        [SerializeField] private PrimitiveSpawner _primitiveSpawner;
        [SerializeField] private float _destroyDelay = 0.5f;

        public void SpawnFallingPart(Primitive source, Primitive overlap, float height) {
            var y = source.Position.y;

            //Left part
            if (source.MinX < overlap.MinX) {
                TrySpawnPart(
                    new Vector3(overlap.MinX - source.MinX, height, overlap.Scale.z),
                    new Vector3((source.MinX + overlap.MinX) / 2, y, overlap.Position.z)
                );
            }

            //Right part
            if (source.MaxX > overlap.MaxX) {
                TrySpawnPart(
                    new Vector3(source.MaxX - overlap.MaxX, height, overlap.Scale.z),
                    new Vector3((source.MaxX + overlap.MaxX) / 2, y, overlap.Position.z)
                );
            }

            //Front part
            if (source.MinZ < overlap.MinZ) {
                TrySpawnPart(
                    new Vector3(overlap.Scale.x, height, overlap.MinZ - source.MinZ),
                    new Vector3(overlap.Position.x, y, (source.MinZ + overlap.MinZ) / 2)
                );
            }

            //Back part
            if (source.MaxZ > overlap.MaxZ) {
                TrySpawnPart(
                    new Vector3(overlap.Scale.x, height, source.MaxZ - overlap.MaxZ),
                    new Vector3(overlap.Position.x, y, (source.MaxZ + overlap.MaxZ) / 2)
                );
            }
        }

        private void TrySpawnPart(Vector3 scale, Vector3 position) {
            var part = _primitiveSpawner.SpawnPrimitive(scale, position);
            part.useGravity = true;
            part.isKinematic = false;

            _primitiveSpawner.ReturnToPoolWithDelay(part, _destroyDelay);
        }
    }
}