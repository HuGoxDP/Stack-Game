using UnityEngine;

namespace _Project.Scripts.Architecture
{
    public struct Primitive
    {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Color Color { get; set; }
        public float MinX => Position.x - Scale.x / 2;
        public float MaxX => Position.x + Scale.x / 2;
        public float MinZ => Position.z - Scale.z / 2;
        public float MaxZ => Position.z + Scale.z / 2;
        
        public Primitive(Vector3 position, Vector3 scale, Color color) {
            Position = position;
            Scale = scale;
            Color = color;
        }
    }
}