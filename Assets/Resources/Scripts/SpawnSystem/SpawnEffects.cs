using DG.Tweening;
using UnityEngine;

namespace Platformer {
    [RequireComponent(typeof(AudioSource))]
    public class SpawnEffects : MonoBehaviour {
        [SerializeField] private GameObject spawnVfx;
        [SerializeField] private float animationDuration = 1f;

        private void Start() {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack);
            
            if (spawnVfx) Instantiate(spawnVfx, transform.position, Quaternion.identity);
        }
    }
}