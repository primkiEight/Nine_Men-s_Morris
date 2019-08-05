using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {

    private GameManager _gameManager;

    private Transform _myTransform;
    private Material _myMaterial;

    private bool _clickBool = false;
    private Stone _myStone;

    //[Header("Board Ring, order in the Ring and position in the Board matrix")]
    //[SerializeField]
    private BoardManager _myBoard;
    //[SerializeField]
    private BoardRings _ring;
    //[SerializeField]
    private int _ringOrder;
    //[SerializeField]
    private Vector2Int _myMatrixPosition;
    private List<Slot> _myNeighboursList = new List<Slot>();
    private List<Slot> _millHorizontalList = new List<Slot>();
    private List<Slot> _millVerticalList = new List<Slot>();

    
    [Header("Visuals")]
    public Color HighlightColor;
    //Not implemented ATM
    //public Color Free;
    //public Color Occupied;
    //public Color Mill;
    private Color _initialColor;

    [Header("Self-Stone Position")]
    public Transform StonePosition;
    //[SerializeField]
    private int _myStoneValue;
    
    private void Awake()
    {
        _myTransform = transform;

        if (_myTransform.GetComponentInParent<BoardManager>())
        {
            _myBoard = _myTransform.GetComponentInParent<BoardManager>();
        } else
        {
            Debug.Log("I'm missing my MotherBoard.");
        }        

        if (transform.GetComponent <MeshRenderer>() != null)
        {
            _myMaterial = transform.GetComponent<MeshRenderer>().material;
            _initialColor = _myMaterial.color;
        } else
        {
            Debug.Log("Mesh me a Renderer with a material.");
        }
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;        
    }

    //Adopts slot's visual material when chaning environment (BoardManager)
    public void SetSlotMaterial(Material slotMaterial)
    {
        transform.GetComponent<MeshRenderer>().material = slotMaterial;
    }

    //Sets Board Ring and ring Order when initializing the Board (BoardManager)
    public void SetRingAndOrder(BoardRings ring, int ringOrder)
    {
        _ring = ring;
        _ringOrder = ringOrder;        
    }

    //Not used ATM
    //public Vector2Int GetSlotMatrixPosition()
    //{
    //    return _myMatrixPosition;
    //}

    //Sets Matrix Position when initializing the Board (BoardManager)
    public void SetMyMatrixPosition(BoardManager board)
    {
        _myMatrixPosition.x = ((int)transform.localPosition.x + (int)board.transform.position.x) / 2;
        _myMatrixPosition.y = ((int)transform.localPosition.z + (int)board.transform.position.z) / 2;
    }

    //Sets Neighbour slots when initializing the Board (BoardManager)
    //Will be used to check if the stone can move to the next slot
    public void SetMyNeighbours(BoardManager board)
    {
        _myNeighboursList.Clear();
        _millHorizontalList.Clear();
        _millVerticalList.Clear();

        if (_ring == BoardRings.A)
        {
            //odd numbers: 2 connections, at the same ring
            if (_ringOrder % 2 == 1) {
                //Setting direct neighbours
                _myNeighboursList.Add(board.SlotsRingAList[MathFunctions.Modulo((_ringOrder - 1), 8)]);
                _myNeighboursList.Add(board.SlotsRingAList[MathFunctions.Modulo((_ringOrder + 1), 8)]);
                _myNeighboursList.Add(board.SlotsRingBList[_ringOrder]);

                //Setting Horizontal Mill slots
                _millHorizontalList.Add(board.SlotsRingAList[MathFunctions.Modulo((_ringOrder - 1), 8)]);
                _millHorizontalList.Add(this);
                _millHorizontalList.Add(board.SlotsRingAList[MathFunctions.Modulo((_ringOrder + 1), 8)]);

                //Setting Vertical Mill slots
                _millVerticalList.Add(this);
                _millVerticalList.Add(board.SlotsRingBList[_ringOrder]);
                _millVerticalList.Add(board.SlotsRingCList[_ringOrder]);
            }
            //even numbers and 0: 3 connections, to the next ring(s)
            else if (_ringOrder % 2 == 0)
            {
                //Setting direct neighbours
                _myNeighboursList.Add(board.SlotsRingAList[MathFunctions.Modulo((_ringOrder - 1), 8)]);
                _myNeighboursList.Add(board.SlotsRingAList[MathFunctions.Modulo((_ringOrder + 1), 8)]);

                //Setting Horizontal Mill slots
                _millHorizontalList.Add(this);
                _millHorizontalList.Add(board.SlotsRingAList[MathFunctions.Modulo((_ringOrder - 1), 8)]);
                _millHorizontalList.Add(board.SlotsRingAList[MathFunctions.Modulo((_ringOrder - 2), 8)]);

                //Setting Vertical Mill slots
                _millVerticalList.Add(this);
                _millVerticalList.Add(board.SlotsRingAList[MathFunctions.Modulo((_ringOrder + 1), 8)]);
                _millVerticalList.Add(board.SlotsRingAList[MathFunctions.Modulo((_ringOrder + 2), 8)]);
            }
        } else if (_ring == BoardRings.B)
        {
            //odd numbers: 2 connections, at the same ring
            if (_ringOrder % 2 == 1)
            {
                //Setting direct neighbours
                _myNeighboursList.Add(board.SlotsRingBList[MathFunctions.Modulo((_ringOrder - 1), 8)]);
                _myNeighboursList.Add(board.SlotsRingBList[MathFunctions.Modulo((_ringOrder + 1), 8)]);
                _myNeighboursList.Add(board.SlotsRingAList[_ringOrder]);
                _myNeighboursList.Add(board.SlotsRingCList[_ringOrder]);

                //Setting Horizontal Mill slots
                _millHorizontalList.Add(board.SlotsRingBList[MathFunctions.Modulo((_ringOrder - 1), 8)]);
                _millHorizontalList.Add(this);
                _millHorizontalList.Add(board.SlotsRingBList[MathFunctions.Modulo((_ringOrder + 1), 8)]);

                //Setting Vertical Mill slots
                _millVerticalList.Add(this);
                _millVerticalList.Add(board.SlotsRingAList[_ringOrder]);
                _millVerticalList.Add(board.SlotsRingCList[_ringOrder]);
            }
            //even numbers and 0: 3 connections, to the next ring(s)
            else if (_ringOrder % 2 == 0)
            {
                //Setting direct neighbours
                _myNeighboursList.Add(board.SlotsRingBList[MathFunctions.Modulo((_ringOrder - 1), 8)]);
                _myNeighboursList.Add(board.SlotsRingBList[MathFunctions.Modulo((_ringOrder + 1), 8)]);

                //Setting Horizontal Mill slots
                _millHorizontalList.Add(this);
                _millHorizontalList.Add(board.SlotsRingBList[MathFunctions.Modulo((_ringOrder - 1), 8)]);
                _millHorizontalList.Add(board.SlotsRingBList[MathFunctions.Modulo((_ringOrder - 2), 8)]);

                //Setting Vertical Mill slots
                _millVerticalList.Add(this);
                _millVerticalList.Add(board.SlotsRingBList[MathFunctions.Modulo((_ringOrder + 1), 8)]);
                _millVerticalList.Add(board.SlotsRingBList[MathFunctions.Modulo((_ringOrder + 2), 8)]);
            }
        } else if (_ring == BoardRings.C)
        {
            //odd numbers: 2 connections, at the same ring
            if (_ringOrder % 2 == 1)
            {
                //Setting direct neighbours
                _myNeighboursList.Add(board.SlotsRingCList[MathFunctions.Modulo((_ringOrder - 1), 8)]);
                _myNeighboursList.Add(board.SlotsRingCList[MathFunctions.Modulo((_ringOrder + 1), 8)]);
                _myNeighboursList.Add(board.SlotsRingBList[_ringOrder]);

                //Setting Horizontal Mill slots
                _millHorizontalList.Add(board.SlotsRingCList[MathFunctions.Modulo((_ringOrder - 1), 8)]);
                _millHorizontalList.Add(this);
                _millHorizontalList.Add(board.SlotsRingCList[MathFunctions.Modulo((_ringOrder + 1), 8)]);

                //Setting Vertical Mill slots
                _millVerticalList.Add(this);
                _millVerticalList.Add(board.SlotsRingAList[_ringOrder]);
                _millVerticalList.Add(board.SlotsRingBList[_ringOrder]);
            }
            //even numbers and 0: 3 connections, to the next ring(s)
            else if (_ringOrder % 2 == 0)
            {
                //Setting direct neighbours
                _myNeighboursList.Add(board.SlotsRingCList[MathFunctions.Modulo((_ringOrder - 1), 8)]);
                _myNeighboursList.Add(board.SlotsRingCList[MathFunctions.Modulo((_ringOrder + 1), 8)]);

                //Setting Horizontal Mill slots
                _millHorizontalList.Add(this);
                _millHorizontalList.Add(board.SlotsRingCList[MathFunctions.Modulo((_ringOrder - 1), 8)]);
                _millHorizontalList.Add(board.SlotsRingCList[MathFunctions.Modulo((_ringOrder - 2), 8)]);

                //Setting Vertical Mill slots
                _millVerticalList.Add(this);
                _millVerticalList.Add(board.SlotsRingCList[MathFunctions.Modulo((_ringOrder + 1), 8)]);
                _millVerticalList.Add(board.SlotsRingCList[MathFunctions.Modulo((_ringOrder + 2), 8)]);
            }
        }
    }

    //Defines what happens when a player clicks on the slot, depending on the GameState
    private void OnMouseDown()
    {
        _clickBool = !_clickBool;

        switch (_gameManager.GetGameState())
        {
            case GameState.PlayerSetup:
                if (_clickBool && _myStone == null)
                {
                    Place();
                }                
                break;
            case GameState.Move:
                if (CheckForMove() && _myStone == null)
                {
                    Move();
                }
                break;
            case GameState.Flying:
                if(CheckForFly() && _gameManager.CanPlayerFly() && _myStone == null)
                {
                    Fly();
                } else if (CheckForMove() && _myStone == null)
                {
                    Move();
                }
                break;
            case GameState.GameOver:
                break;
            case GameState.Mill:
                break;
            default:
                break;
        }
    }

    //Defines stone's placing behaviour during the Player Setup gamestate
    private void Place()
    {
        Debug.Log("Setting a stone.");
        SetTheStone(StonePosition);
        Debug.Log("Check For Mill");
        int mill = CheckForMill();
        if (mill == 3 || mill == -3)
        {
            Debug.Log("Player Setup CountDown");
            _gameManager.PlayerSetupCountdown(true);
            Debug.Log("Player made a mill.");
            _gameManager.ReportPlayerMove(mill);
        }
        else
        {
            Debug.Log("Player made a move without a mill.");

            Debug.Log("Player Setup CountDown");
            _gameManager.PlayerSetupCountdown(true);
            
            Debug.Log("Alternate Player Turn");
            _gameManager.AlternatePlayerTurn();

            Debug.Log("A1");
        }
        Debug.Log("Can Player Move On The Board?");
        _myBoard.CanPlayerMoveOnTheBoard();
    }

    //Defines stone's moving behaviour during the Move (and Flying) gamestate
    private void Move()
    {
        Debug.Log("Destroy the selected Stone on the old position");
        _gameManager.GetSelectedStoneSlot().RemoveTheStone();

        Debug.Log("Set and instantiate the stone on the old position...");
        SetTheStone(_gameManager.GetSelectedStoneSlot().StonePosition);

        Debug.Log("Move and animate the stone moving");
        _myStone.MoveTheStone(StonePosition);

        Debug.Log("Set the GameManager selected Stone and Stone Slot to null");
        _gameManager.SetSelectedStone(null);

        Debug.Log("Check For Mill");
        int mill = CheckForMill();
        if (mill == 3 || mill == -3)
        {
            Debug.Log("Player made a mill.");
            _gameManager.ReportPlayerMove(mill);
        }
        else
        {
            Debug.Log("Player made a move without a mill.");
            Debug.Log("Alternate Player Turn");
            _gameManager.AlternatePlayerTurn();
        }

        Debug.Log("Can Player Move On The Board?");
        _myBoard.CanPlayerMoveOnTheBoard();
    }

    //Defines stone's flying behaviour during the Move/Flying gamestate
    private void Fly()
    {
        Debug.Log("Destroy the selected Stone on the old position");
        _gameManager.GetSelectedStoneSlot().RemoveTheStone();

        Debug.Log("Set and instantiate the stone on the old position...");
        SetTheStone(_gameManager.GetSelectedStoneSlot().StonePosition);

        Debug.Log("Fly and animate the stone moving");
        _myStone.FlyTheStone(StonePosition);

        Debug.Log("Set the GameManager selected Stone and Stone Slot to null");
        _gameManager.SetSelectedStone(null);

        Debug.Log("Check For Mill");
        int mill = CheckForMill();
        if (mill == 3 || mill == -3)
        {
            Debug.Log("Player made a mill.");
            _gameManager.ReportPlayerMove(mill);
        }
        else
        {
            Debug.Log("Player made a move without a mill.");
            Debug.Log("Alternate Player Turn");
            _gameManager.AlternatePlayerTurn();
        }
    }

    //Defines the procedure for placing a Stone on a Slot
    private void SetTheStone(Transform positionToInstantiate)
    {
        _myMaterial.color = HighlightColor;
        Stone activePlayerStone = _gameManager.GetActivePlayerStone();
        StoneInstantiate(activePlayerStone, positionToInstantiate);
        _myStoneValue = activePlayerStone.StonePlayerValue;
        Debug.Log("Setting a stone value of " + _myStoneValue);
        _myBoard.SetBoardState(_myMatrixPosition, this);
    }

    //Defines the procedure for removing a Stone from a Slot thus cleaning the Slot
    public void RemoveTheStone()
    {
        _myMaterial.color = _initialColor;
        StoneDestroy();
        _myStoneValue = 0;
        _myBoard.SetBoardState(_myMatrixPosition, this);
    }

    //Defines the procedure for removing a Stone from a Slot with a mill (destroying and permanently removing from the board)
    public void RemoveTheStoneWithMill()
    {
        int value = _myStoneValue;
        RemoveTheStone();
        _gameManager.DecreasePlayerPieces(value);
        if (_gameManager.GetGameState() == GameState.Mill)
            _gameManager.SetMillState(false);
        _clickBool = false;
    }

    //Instantiating the stone when placing during the PlayerSetup game state, or instantiating when moving or flying to another slot
    public void StoneInstantiate(Stone stoneToPlace, Transform positionToInstantiate)
    {
        stoneToPlace = _gameManager.GetPlayerStonePrefab(stoneToPlace);

        if (!_myStone)
        {
            Quaternion rotation = stoneToPlace.ViewRotationOffset;
            Stone myStone = Instantiate(stoneToPlace, positionToInstantiate.position + stoneToPlace.ViewPositionOffset, rotation, StonePosition);
            _myStone = myStone;
        }
    }

    //Permanently destroying the stone gameobject
    public void StoneDestroy()
    {
        if (_myStone)
        {
            Destroy(_myStone.gameObject);
            _myStone = null;
        }
    }

    //Checking for free neighbours in my neighbourhood and returning 'true' if the stone can be moved to any of my neighbouring slots, and false if there are none
    //Debug logs are from a stone's perspective
    public bool CheckFreeNeighbours()
    {
        bool canIMove = false;

        Debug.Log("I " + this.name + " was asked to check my neighbours for free move.");
        for (int i = 0; i < _myNeighboursList.Count; i++)
        {
            Debug.Log("Checking neighbour " + _myNeighboursList[i].name + ".");
            if (_myNeighboursList[i].ReturnStoneValue() == 0)
            {
                Debug.Log("I can move on my neighbour " + _myNeighboursList[i].name + ".");
                canIMove = true;
            } else if (_myNeighboursList[i].ReturnStoneValue() != 0)
            {
                Debug.Log("I can not move on my neighbour " + _myNeighboursList[i].name + ", it is occupied.");
            }
        }

        if (!canIMove)
            Debug.Log("I cannot move, my neighbours are occupied!");

        return canIMove;
    }

    //Used for checking which player occupies this slot (GameManager, BoardManager, Slot)
    public int ReturnStoneValue()
    {
        return _myStoneValue;
    }
    
    //Used for checking which stone occupies this slot, and for setting the mill bool to disable removing the stones (GameManager, BoardManager, Slot)
    public Stone ReturnStone()
    {
        return _myStone;
    }

    //Checks if the stone on this slot makes a mill with other stones on the neighbouring horizontal and vertical slots (BoardManager)
    //Returns a value of 3/-3 if a player1/player2 has a mill, or 0 if it does not
    public int CheckForMill()
    {
        int millValue = 0;

        for (int i = 0; i < _millHorizontalList.Count; i++)
        {
            millValue += _millHorizontalList[i].ReturnStoneValue();
        }

        if (millValue == 3 || millValue == -3)
            return millValue;
        else
            millValue = 0;

        for (int i = 0; i < _millVerticalList.Count; i++)
        {
            millValue += _millVerticalList[i].ReturnStoneValue();
        }

        if (millValue == 3 || millValue == -3)
            return millValue;
        else
            millValue = 0;

        return millValue;
    }

    //Checks if the selected stone (GameManager) can move to this slot by checking if that stone is on one of this slot's neighbouring slots
    //during the Move (or Flying) game states
    public bool CheckForMove()
    {
        Slot selectedStoneSlot = _gameManager.GetSelectedStoneSlot();
        bool canStoneMoveHere = false;

        if (selectedStoneSlot)
        {
            for (int i = 0; i < _myNeighboursList.Count; i++)
            {
                if (selectedStoneSlot == _myNeighboursList[i])
                {
                    canStoneMoveHere = true;
                    Debug.Log("Selected Stone " + _gameManager.GetSelectedStone() + " can move here on " + this.name + "!");
                }
                else
                {
                    Debug.Log("Selected Stone " + _gameManager.GetSelectedStone() + " can not move here on " + this.name + "!");
                }
            }
        }

        return canStoneMoveHere;
    }

    //Checks if the selected stone (GameManager) can move to this slot by checking if this stone is occupied (with a debug message)
    //(together with checking if that player can fly...)
    public bool CheckForFly()
    {
        if(_myStone)
        {
            Debug.Log("Can not fly here.");
            return false;
        } else
        {
            Debug.Log("Can fly here.");
            return true;
        }
    }
}
