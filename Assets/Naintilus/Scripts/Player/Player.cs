using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    // nombre de vies max
    // nombre de vies actuel
    // durée de l'invincibilité

    [Header("External Components")]
    [SerializeField] private Camera _cam = default;
    [SerializeField] private Transform _armPivot = default;
    [Header("Movement Settings")]
    [SerializeField] private float _bubblePushForce = 3.0f;
    [SerializeField] private float _verticalDrag = 2.0f;
    [SerializeField] private float _defaultMaxVelocity = 3.5f;
    [SerializeField] private float _divingVelocityModifier = 2.0f;
    [SerializeField] private float _velocityLerpDuration = 1.5f;
    [SerializeField] private float _maxHorizontalDelta = 8.5f;
    [Header("BubbleGun Settings")]
    [SerializeField] private Texture2D _crossHairCursor = default;
    [SerializeField] private Transform _bubbleOrigin = default;
    [SerializeField] private GameObject _bubblePrefab = default;
    [SerializeField] private float _fireRate = 0.1f;

    private Rigidbody _rigidbody;

    private bool _isFiring = false;
    private float _nextFiringTime = 0.0f;
    private Vector3 _bubbleForceDirection = Vector3.zero;
    private Coroutine _lerpVelocityCoroutine = null;
    private GameObject _bubblesHolder = null;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.SetCursor(_crossHairCursor, Vector2.zero, CursorMode.Auto);
        _bubblesHolder = new GameObject("Bubbles");
        _bubblesHolder.transform.position = Vector3.zero;
    }

    private void Update()
    {
        RotateArm();

        if(Input.GetMouseButton(0))
        {
            Fire();
            _isFiring = true;
        }
        else
        {
            _isFiring = false;
        }
    }

    private void FixedUpdate()
    {
        float maxVelocity = _defaultMaxVelocity;

        if (_isFiring)
        {
            ApplyBubbleForce(out _bubbleForceDirection);

            if (Vector3.Dot(Vector3.up, _bubbleForceDirection) > 0.05f)
            {
                _rigidbody.AddForce(Vector3.down * _verticalDrag);
            }
            else if(Vector3.Dot(Vector3.down, _bubbleForceDirection) > 0.95f)
            {
                maxVelocity *= _divingVelocityModifier;
            }
        }

        ApplyBounds();

        ClampVelocity(maxVelocity);
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
        var firingDirection = new Vector3(_armPivot.up.x, _armPivot.up.y, 0.0f);
        
        if(Time.time >= _nextFiringTime)
        {
            var bubbleInstance = Instantiate(_bubblePrefab, _bubbleOrigin.position, Quaternion.identity);
            bubbleInstance.transform.right = firingDirection;
            bubbleInstance.transform.SetParent(_bubblesHolder.transform);

            _nextFiringTime = Time.time + 1.0f / _fireRate;
        }
        
    }

    private void ApplyBubbleForce(out Vector3 bubbleForceDirection)
    {
        bubbleForceDirection = new Vector3(_armPivot.up.x, _armPivot.up.y, 0.0f).normalized;
        bubbleForceDirection *= -1f;

        _rigidbody.AddForce(bubbleForceDirection * _bubblePushForce);
    }

    private void ApplyBounds()
    {
        if(_maxHorizontalDelta - Mathf.Abs(transform.position.x) <= 0.1f && Mathf.Sign(_rigidbody.velocity.x) == Mathf.Sign(transform.position.x))
        {
            var oppositeForceDir = new Vector3(-_rigidbody.velocity.x, 0.0f, 0.0f);
            oppositeForceDir.Normalize();

            _rigidbody.AddForce(oppositeForceDir * _bubblePushForce * 4f);
        }
    }

    private void ClampVelocity(float maxVelocity)
    {
        if (_rigidbody.velocity.magnitude >= maxVelocity)
        {
            // Si IsDiving => Clamp à maxvelocity * divingvelocitymodifier
            // Si IsDiving == true last frame && IsDiving == false this frame, stoppeddiving = true => startcoroutine;

            /*if(_lerpVelocityCoroutine == null && _stoppedDiving)
            {
                _lerpVelocityCoroutine = StartCoroutine(LerpVelocity(maxVelocity, _velocityLerpDuration));
                _stoppedDiving = false;
            }*/

            _rigidbody.velocity = _rigidbody.velocity.normalized * maxVelocity;
        }
    }

    private IEnumerator LerpVelocity(float maxVelocity, float _velocityLerpDuration)
    {
        float clampTime = 0.0f;
        float rate = 1.0f / _velocityLerpDuration;

        while(clampTime < _velocityLerpDuration)
        {
            clampTime += Time.fixedDeltaTime * rate;

            _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, _rigidbody.velocity.normalized * maxVelocity, clampTime);

            yield return null;
        }

        _lerpVelocityCoroutine = null;
    }

    // TakeDamage()

}