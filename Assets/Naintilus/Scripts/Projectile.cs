using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _forwardSpeed = 1.5f;
    [SerializeField] private float _lifeTime = 2.0f;
    [SerializeField] private AnimationCurve velocityOverTime;

    private float life = 0;
    private float normalizedLife {get{return life / _lifeTime;}}

    private Rigidbody _rigidbody;
    private Vector3 _forward;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Destroy(gameObject, _lifeTime);
    }

    private void FixedUpdate()
    {
        life += Time.fixedDeltaTime;
        _rigidbody.velocity = _forward * _forwardSpeed * velocityOverTime.Evaluate(normalizedLife);
    }

    public void SetForward(Vector3 direction)
    {
        _forward = direction;
    }
}
