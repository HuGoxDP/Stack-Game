using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Architecture
{
    public class PrimitiveSpawner : MonoBehaviour
    {
        private ObjectPool<Rigidbody> _pool;
        private Dictionary<int, IEnumerator> _coroutines;

        private void Awake() {
            _coroutines = new Dictionary<int, IEnumerator>();
            _pool = new ObjectPool<Rigidbody>(CreateFunc, ActionOnGet, ActionOnRelease);
        }

        private void OnDisable() {
            foreach (var coroutine in _coroutines.Values) {
                StopCoroutine(coroutine);
            }
        }

        private void OnEnable() {
            foreach (var coroutine in _coroutines.Values) {
                StartCoroutine(coroutine);
            }
        }

        private void OnDestroy() {
            _pool?.Dispose();
        }

        private void ActionOnRelease(Rigidbody obj) {
            obj.gameObject.SetActive(false);
        }

        private void ActionOnGet(Rigidbody obj) {
            obj.useGravity = false;
            obj.isKinematic = true;
            obj.gameObject.SetActive(true);
        }

        private Rigidbody CreateFunc() {
            var primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var rb = primitive.AddComponent<Rigidbody>();
            primitive.gameObject.SetActive(false);
            return rb;
        }

        public void ReturnPrimitiveToPool(Rigidbody primitive, float delay = 0f) {
            var id = primitive.GetInstanceID();
            _coroutines.Add(id, ReturnToPool(primitive, delay, id));
            StartCoroutine(ReturnToPool(primitive, delay, id));
        }

        private IEnumerator ReturnToPool(Rigidbody primitive, float delay, int id) {
            yield return new WaitForSeconds(delay);
            _pool.Release(primitive);
            _coroutines.Remove(id);
        }
        
        public Rigidbody SpawnPrimitive(in Primitive primitive) {
            var rb = _pool.Get();
            rb.transform.rotation = Quaternion.identity;
            rb.transform.localScale = primitive.Scale;
            rb.transform.position = primitive.Position;
            
            if(rb.TryGetComponent<MeshRenderer>(out var meshRenderer)){
                meshRenderer.material.color = primitive.Color;}
            
            return rb;
        }
    }
}