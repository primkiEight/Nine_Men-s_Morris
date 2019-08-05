using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour {

    private Transform _myTransform;
    private bool _clickBool = false;
    private bool _IsInAMill = false;
    private GameManager _gameManager;

    [Header("Player1 Value = 1, Player2 Value = -1")]
    [Range(-1, 1, order = 0)]
    public int StonePlayerValue;

    [Header("Visual Offsets For Proper Object Placing")]
    public Vector3 ViewPositionOffset;
    public Quaternion ViewRotationOffset;

    [Header("Player Moving Speed")]
    public float MoveSpeed;
    private bool _isMoving = false;
    private Vector3 _pointBmove;

    [Header("Particle Fly Prefab")]
    public ParticleSystem ParticleSystemPrefab;

    private void Awake()
    {
        _myTransform = transform;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    //Defines stone's moving behaviour during the Move (and Flying) gamestate
    //Tells the GameManager which stone is the selected stone during the Move and Flying game states
    //Defines procedures if the stone can be removed during the Mill game state
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
                    GameManager.Instance.UICanvas.UpdateUIMessage(GameManager.Instance.UICanvas.Language.PlayerMillMessage04);
                } else if (StonePlayerValue != _gameManager.GetActivePlayerStone().StonePlayerValue)
                {
                    //When method returns true, we can destroy this stone since all of them are in one of the mills
                    if (_gameManager.Board.CanThisStoneBeDestroyed(this))
                    {
                        _myTransform.parent.GetComponentInParent<Slot>().RemoveTheStoneWithMill();
                    }
                    //When method returns false, we need to check if this stone is in a mill - if true, it can not be destroyed
                    else
                    {
                        if (_IsInAMill)
                        {
                            Debug.Log("This stone is in a mill and can not be destroyed!");
                        } else
                        {
                            Debug.Log("This stone is not in a mill and can be destroyed.");
                            _myTransform.parent.GetComponentInParent<Slot>().RemoveTheStoneWithMill();
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    //Defines the destination point and the adjusted speed for stone movement (for Update) (Slot)
    public void MoveTheStone (Transform pointBdestination)
    {
        _pointBmove = pointBdestination.position;

        MoveSpeed *= Vector3.Distance(_myTransform.position, _pointBmove) / MoveSpeed;

        _isMoving = true;
    }

    //Defines the destination point for flying stone movement
    //TODO: Add animation (particle system)
    public void FlyTheStone(Transform pointBdestination)
    {
        _pointBmove = pointBdestination.position + ViewPositionOffset;

        _myTransform.position = _pointBmove;

        Instantiate(ParticleSystemPrefab, transform.position, Quaternion.identity, transform);
    }

    //Moves the stone from point A to the point B
    private void Update()
    {
        if (_isMoving)
        {
            float distance = Vector3.Distance(_myTransform.position, _pointBmove + ViewPositionOffset);

            _myTransform.position = Vector3.MoveTowards(_myTransform.position, _pointBmove + ViewPositionOffset, MoveSpeed * Time.deltaTime);

            if(distance <= 0.1f)
            {
                _myTransform.position = _pointBmove + ViewPositionOffset;
                _isMoving = false;
            }
        }
    }

    //Sets the mill bool to disable the stone to be removed if in a mill (BoardManager)
    //Does not have a function if all the stones of the same player are in a mill
    public void SetStoneMillBool(bool status)
    {
        _IsInAMill = status;
    }
}
