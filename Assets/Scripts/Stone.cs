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
                if (_clickBool)
                {
                    //Debug.Log("Setting a stone.");
                    //SetTheStone();
                    //Debug.Log("Alternate Player Turn");
                    //_gameManager.AlternatePlayerTurn();
                    //Debug.Log("Player Setup CountDown");
                    //_gameManager.PlayerSetupCountdown(true);
                    //Debug.Log("Check For Mill");
                    //_gameManager.ReportPlayerMove(CheckForMill());
                    //Debug.Log("Can Player Move On The Board?");
                    //_myBoard.CanPlayerMoveOnTheBoard();
                    //Debug.Log("A stone value mi je " + _myStoneValue);
                }
                else
                {
                    //Po pravilima ovo bi se trebalo onemogućiti
                    //Debug.Log("Removing a stone.");
                    //RemoveTheStone();
                    //Debug.Log("Alternate Player Turn");
                    //_gameManager.AlternatePlayerTurn();
                    //Debug.Log("Player Setup CountDown");
                    //_gameManager.PlayerSetupCountdown(false);
                }
                break;
            case GameState.Move:
                if (_gameManager.GetSelectedStone() != this)
                {
                    _gameManager.SetSelectedStone(this);
                }   
                break;
            case GameState.Flying:
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
