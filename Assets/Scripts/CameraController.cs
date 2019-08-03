using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Camera MainCamera;
    public Transform BoardTransformToLookAt;

    private Transform _mainCameraTransform;

    public Transform LeftCameraPosition;
    public Transform MidleCameraPosition;
    public Transform RightCameraPosition;
    private Transform _transformToMoveTo;

    private void Awake()
    {
        _mainCameraTransform = MainCamera.transform;
        CameraViewReset();
    }

    public void CameraViewAlternate()
    {
        if(_transformToMoveTo == MidleCameraPosition)
        {
            _transformToMoveTo = LeftCameraPosition;
        } else if (_transformToMoveTo == LeftCameraPosition)
        {
            _transformToMoveTo = RightCameraPosition;
        } else if (_transformToMoveTo == RightCameraPosition)
        {
            _transformToMoveTo = LeftCameraPosition;
        }
    }

    public void CameraViewReset()
    {
        //_transformToMoveTo = MidleCameraPosition;
        _transformToMoveTo = LeftCameraPosition;
        if (BoardTransformToLookAt)
        {
            _mainCameraTransform.LookAt(BoardTransformToLookAt);
        } else
        {
            Debug.Log("No board to look at; looking at 0, 0, 0.");
        }
    }

    public void Update()
    {
        float distance = Vector3.Distance(_mainCameraTransform.localPosition, _transformToMoveTo.localPosition);

        if (distance >= 0.1f)
        {
            _mainCameraTransform.localPosition = Vector3.MoveTowards(_mainCameraTransform.localPosition, _transformToMoveTo.localPosition, 40 * Time.deltaTime);
            _mainCameraTransform.LookAt(BoardTransformToLookAt);
            //_mainCameraTransform.localRotation = Quaternion.RotateTowards(_mainCameraTransform.localRotation, _transformToMoveTo.localRotation, 40 * Time.deltaTime);
        }
        else
        {
            _mainCameraTransform.localPosition = _transformToMoveTo.localPosition;
            //_mainCameraTransform.localRotation = _transformToMoveTo.localRotation;
            _mainCameraTransform.LookAt(BoardTransformToLookAt);
        }
    }
}
