using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private Transform _target = default;
    [SerializeField, Range(0.01f, 1.0f)] private float _smoothFactorX = 0.125f;
    [SerializeField, Range(0.01f, 1.0f)] private float _smoothFactorY = 0.125f;
    [SerializeField] private float _maxAngleX = 5.0f;
    [SerializeField] private float _maxAngleY = 5.0f;

    private Vector3 _offset;
    private Quaternion _originalRotation;
    private Vector3 _originalForward;
    private Vector3 _dirXZ, _forwardXZ, _dirYZ, _forwardYZ;

    private Quaternion _targetRotation;

    private void Start()
    {
        _offset = transform.position - _target.position;

        _originalRotation = transform.rotation;

    }

    private void FixedUpdate()
    {

        Vector3 aimedPosition =  _target.position + _offset;
        aimedPosition.x = transform.position.x;
        aimedPosition.z = transform.position.z;

        _targetRotation = Quaternion.LookRotation(_target.transform.position - transform.position);
        _targetRotation.eulerAngles = new Vector3(_targetRotation.eulerAngles.x,
                                                  Mathf.Clamp(_targetRotation.eulerAngles.y, -_maxAngleX, _maxAngleX),
                                                  _targetRotation.eulerAngles.z);

        Vector3 dirToTarget = _target.position - transform.position;
        _originalForward = _originalRotation * Vector3.forward;

        _dirXZ = Vector3.ProjectOnPlane(dirToTarget, Vector3.up);
        _forwardXZ = Vector3.ProjectOnPlane(_originalForward, Vector3.up);
        float yAngle = Vector3.Angle(_dirXZ, _forwardXZ) * Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(_forwardXZ, _dirXZ)));
        float yClamped = Mathf.Clamp(yAngle, -_maxAngleX, _maxAngleX);
        Quaternion yRotation = Quaternion.AngleAxis(yClamped, Vector3.up);

        _originalForward = yRotation * _originalRotation * Vector3.forward;
        Vector3 xAxis = yRotation * _originalRotation * Vector3.right;
        _dirYZ = Vector3.ProjectOnPlane(dirToTarget, xAxis);
        _forwardYZ = Vector3.ProjectOnPlane(_originalForward, xAxis);
        float xAngle = Vector3.Angle(_dirYZ, _forwardYZ) * Mathf.Sign(Vector3.Dot(xAxis, Vector3.Cross(_forwardYZ, _dirYZ)));
        float xClamped = Mathf.Clamp(xAngle, -_maxAngleY, _maxAngleY);
        Quaternion xRotation = Quaternion.AngleAxis(xClamped, Vector3.right);
        Quaternion newRotation = yRotation * _originalRotation * xRotation;

        transform.position = Vector3.Slerp(transform.position, aimedPosition, _smoothFactorY);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, _smoothFactorX);

        
    }
}
