using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour {

    private Transform _myTransform;
    private bool _clickBool = false;

    private GameManager _gameManager;

    [Range(-1, 1, order = 0)]
    public int StonePlayerValue;

    public float MoveSpeed;
    private bool _isMoving = false;
    private Vector3 _pointBmove;

    private void Awake()
    {
        _myTransform = transform;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    private void OnMouseDown()
    {
        _clickBool = !_clickBool;

        switch (_gameManager.GetGameState())
        {
            case GameState.PlayerSetup:
                break;
            case GameState.Move:
                if (_gameManager.GetSelectedStone() != this)
                {
                    _gameManager.SetSelectedStone(this);
                }   
                break;
            case GameState.Flying:
                if (_gameManager.GetSelectedStone() != this)
                {
                    _gameManager.SetSelectedStone(this);
                }
                break;
            case GameState.GameOver:
                break;
            case GameState.Mill:

                if(StonePlayerValue == _gameManager.GetActivePlayerStone().StonePlayerValue)
                {
                    Debug.Log("You can not destroy your own stone.");
                } else if (StonePlayerValue != _gameManager.GetActivePlayerStone().StonePlayerValue)
                {
                    _myTransform.parent.GetComponentInParent<Slot>().RemoveTheStoneWithMill();
                }

                break;
            default:
                break;
        }
    }

    public void MoveTheStone (Transform pointBdestination)
    {
        _pointBmove = pointBdestination.position;

        MoveSpeed *= Vector3.Distance(_myTransform.position, _pointBmove) / MoveSpeed;

        _isMoving = true;
    }

    public void FlyTheStone(Transform pointBdestination)
    {
        _pointBmove = pointBdestination.position;

        //Animirati, s odgodom (Coo)

        _myTransform.position = _pointBmove;
    }

    private void Update()
    {
        if (_isMoving)
        {
            float distance = Vector3.Distance(_myTransform.position, _pointBmove);

            _myTransform.position = Vector3.MoveTowards(_myTransform.position, _pointBmove, MoveSpeed * Time.deltaTime);

            if(distance <= 0.1f)
            {
                _myTransform.position = _pointBmove;
                _isMoving = false;
            }
        }
    }
}
