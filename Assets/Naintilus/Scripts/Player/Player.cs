using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    // nombre de vies max
    // nombre de vies actuel
    // dur�e de l'invincibilit�
    private enum PLAYER_STATE{FINE, GETTIN_SMASHED}

    [Header("External Components")]
    [SerializeField] private Camera _cam = default;
    [SerializeField] private Transform _armPivot = default;
    [Header("Movement Settings")]
    [SerializeField] private float _bubblePushForce = 3.0f;
    [SerializeField] private float _defaultMaxVelocity = 3.5f;
    [SerializeField] private float _divingVelocityModifier = 2.0f;
    [SerializeField] private float _maxHorizontalDelta = 8.5f;
    [Header("BubbleGun Settings")]
    [SerializeField] private Texture2D _crossHairCursor = default;
    [SerializeField] private Transform _canonOrigin = default;
    [SerializeField] private GameObject _bubblePrefab = default;
    [SerializeField] private float _bubbleFireRate = 0.1f;
    [SerializeField] private GameObject _torpedoPrefab = default;
    [SerializeField] private float _torpedoFireRate = 0.1f;

    [SerializeField] private float invulnerabilityTime = .5f;
    [SerializeField] private GameObject invulnerabilityBubble;
    [SerializeField] private Material invulnerabilityMat;
    [SerializeField] private Collider playerCollider;

    private Rigidbody _rigidbody;

    private bool _isFiringBubbles = false;
    private float _nextBubbleTime = 0.0f;
    private float _nextTorpedoTime = 0.0f;
    private Vector3 _bubbleForceDirection = Vector3.zero;
    private GameObject _bubblesHolder = null;
    private GameObject _torpedosHolder = null;
    private PLAYER_STATE _state;

    private Vector3 FiringDirection => new Vector3(_armPivot.up.x, _armPivot.up.y, 0.0f);

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.SetCursor(_crossHairCursor, Vector2.zero, CursorMode.Auto);

        _bubblesHolder = new GameObject("Bubbles");
        _bubblesHolder.transform.position = Vector3.zero;
        
        _torpedosHolder = new GameObject("Torpedos");
        _torpedosHolder.transform.position = Vector3.zero;
    }

    private void Update()
    {
        RotateArm();

        if(Input.GetMouseButton(0))
        {
            FireBubble();
            _isFiringBubbles = true;
        }
        else
        {
            _isFiringBubbles = false;
        }

        if(Input.GetMouseButton(1))
        {
            FireTorpedo();
        }
    }

    private void FixedUpdate()
    {
        float maxVelocity = _defaultMaxVelocity;

        if (_isFiringBubbles)
        {
            ApplyBubbleForce(out _bubbleForceDirection);

            if(Vector3.Dot(Vector3.down, _bubbleForceDirection) > .5f)
            {
                maxVelocity *= _divingVelocityModifier * Vector3.Dot(Vector3.down, _bubbleForceDirection);
            }
        }

        ApplyBounds();

        ClampVelocity(maxVelocity);
    }

    
    public void TakeDamage(Vector3 force){
        //take dmg
        StartCoroutine(GETSMASHED(force));
    }

    private IEnumerator GETSMASHED(Vector3 force)
    {
        _state = PLAYER_STATE.GETTIN_SMASHED;
        _rigidbody.velocity = Vector3.zero;
        yield return new WaitForSeconds(.2f);

        _cam.GetComponent<PlayerCam>().enabled = false;
        _rigidbody.isKinematic = true;
        yield return LerpPosition(transform.position, transform.position + force * 20, .75f);
        yield return new WaitForSeconds(1f);

        yield return Respawn();
    }

    private IEnumerator LerpPosition(Vector3 start, Vector3 end, float time)
    {
        float countdown = 0;
        float interpolator = 0;
        while(interpolator < 1)
        {
            transform.position = Vector3.Lerp(start, end, interpolator);
            countdown += Time.fixedDeltaTime;
            interpolator = countdown/time;
            yield return null;
        }
        transform.position = end;
    }

    private IEnumerator Respawn()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = Vector3.zero;
        transform.position = _cam.ViewportToWorldPoint(new Vector3(.5f, .5f, 9));
        _cam.GetComponent<PlayerCam>().enabled = true;
        playerCollider.enabled = false;
        yield return InvulnerabilityFeedback();
        playerCollider.enabled = true;
        
        
    }

    private IEnumerator InvulnerabilityFeedback()
    {
        invulnerabilityBubble.SetActive(true);
        float countdown = 0;
        invulnerabilityMat.SetFloat("_Dissolve", 0);
        while(countdown < invulnerabilityTime)
        {
            invulnerabilityMat.SetFloat("_Dissolve", Mathf.Lerp(0, 1, countdown/invulnerabilityTime));
            countdown += Time.deltaTime;
            yield return null;
        }
        invulnerabilityBubble.SetActive(false);
    }

    private void RotateArm()
    {
        Vector2 mouseScreenPos = Input.mousePosition;

        Vector3 mouseWorldPos = _cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -(_cam.transform.position - _armPivot.position).z));
        Vector3 mouseDirection = mouseWorldPos - _armPivot.position;
        mouseDirection.Normalize();

        _armPivot.up = mouseDirection;
    }

    private void FireBubble()
    {        
        if(Time.time >= _nextBubbleTime)
        {
            var bubbleInstance = Instantiate(_bubblePrefab, _canonOrigin.position, Quaternion.identity);
            bubbleInstance.transform.SetParent(_bubblesHolder.transform);
            bubbleInstance.transform.localScale = bubbleInstance.transform.localScale * Random.Range(0.6f, 1f);
            bubbleInstance.GetComponent<Projectile>().SetForward(FiringDirection);

            _nextBubbleTime = Time.time + 1.0f / _bubbleFireRate;

            Debug.Log("Fire bubble");
        }   
    }

    private void FireTorpedo()
    {
        if(Time.time >= _nextTorpedoTime)
        {
            var torpedoInstance = Instantiate(_torpedoPrefab, _canonOrigin.position, Quaternion.identity);
            torpedoInstance.transform.SetParent(_torpedosHolder.transform);
            torpedoInstance.transform.right = FiringDirection;
            torpedoInstance.GetComponent<Projectile>().SetForward(FiringDirection);

            _nextTorpedoTime = Time.time + 1.0f / _torpedoFireRate;
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
            _rigidbody.velocity = _rigidbody.velocity.normalized * maxVelocity;
        }
    }
}
