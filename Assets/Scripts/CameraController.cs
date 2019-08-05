using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraController : MonoBehaviour {

    [Header("Main Camera And Camera Focus")]
    public Camera MainCamera;
    public Transform BoardTransformToLookAt;

    private Transform _mainCameraTransform;
    private Transform _transformToMoveTo;

    [Header("Camera moving points and behaviour")]
    public Transform LeftCameraPosition;
    public Transform MidleCameraPosition;
    public Transform RightCameraPosition;
    public float RotateSpeed;
    public Vector3 RotateAxis;
    public Vector3 RotateEulers;

    private PostProcessingProfile _postProcessingProfile;
    private bool _gameOver = false;

    private void Awake()
    {
        _mainCameraTransform = MainCamera.transform;
        CameraViewReset();
    }

    //Alternates camera view to match player view (GameManager)
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
        _transformToMoveTo = LeftCameraPosition;
        if (BoardTransformToLookAt)
        {
            _mainCameraTransform.LookAt(BoardTransformToLookAt);
        } else
        {
            Debug.Log("No board to look at; looking at 0, 0, 0.");
        }
    }

    //Moves camera to match player views
    public void Update()
    {
        if (!_gameOver)
        {
            float distance = Vector3.Distance(_mainCameraTransform.localPosition, _transformToMoveTo.localPosition);

            if (distance >= 0.1f)
            {
                _mainCameraTransform.localPosition = Vector3.MoveTowards(_mainCameraTransform.localPosition, _transformToMoveTo.localPosition, 40 * Time.deltaTime);
                _mainCameraTransform.LookAt(BoardTransformToLookAt);
            }
            else
            {
                _mainCameraTransform.localPosition = _transformToMoveTo.localPosition;
                _mainCameraTransform.LookAt(BoardTransformToLookAt);
            }
        }
        else
        {
            _mainCameraTransform.RotateAround(BoardTransformToLookAt.position, RotateAxis, RotateSpeed * Time.deltaTime);
        }
    }

    //Activates Camera rotation when the game is in the GameOver state (GameManager)
    public void ActivateGameOverCamera()
    {
        _gameOver = true;
    }

    //Updates PostProcessing Profiles when changing environments (Environment SO, GameManager)
    public void UpdatePostProcessingProfile(PostProcessingProfile profile)
    {
        _postProcessingProfile = profile;
        if(_mainCameraTransform.GetComponent<PostProcessingBehaviour>())
            _mainCameraTransform.GetComponent<PostProcessingBehaviour>().profile = _postProcessingProfile;
    }
}
