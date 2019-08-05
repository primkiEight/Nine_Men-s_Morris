using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Generic Rotate Around Target Script - used only on an evnironment windmill in this project
public class RotateAroundTarget : MonoBehaviour
{
    public Transform Center;
    public float Speed;
    public Vector3 Axis;
    public Vector3 Eulers;

    void Update()
    {
        transform.RotateAround(Center.position, Axis, Speed * Time.deltaTime);
        transform.Rotate(Eulers);
    }
}
