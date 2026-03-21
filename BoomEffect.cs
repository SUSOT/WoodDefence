using System.Collections;
using ObjectPooling;
using UnityEngine;

namespace _01_Works.CM._01_Scripts
{
    public class BoomEffect : PoolableMono
    {
        [SerializeField] private float waitTime = 2.5f;
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = transform.GetComponent<ParticleSystem>();
        }

        public void Play()
        {
            _particleSystem.Play();
            StartCoroutine(PushToPool());
        }

        private IEnumerator PushToPool()
        {
            yield return new WaitForSeconds(waitTime);
            PoolManager.Instance.Push(this);
        }

        public override void ResetItem(){
        
        }
    }
}
