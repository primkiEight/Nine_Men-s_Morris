using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    PlayerSetup,
    Move,
    Flying,
    GameOver,
    Mill
}

public class GameManager : MonoBehaviour
{

    [Header("Board Prefab")]
    public BoardManager Board;

    [Header("Camera Holder")]
    public CameraController CameraHolder;

    [Header("UI Canvas")]
    public UIController UICanvas;

    [Header("Player prefabs and data")]
    public Stone Player1StonePrefab;
    public Stone Player2StonePrefab;
    private Stone _playerStone;
    public int PlayerStonePieces = 9;
    private int _totalPlayerPieces;
    private int _player1RemainingPieces;
    private int _player2RemainingPieces;

    [SerializeField]
    private Stone _selectedStone = null;
    [SerializeField]
    private Slot _selectedStoneSlot = null;

    [SerializeField]
    private GameState _currentGameState = GameState.PlayerSetup;
    //[SerializeField]
    private GameState _previousGameState;

    public GameState GetGameState()
    {
        return _currentGameState;
    }

    private enum PlayerTurn
    {
        Player1,
        Player2
    }
    [SerializeField]
    private PlayerTurn _currentPlayer;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one GameManager!");
            return;
        }
        Instance = this;

        Debug.Log("GameManager awakes!");
    }

    private void Start()
    {
        _currentPlayer = PlayerTurn.Player1;
        _playerStone = Player1StonePrefab;
        _totalPlayerPieces = PlayerStonePieces * 2;

        _player1RemainingPieces = PlayerStonePieces;
        _player2RemainingPieces = PlayerStonePieces;

        _currentGameState = GameState.PlayerSetup;
        _previousGameState = _currentGameState;

        Debug.Log("BoardInitializing");
        Board.InitializeBoard();

        UICanvas.UpdateUIPlayerNames();
        UICanvas.UpdateUIMessage(UICanvas.Language.PlayerSetupMessage);
        UICanvas.UpdateUIAwake(true);
    }

    public void AlternatePlayerTurn()
    {
        if (_currentPlayer == PlayerTurn.Player1)
        {
            _currentPlayer = PlayerTurn.Player2;
            _playerStone = Player2StonePrefab;
            CameraHolder.CameraViewAlternate();
            UICanvas.UpdateUIAlternateScreens();
            Debug.Log("Player2 Turn");
        }
        else if (_currentPlayer == PlayerTurn.Player2)
        {
            _currentPlayer = PlayerTurn.Player1;
            _playerStone = Player1StonePrefab;
            CameraHolder.CameraViewAlternate();
            UICanvas.UpdateUIAlternateScreens();
            Debug.Log("Player1 Turn");
        }

        if (_currentGameState == GameState.PlayerSetup)
            UICanvas.UpdateUIMessage(UICanvas.Language.PlayerSetupMessage);
        if (_currentGameState == GameState.Move)
            UICanvas.UpdateUIMessage(UICanvas.Language.PlayerMoveMessage);
    }

    public Stone GetActivePlayerStone()
    {
        return _playerStone;
    }

    public Stone GetSelectedStone()
    {
        return _selectedStone;
    }

    public Slot GetSelectedStoneSlot()
    {
        return _selectedStoneSlot;
    }

    public void SetSelectedStone(Stone selectedStone)
    {
        if(selectedStone == null)
        {
            _selectedStone = selectedStone;
            _selectedStoneSlot = null;
        }   
        else
        {
            if ((selectedStone.StonePlayerValue == 1 && _currentPlayer == PlayerTurn.Player1)
            || (selectedStone.StonePlayerValue == -1 && _currentPlayer == PlayerTurn.Player2))
            {
                _selectedStone = selectedStone;
                _selectedStoneSlot = selectedStone.transform.parent.GetComponentInParent<Slot>();
            }   
        }
    }

    public void PlayerSetupCountdown(bool countdown)
    {
        if (_currentGameState == GameState.PlayerSetup)
        {
            if (countdown)
            {
                _totalPlayerPieces--;
                Debug.Log("Player Pieces -1");
                //postoji zbog Undo opcije, odnosno jer trenutno omogućujem da igrač povuće potez
            }
            else
            {
                _totalPlayerPieces++;
                Debug.Log("Player Pieces +1");
                //Alternate se poziva (trenutno) is Slot klase
                //AlternatePlayerTurn();
            }

            if (_totalPlayerPieces == 0)
            {
                //_currentPlayer = PlayerTurn.Player1;
                ChangeGameState();
                Debug.Log("GameState is now Move");
            }   
        }
    }

    public void DecreasePlayerPieces(int value)
    {
        if (value == 1)
        {
            _player1RemainingPieces--;            
        } else if (value == -1)
        {
            _player2RemainingPieces--;
        }

        UICanvas.UpdateUIStones(_player1RemainingPieces, _player2RemainingPieces);

        if (_player1RemainingPieces == 2)
        {
            Debug.Log("Player 2 Wins!");
            UICanvas.UpdateUIMessage(UICanvas.Language.Player2 + UICanvas.Language.PlayerWin);
            _currentGameState = GameState.GameOver;
            GameOver();
        } else if (_player2RemainingPieces == 2)
        {
            Debug.Log("Player 1 Wins!");
            UICanvas.UpdateUIMessage(UICanvas.Language.Player1 + UICanvas.Language.PlayerWin);
            _currentGameState = GameState.GameOver;
            GameOver();
        } else if (_player1RemainingPieces == 3 && _player2RemainingPieces == 3)
        {
            Debug.Log("It's a draw!");
            UICanvas.UpdateUIMessage(UICanvas.Language.PlayerDraw);
            _currentGameState = GameState.GameOver;
            GameOver();
        }
    }

    public void ChangeGameState()
    {
        _currentGameState++;

        //if(_currentGameState == GameState.Move)
        //    _currentPlayer = PlayerTurn.Player1;

        //AlternatePlayerTurn();

        if (_currentGameState == GameState.GameOver)
        {
            GameOver();
            Debug.Log("GameState is now GameOver");
        }
    }

    private void GameOver()
    {

    }

    public void ReportPlayerCannotMove(bool player1, bool player2)
    {
        if (_currentGameState != GameState.PlayerSetup)
        {
            if (!player1 && player2)
            {
                UICanvas.UpdateUIMessage(UICanvas.Language.Player1 + UICanvas.Language.PlayerNoMovesWin01 + UICanvas.Language.Player2 + UICanvas.Language.PlayerNoMovesWin02);
                Debug.Log("Player 1 cannot make any more moves, Player 2 wins!");
            }
            if (!player2 && player1)
            {
                UICanvas.UpdateUIMessage(UICanvas.Language.Player2 + UICanvas.Language.PlayerNoMovesWin01 + UICanvas.Language.Player1 + UICanvas.Language.PlayerNoMovesWin02);
                Debug.Log("Player 2 cannot make any more moves, Player 1 wins!");
            }   
            if (!player1 && !player2)
                Debug.Log("Players cannot make any more moves, it's a draw!");

            _currentGameState = GameState.GameOver;
            GameOver();
            Debug.Log("GameState is now GameOver");
        }
    }

    public void ReportPlayerMove(int millCondition)
    {
        if(millCondition == 3)
        {
            Debug.Log("Player 1 made a mill. Changeing GameStatus to Mill.");
            SetMillState(true);
        } else if (millCondition == -3)
        {
            Debug.Log("Player 2 made a mill. Changeing GameStatus to Mill.");
            SetMillState(true);
        }
    }

    public void SetMillState(bool state)
    {
        if (state){
            _previousGameState = _currentGameState;
            _currentGameState = GameState.Mill;
            UICanvas.UpdateUIMessage(UICanvas.Language.PlayerMillMessage01);
        } else {
            _currentGameState = _previousGameState;
            
            AlternatePlayerTurn();
        }
    }
}
