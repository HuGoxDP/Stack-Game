using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.Architecture
{
    public class PrimitiveController : MonoBehaviour
    {
        [SerializeField] private AnimatePrimitive _animatePrimitive;
        [SerializeField] private PrimitiveSpawner _primitiveSpawner;

        [Header("Primitive Size")]
        [field: SerializeField] public float Width { get; private set; }
        [field: SerializeField] public float Height { get; private set; }

        private Transform _newPrimitive;
        private Transform _oldPrimitive;
        private float _currentHeight;
        private void Start()
        {
            _currentHeight = Height;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopPrimitive();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                GenerateAndAnimatePrimitive();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void GenerateAndAnimatePrimitive()
        {
            var x = transform.position.x;
            var z = transform.position.z;
            var y = transform.position.y + _currentHeight;
            if (_oldPrimitive == null)
            {
                _newPrimitive = _primitiveSpawner?.SpawnPrimitive(PrimitiveType.Cube, new Vector3(Width, Height, Width), new Vector3(x, y, z), Quaternion.identity);
            }
            else
            {
                _newPrimitive = _primitiveSpawner?.SpawnPrimitive(PrimitiveType.Cube, _oldPrimitive.localScale, new Vector3(x, y, z), Quaternion.identity);
            }
            _animatePrimitive?.Animate(_newPrimitive);
            _currentHeight += Height;
        }

        private void StopPrimitive()
        {
            if (_newPrimitive == null)
                return;

            _animatePrimitive.Stop();
            _newPrimitive.gameObject.SetActive(false);

            var oldScale = _oldPrimitive?.localScale ?? new Vector3(Width, Height, Width);
            var oldPosition = _oldPrimitive?.position ?? Vector3.zero;
            
            var newScale = _newPrimitive.localScale;
            var newPosition = _newPrimitive.position;
            
            var oldLeftZ = oldPosition.z - oldScale.z / 2; 
            var oldRightZ = oldPosition.z + oldScale.z / 2;
            
            var newLeftZ = newPosition.z - newScale.z / 2;
            var newRightZ = newPosition.z + newScale.z / 2;
            
            var intersectionLeftZ = Mathf.Max(oldLeftZ, newLeftZ);
            var intersectionRightZ = Mathf.Min(oldRightZ, newRightZ);

            if (intersectionLeftZ > intersectionRightZ)
            {
                var primitive = _primitiveSpawner.SpawnPrimitive(PrimitiveType.Cube, newScale, newPosition, Quaternion.identity);
                primitive.AddComponent<Rigidbody>();
                Destroy(primitive.gameObject, 1f);
                Debug.Log("Game Over!");
                return;
            }
            
            var calculateScale = intersectionRightZ - intersectionLeftZ;
            var calculateCenter = (intersectionLeftZ + intersectionRightZ) / 2;
            
            newPosition = new Vector3(newPosition.x, newPosition.y, calculateCenter);
            newScale = new Vector3(newScale.x, Height, calculateScale);
            
            var newPrimitive = _primitiveSpawner.SpawnPrimitive(PrimitiveType.Cube, newScale, newPosition, Quaternion.identity);
            _oldPrimitive = newPrimitive;

            // spawn drop primitive
            //check left part
            var leftPartSize = intersectionLeftZ - newLeftZ;
            if (leftPartSize > 0)
            {
                var leftPartCenter = (intersectionLeftZ + newLeftZ) / 2;
                var leftPartPosition = new Vector3(newPosition.x, newPosition.y, leftPartCenter);
                var leftPartScale = new Vector3(newScale.x, Height, leftPartSize);
                var primitive = _primitiveSpawner.SpawnPrimitive(PrimitiveType.Cube, leftPartScale, leftPartPosition, Quaternion.identity);
                
                primitive.AddComponent<Rigidbody>();
                Destroy(primitive.gameObject, 0.5f);
            }
            
            //check right part
            var rightPartSize = newRightZ - intersectionRightZ;
            if (rightPartSize > 0)
            {
                var rightPartCenter = (intersectionRightZ + newRightZ) / 2;
                var rightPartPosition = new Vector3(newPosition.x, newPosition.y, rightPartCenter);
                var rightPartScale = new Vector3(newScale.x, Height, rightPartSize);
                var primitive = _primitiveSpawner.SpawnPrimitive(PrimitiveType.Cube, rightPartScale, rightPartPosition, Quaternion.identity);
               
                primitive.AddComponent<Rigidbody>();
                Destroy(primitive.gameObject, 0.5f);
            }
            
            Destroy(_newPrimitive.gameObject);
        }
    }
}