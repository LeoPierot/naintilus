using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    // nombre de vies max
    // nombre de vies actuel
    // durée de l'invincibilité
    // fréquence de tir
    // (dégâts tir ?)
    // vélocité max

    [Header("External Components")]
    [SerializeField] private Camera _cam = default;
    [SerializeField] private Transform _armPivot = default;
    [Header("Settings")]
    [SerializeField] private float _bubblePushForce = 3f;
    [SerializeField] private float _verticalDrag = 2f;
    [SerializeField] private float _maxVelocity = 3.5f;
    [SerializeField] private float _maxHorizontalDelta = 8.5f;

    // TRASH TIER
    [SerializeField] private ParticleSystem _bubblePS = default;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _bubblePS.Stop();
    }

    private void Update()
    {
        RotateArm();

        if(Input.GetMouseButtonDown(0))
        {
            _bubblePS.Play();

        }
        else if(Input.GetMouseButtonUp(0))
        {
            _bubblePS.Stop();
        }

        if(Input.GetMouseButton(0))
        {
            Fire();
        }

        _rigidbody.AddForce(Vector3.down * _verticalDrag);

        if (_rigidbody.velocity.magnitude >= _maxVelocity)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _maxVelocity;
        }
    }

    private void RotateArm()
    {
        Vector2 mouseScreenPos = Input.mousePosition;

        Vector3 mouseWorldPos = _cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -(_cam.transform.position - _armPivot.position).z));
        Vector3 mouseDirection = mouseWorldPos - _armPivot.position;
        mouseDirection.Normalize();

        _armPivot.up = mouseDirection;
    }

    private void Fire()
    {
        Vector3 direction = new Vector3(_armPivot.up.x, _armPivot.up.y, 0f);

        if(_maxHorizontalDelta - Mathf.Abs(transform.position.x) <= 0.1f && -Mathf.Sign(direction.x) == Mathf.Sign(transform.position.x))
        {
            direction.x = 0f;
        }

        _rigidbody.AddForce(direction * -1f * _bubblePushForce);
    }

    // TakeDamage()

}