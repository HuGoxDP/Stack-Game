using UnityEngine;

namespace _Project.Scripts.Architecture
{
    public struct Primitive
    {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public float MinX { get; set; }
        public float MaxX { get; set; }
        public float MinZ { get; set; }
        public float MaxZ { get; set; }

        public Primitive(Vector3 position, Vector3 scale) {
            Position = position;
            Scale = scale;

            MinX = position.x - scale.x / 2;
            MaxX = position.x + scale.x / 2;
            MinZ = position.z - scale.z / 2;
            MaxZ = position.z + scale.z / 2;
        }
    }
}