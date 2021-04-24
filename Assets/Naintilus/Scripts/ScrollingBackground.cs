using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] private GameObject _cam;

    private float _length;
    public float _currentPosition;

    private void Start()
    {
        _currentPosition = transform.position.y;
        _length = transform.localScale.z * GetComponent<MeshFilter>().mesh.bounds.size.z;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, _currentPosition, transform.position.z);

        if (_cam.transform.position.y > _currentPosition + _length)
        {
            _currentPosition += _length;
        }
        else if (_cam.transform.position.y < _currentPosition - _length)
        {
            _currentPosition -= _length;
        }
    }
}
