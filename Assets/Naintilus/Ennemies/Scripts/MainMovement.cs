using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMovement : MonoBehaviour
{
    public float inSpeed;
    public float outSpeed;
    public float anticipationTime;
    public float inFrameTime;
    private Rigidbody rb;
    private Camera cam;

    private Vector2 ViewportPos { get{return cam.WorldToViewportPoint(transform.position);}}

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        StartCoroutine(MovementCoroutine());
    }

    private IEnumerator MovementCoroutine()
    {
        yield return new WaitForSeconds(anticipationTime);
        rb.velocity = inSpeed * transform.forward;
        while(ViewportPos.x > 0 && ViewportPos.y > 0 && ViewportPos.x < 1 && ViewportPos.y < 1)
        {
            yield return null;
        }
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(inFrameTime);
        rb.velocity = outSpeed * -transform.forward;
    }
}
