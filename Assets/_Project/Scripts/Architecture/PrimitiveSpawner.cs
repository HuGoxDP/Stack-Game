using UnityEngine;

namespace _Project.Scripts.Architecture
{
    public class PrimitiveSpawner : MonoBehaviour
    {
        // TODO REMAKE: Make object pooling for primitives and spawn them from pool
        public Transform SpawnPrimitive(PrimitiveType type, Vector3 scale, Vector3 position, Quaternion rotation)
        {
            var primitive = GameObject.CreatePrimitive(type);
            primitive.transform.localScale = scale;
            primitive.transform.position = position;
            primitive.transform.rotation = rotation;
            return primitive.transform;
        }
    }
}