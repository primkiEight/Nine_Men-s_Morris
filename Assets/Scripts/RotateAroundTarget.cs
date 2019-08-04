using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundTarget : MonoBehaviour
{
    public Transform Center;
    public float Speed;
    public Vector3 Axis;
    public Vector3 Eulers;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Center.position, Axis, Speed * Time.deltaTime);
        transform.Rotate(Eulers);
    }
}
