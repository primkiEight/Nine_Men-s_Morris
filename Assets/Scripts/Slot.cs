using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {

    private bool _clickBool = false;

    private GameManager _gameManager;

    private Transform _myTransform;
    private Material _myMaterial;

    public Stone TestStonePrefab;
    private Stone _myStone;

    [Header("Board Ring, order in the Ring and position in the Board matrix")]
    [SerializeField]
    private BoardManager _myBoard;
    [SerializeField]
    private BoardRings _ring;
    [SerializeField]
    private int _ringOrder;
    [SerializeField]
    private Vector2Int _myMatrixPosition;
    public List<Slot> _myNeighboursList = new List<Slot>();
    public List<Slot> _millHorizontalList = new List<Slot>();
    public List<Slot> _millVerticalList = new List<Slot>();

    private Color _initialColor;
    [Header("Visuals")]
    public Color HighlightColor;

    [Header("Positions")]
    public Transform StonePosition;
    [SerializeField]
    private int _myStoneValue;
    
    private void Awake()
    {
        _myTransform = transform;

        if (_myTransform.GetComponentInParent<BoardManager>())
        {
            _myBoard = _myTransform.GetComponentInParent<BoardManager>();
        } else
        {
            Debug.Log("I'm missing my MotherBoard");
        }        

        if (transform.GetComponent <MeshRenderer>() != null)
        {
            _myMaterial = transform.GetComponent<MeshRenderer>().material;
            _initialColor = _myMaterial.color;
        }
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;        
    }

    public void SetRingAndOrder(BoardRings ring, int ringOrder)
    {
        _ring = ring;
        _ringOrder = ringOrder;        
    }

    public Vector2Int GetSlotMatrixPosition()
    {
        return _myMatrixPosition;
    }

    public void SetMyMatrixPosition(BoardManager board)
    {
        _myMatrixPosition.x = ((int)transform.localPosition.x + (int)board.transform.position.x) / 2;
        _myMatrixPosition.y = ((int)transform.localPosition.z + (int)board.transform.position.z) / 2;
    }

    public void SetMyNeighbours(BoardManager board)
    {
        //_myNeighboursList = new List<Slot>();
        //_millHorizontalList = new List<Vector2Int>();
        //_millVerticalList = new List<Vector2Int>();
        //
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

    private void OnMouseDown()
    {
        _clickBool = !_clickBool;

        switch (_gameManager.GetGameState())
        {
            case GameState.PlayerSetup:
                if (_clickBool && _myStone == null)
                {
                    Debug.Log("Setting a stone.");
                    SetTheStone(StonePosition);
                    //Debug.Log("Alternate Player Turn");
                    //_gameManager.AlternatePlayerTurn();
                    //Debug.Log("Player Setup CountDown");
                    //_gameManager.PlayerSetupCountdown(true);
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
                        Debug.Log("Alternate Player Turn");
                        _gameManager.AlternatePlayerTurn();

                        Debug.Log("A1");
                    }
                    Debug.Log("Player Setup CountDown");
                    _gameManager.PlayerSetupCountdown(true);
                    Debug.Log("Can Player Move On The Board?");
                    _myBoard.CanPlayerMoveOnTheBoard();                    
                }
                else
                {
                    //if (_gameManager.GetActivePlayerStone() == this)
                    //{
                    //    //Po pravilima ovo bi se trebalo onemogućiti
                    //    Debug.Log("Removing a stone.");
                    //    RemoveTheStone();
                    //    //Debug.Log("Alternate Player Turn");
                    //    //_gameManager.AlternatePlayerTurn();
                    //    Debug.Log("Player Setup CountDown");
                    //    _gameManager.PlayerSetupCountdown(false);
                    //}
                }
                break;
            case GameState.Move:
                if (CheckForMove())
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
                    if(mill == 3 || mill == -3)
                    {
                        Debug.Log("Player made a mill.");
                        _gameManager.ReportPlayerMove(mill);
                    } else
                    {
                        Debug.Log("Player made a move without a mill.");
                        Debug.Log("Alternate Player Turn");
                        _gameManager.AlternatePlayerTurn();
                    }

                    Debug.Log("Can Player Move On The Board?");
                    _myBoard.CanPlayerMoveOnTheBoard();
                }

                break;
            case GameState.Flying:
                break;
            case GameState.GameOver:
                break;
            case GameState.Mill:
                break;
            default:
                break;
        }
    }

    private void SetTheStone(Transform positionToInstantiate)
    {
        _myMaterial.color = HighlightColor;
        Stone activePlayerStone = _gameManager.GetActivePlayerStone();
        StoneInstantiate(activePlayerStone, positionToInstantiate);
        _myStoneValue = activePlayerStone.StonePlayerValue;
        Debug.Log("Setting a stone value of " + _myStoneValue);
        //_gameManager.Board.SetBoardState(_myMatrixPosition, this);
        _myBoard.SetBoardState(_myMatrixPosition, this);
    }

    public void RemoveTheStone()
    {
        _myMaterial.color = _initialColor;
        StoneDestroy();
        _myStoneValue = 0;
        //_gameManager.Board.SetBoardState(_myMatrixPosition, this);        
        _myBoard.SetBoardState(_myMatrixPosition, this);

        //if (_gameManager.GetGameState() == GameState.Mill)
        //{
        //    _gameManager.SetMillState(false);
        //}
    }

    public void RemoveTheStoneWithMill()
    {
        int value = _myStoneValue;
        RemoveTheStone();
        _gameManager.DecreasePlayerPieces(value);
        if (_gameManager.GetGameState() == GameState.Mill)
            _gameManager.SetMillState(false);
    }

    public void StoneInstantiate(Stone stoneToPlace, Transform positionToInstantiate)
    {
        if (!_myStone)
        {
            Stone myStone = Instantiate(stoneToPlace, positionToInstantiate.position, Quaternion.identity, StonePosition);
            _myStone = myStone;
        }
    }

    public void StoneDestroy()
    {
        if (_myStone)
        {
            Destroy(_myStone.gameObject);
            _myStone = null;
        }
    }

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

    public int ReturnStoneValue()
    {
        return _myStoneValue;
    }

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
}
