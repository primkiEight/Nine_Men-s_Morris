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
    [Header("Environment Data")]
    public Transform EnvironmentParent;
    public Environment AncientEnvironmentData;
    public Environment MedievalEnvironmentData;
    private Environment _environmentData;

    private GameObject _medievalEnvironment;
    private GameObject _ancientEnvironment;

    [Header("Board Prefab")]
    public BoardManager Board;

    [Header("Camera Holder")]
    public CameraController CameraHolder;

    [Header("UI Canvas")]
    public UIController UICanvas;

    [Header("Player prefabs and data")]
    private Stone _player1StonePrefab;
    private Stone _player2StonePrefab;
    private Stone _playerStone = null;
    private Stone _flyingPlayer;
    public int PlayerStonePieces = 9;
    private int _totalPlayerPieces;
    private int _player1RemainingPieces;
    private int _player1PlacedPieces;
    private int _player2RemainingPieces;
    private int _player2PlacedPieces;

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

        _medievalEnvironment = Instantiate(MedievalEnvironmentData.EnvironmentGroup, EnvironmentParent.transform.position, Quaternion.identity, EnvironmentParent.transform) as GameObject;
        _ancientEnvironment = Instantiate(AncientEnvironmentData.EnvironmentGroup, EnvironmentParent.transform.position, Quaternion.identity, EnvironmentParent.transform) as GameObject;

        _ancientEnvironment.SetActive(false);

        _environmentData = AncientEnvironmentData;
    }

    public void ApplyEnvironment()
    {
        if (_environmentData == MedievalEnvironmentData)
        {
            _environmentData = AncientEnvironmentData;
            _medievalEnvironment.SetActive(false);
            _ancientEnvironment.SetActive(true);
        } else
        {
            _environmentData = MedievalEnvironmentData;
            _medievalEnvironment.SetActive(true);
            _ancientEnvironment.SetActive(false);            
        }
                
        _player1StonePrefab = _environmentData.StoneP1Prefab;
        _player2StonePrefab = _environmentData.StoneP2Prefab;

        Board.ApplySlotMaterial(_environmentData.Slots);

        UICanvas.ChangeFont(_environmentData.Font);
        UICanvas.ChangeImageSprites(_environmentData.PlayerImage, _environmentData.MessageImage);
    }
    
    private void Start()
    {
        Debug.Log("BoardInitializing");
        Board.InitializeBoard();

        ApplyEnvironment();

        _currentPlayer = PlayerTurn.Player1;
        _playerStone = _player1StonePrefab;
        _totalPlayerPieces = PlayerStonePieces * 2;

        _player1RemainingPieces = PlayerStonePieces;
        _player2RemainingPieces = PlayerStonePieces;
        _player1PlacedPieces = 0;
        _player2PlacedPieces = 0;

        _currentGameState = GameState.PlayerSetup;
        _previousGameState = _currentGameState;

        UICanvas.UpdateUIPlayerNames();
        UICanvas.UpdateUIMessage(UICanvas.Language.PlayerSetupMessage);
        UICanvas.UpdateUIAwake(true);
    }

    public void AlternatePlayerTurn()
    {
        if (_currentGameState == GameState.PlayerSetup)
            UICanvas.UpdateUIMessage(UICanvas.Language.PlayerSetupMessage);
        if (_currentGameState == GameState.Move)
            UICanvas.UpdateUIMessage(UICanvas.Language.PlayerMoveMessage);
        if (_currentGameState == GameState.Flying)
            UICanvas.UpdateUIMessage(UICanvas.Language.PlayerFlyMessage);

        Debug.Log("Can Player Move On The Board?");
        Board.CanPlayerMoveOnTheBoard();

        if (_currentPlayer == PlayerTurn.Player1)
        {
            _currentPlayer = PlayerTurn.Player2;
            _playerStone = _player2StonePrefab;
            CameraHolder.CameraViewAlternate();
            UICanvas.UpdateUIAlternateScreens();
            Debug.Log("Player2 Turn");
        }
        else if (_currentPlayer == PlayerTurn.Player2)
        {
            _currentPlayer = PlayerTurn.Player1;
            _playerStone = _player1StonePrefab;
            CameraHolder.CameraViewAlternate();
            UICanvas.UpdateUIAlternateScreens();
            Debug.Log("Player1 Turn");
        }
    }

    public Stone GetActivePlayerStone()
    {
        return _playerStone;
    }

    public void SetFlyingPlayerStone(Stone flyingPlayer)
    {
        _flyingPlayer = flyingPlayer;
    }

    public Stone GetFlyingPlayerStone()
    {
        return _flyingPlayer;
    }

    public bool CanPlayerFly()
    {
        if(GetActivePlayerStone() == _flyingPlayer)
            return true;
        return false;
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

                if(_currentPlayer == PlayerTurn.Player1)
                {
                    _player1RemainingPieces--;
                    _player1PlacedPieces++;
                } else
                {
                    _player2RemainingPieces--;
                    _player2PlacedPieces++;
                }

                UICanvas.UpdateUIStones(_player1RemainingPieces, _player2RemainingPieces);
            }
            else
            {
                //Not an option now.
                _totalPlayerPieces++;
                Debug.Log("Player Pieces +1");                
            }

            if (_totalPlayerPieces == 0)
            {
                _player1RemainingPieces = _player1PlacedPieces;
                _player2RemainingPieces = _player2PlacedPieces;
                UICanvas.UpdateUIStones(_player1RemainingPieces, _player2RemainingPieces);
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


        //////OVO direktno ispod provjera JE ZADNJE ŠTO SAM DODAO, PROVJERA DA NISU U PLAYER SETUPU
        //Ne znam hoće li sada igra moći završiti kada posljednji igrač ostane na 3 ili 2 kamena nakon faze postavljanja
        if (_previousGameState != GameState.PlayerSetup)
        {
            if (_player1RemainingPieces == 2)
            {
                Debug.Log("Player 2 Wins!");
                UICanvas.UpdateUIMessage(UICanvas.Language.Player2 + UICanvas.Language.PlayerWin);
                _currentGameState = GameState.GameOver;
                GameOver();
            }
            else if (_player2RemainingPieces == 2)
            {
                Debug.Log("Player 1 Wins!");
                UICanvas.UpdateUIMessage(UICanvas.Language.Player1 + UICanvas.Language.PlayerWin);
                _currentGameState = GameState.GameOver;
                GameOver();
            }
            else if (_player1RemainingPieces == 3 || _player2RemainingPieces == 3)
            {
                if (_flyingPlayer == null)
                {
                    if (_player1RemainingPieces == 3)
                    {
                        Debug.Log("Player1 can now fly!");
                        SetFlyingPlayerStone(_player1StonePrefab);
                    }
                    else if (_player2RemainingPieces == 3)
                    {
                        Debug.Log("Player2 can now fly!");
                        SetFlyingPlayerStone(_player2StonePrefab);
                    }

                    //ChangeGameState();
                    _currentGameState = GameState.Flying;
                    AlternatePlayerTurn();
                }
            }
            
            if (_player1RemainingPieces == 3 && _player2RemainingPieces == 3)
            {
                Debug.Log("It's a draw!");
                UICanvas.UpdateUIMessage(UICanvas.Language.PlayerDraw);
                _currentGameState = GameState.GameOver;
                GameOver();
            }
        }
    }

    public void ChangeGameState()
    {
        _currentGameState++;

        if (_currentGameState == GameState.Move)
        {
            UICanvas.UpdateUIMessage(UICanvas.Language.PlayerMoveMessage);
        }

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
