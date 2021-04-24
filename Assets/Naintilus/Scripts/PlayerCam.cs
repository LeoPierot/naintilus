using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private Transform _target = default;
    [SerializeField] private float _smoothTime = 0.125f;

    private void FixedUpdate()
    {
        float targetYPos = Mathf.Lerp(transform.position.y, _target.position.y, Time.time * _smoothTime);

        transform.position = new Vector3(transform.position.x, targetYPos, transform.position.z);
    }
}
