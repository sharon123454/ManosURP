using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private Transform _bulletHitVFXPrefab;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private float _moveSpeed = 200f;

    private Vector3 _targetPosition;

    private void Update()
    {
        Vector3 moveDir = (_targetPosition - transform.position).normalized;

        float distanceBeforeMoving = Vector3.Distance(transform.position, _targetPosition);

        transform.position += moveDir * _moveSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, _targetPosition);

        if (distanceBeforeMoving < distanceAfterMoving)
        {
            transform.position = _targetPosition;

            if (_trailRenderer)
                _trailRenderer.transform.parent = null;

            Destroy(gameObject);

            Instantiate(_bulletHitVFXPrefab, _targetPosition, Quaternion.identity);
        }
    }

    public void SetUp(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

}