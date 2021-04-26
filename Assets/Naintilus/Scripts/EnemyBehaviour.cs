using UnityEngine;
using System.Collections;

public abstract class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] protected Animator _explosionPrefab = default;
    [SerializeField] protected SpriteRenderer _renderer = default;
    [SerializeField] protected Collider _selfCollider = default;
    [SerializeField] protected Transform _target = default;
    [SerializeField] protected float _regularSpeed = 1.0f;
    [SerializeField] protected float _chaseSpeed = 2.0f;
    [SerializeField] protected float _reactionDistance = 2.0f;
    [SerializeField] protected float _reactionTime = .5f;


    protected Rigidbody _rigidbody;
    protected bool _hit;
    protected Vector3 _forwardDirection;

    public Transform Target => _target;
    public float RegularSpeed => _regularSpeed;
    public float ChaseSpeed => _chaseSpeed;
    public float ReactionDistance => _reactionDistance;
    public float ReactionTime => _reactionTime;

    public virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _target = GameObject.FindGameObjectWithTag("Player").transform;

        if(_target == null)
        {
            _target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _rigidbody.velocity = Vector3.zero;
            Vector3 smashDirection = _target.position - transform.position;
            other.GetComponent<Player>().TakeDamage(smashDirection);
            StartCoroutine(DieCoroutine());
        }
        else if(other.CompareTag("Torpedo"))
        {
            Destroy(other.gameObject);
            StartCoroutine(DieCoroutine());
        }
    }

    private IEnumerator DieCoroutine()
    {
        _renderer.enabled = false;
        _selfCollider.enabled = false;
        _explosionPrefab.SetBool("boom", true);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    public virtual void SetDirection(Vector3 direction) 
    {
        if (Mathf.Sign(direction.x) != Mathf.Sign(_forwardDirection.x))
        {
            Flip();
        }

        _forwardDirection = direction;
    }

    public virtual void Move(float speed)
    {
        _rigidbody.velocity = _forwardDirection * speed;
    }

    private void Flip()
    {
        transform.Rotate(transform.up, 180f);
    }
}