using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _forwardSpeed = 1.5f;
    [SerializeField] private float _lifeTime = 2.0f;

    private Rigidbody _rigidbody;
    private Vector3 _forward;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Destroy(gameObject, _lifeTime);
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _forward * _forwardSpeed;
    }

    public void SetForward(Vector3 direction)
    {
        _forward = direction;
    }
}
