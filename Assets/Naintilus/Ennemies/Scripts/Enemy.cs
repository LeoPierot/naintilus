using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public string name;
    public GameObject prefab;


    public void Instantiate(Vector3 position, Vector3 forward)
    {
        GameObject instance = GameObject.Instantiate(prefab, position, Quaternion.identity);
        instance.name = prefab.name;
        instance.transform.forward = forward;
    }
}
