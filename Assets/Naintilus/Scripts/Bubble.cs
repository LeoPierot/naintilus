using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private float _forwardSpeed = 1.5f;
    [SerializeField] private float _lifeTime = 2.0f;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Destroy(gameObject, 2.0f);
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = transform.right * _forwardSpeed;
    }
}
