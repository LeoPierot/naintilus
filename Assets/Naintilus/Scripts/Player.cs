using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    // nombre de vies max
    // nombre de vies actuel
    // dur�e de l'invincibilit�
    // fr�quence de tir
    // (d�g�ts tir ?)
    // v�locit� max

    private enum PLAYER_STATE
    {
        FINE, GETTIN_SMASHED
    }

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
    private PLAYER_STATE _State;

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

        if (_rigidbody.velocity.magnitude >= _maxVelocity && Vector3.Dot(_rigidbody.velocity, Vector3.down) > .4f)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _maxVelocity;
        }
    }

    public void TakeDamage(Vector3 force){
        //take dmg
        StartCoroutine(GETSMASHED(force));
    }

    private IEnumerator GETSMASHED(Vector3 force)
    {
        _State = PLAYER_STATE.GETTIN_SMASHED;
        _rigidbody.velocity = Vector3.zero;
        yield return new WaitForSeconds(.2f);

        _cam.GetComponent<PlayerCam>().enabled = false;
        _rigidbody.isKinematic = true;
        yield return LerpPosition(transform.position, transform.position + force * 20, .75f);
        yield return new WaitForSeconds(1f);

        _rigidbody.isKinematic = false;
        _rigidbody.velocity = Vector3.zero;
        transform.position = _cam.ViewportToWorldPoint(new Vector3(.5f, .5f, 9));
        _cam.GetComponent<PlayerCam>().enabled = true;
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