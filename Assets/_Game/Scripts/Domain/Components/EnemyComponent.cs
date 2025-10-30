using UnityEngine;

namespace Game.Scripts.Domain.Components
{
    public class EnemyComponent : MonoBehaviour
    {
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Rigidbody _targetRigidbody;
        
        public Transform TargetTransform => _targetTransform;
        public Rigidbody TargetRigidbody => _targetRigidbody;

        public void DestroyTargetObject()
        {
            Destroy(_targetTransform.gameObject);
        }
    }
}