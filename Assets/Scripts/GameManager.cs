using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* PlayerSetup: Players alternately place their stones, mills are possible
 * (3 stones in a row allow a player to destroy other player's stone which is not in a mill formation (unless no other stones are available to destroy)
 * Move: Players alternately move stones on the board (on the next free slot) trying to form a mill - 3 stones in a row
 * Flying: A player that is left on only 3 stones on the board can 'fly' to any other free space on the board
 * GameOver: The game ends if one of the players is left with two stones on the board, if a player can not move to any adjacent space,
 * or if both players have 3 stones on the board (a draw)
 * Mill: game state when one player has placed 3 stones in a row, and now can take/remove an opponent's stone
 */
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

    [Header("Number Of Player Pieces (Stones) = 9")]
    public int PlayerStonePieces = 9;
    private int _totalPlayerPieces;
    private int _player1RemainingPieces;
    private int _player1PlacedPieces;
    private int _player2RemainingPieces;
    private int _player2PlacedPieces;

    //[SerializeField]
    private Stone _selectedStone = null;
    //[SerializeField]
    private Slot _selectedStoneSlot = null;

    //[SerializeField]
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
    //[SerializeField]
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

    //Applies all the environment changes (the UI and the scenery)
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

        CameraHolder.UpdatePostProcessingProfile(_environmentData.Profile);

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

    //Alternates the player turns, updating the UI, moving the camera and checking if the players can move on the board
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

    //Allows a Stone and a Slot to access the active player stone's reference
    public Stone GetActivePlayerStone()
    {
        return _playerStone;
    }

    //Defines which player can fly, so that others know when checking the move
    public void SetFlyingPlayerStone(Stone flyingPlayer)
    {
        _flyingPlayer = flyingPlayer;
    }

    //Checking (when flying) if the active player can fly to other slots on the board (Slot)
    public bool CanPlayerFly()
    {
        if(GetActivePlayerStone() == _flyingPlayer)
            return true;
        return false;
    }

    //Allows a Stone and a a Slot to access the selected stone's reference
    public Stone GetSelectedStone()
    {
        return _selectedStone;
    }

    //Allows a Slot to access the selected stone's slot reference
    public Slot GetSelectedStoneSlot()
    {
        return _selectedStoneSlot;
    }

    //Sets which player's stone is the selected stone (when moving and flying) (Slot, Stone)
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

    //Allowing the Slot to use the right stone's prefab for instantiate
    public Stone GetPlayerStonePrefab(Stone lookingForThisStonePrefab)
    {
        if (lookingForThisStonePrefab.StonePlayerValue == 1)
        {
            return _player1StonePrefab;
        }
        else if (lookingForThisStonePrefab.StonePlayerValue == -1)
        {
            return _player2StonePrefab;
        }

        return null;
    }

    //The countdown procedure for PlayerSetup game state (keeping track of the right numbers, and updating UI)
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
                ChangeGameState();
                Debug.Log("GameState is now Move");

                _player1RemainingPieces = 0;
                _player2RemainingPieces = 0;

                for (int i = 0; i < Board.AllSlots.Count; i++)
                {
                    if (Board.AllSlots[i].ReturnStoneValue() == 1)
                        _player1RemainingPieces++;
                    else if (Board.AllSlots[i].ReturnStoneValue() == -1)
                        _player2RemainingPieces++;
                }
                UICanvas.UpdateUIStones(_player1RemainingPieces, _player2RemainingPieces);
            }   
        }
    }

    //The countdown procedure for Mill game state (keeping track of the right numbers, and updating UI)
    //Ending the game under the righ conditions
    //Defining when a player can fly
    public void DecreasePlayerPieces(int value)
    {
        if (_previousGameState == GameState.PlayerSetup)
        {
            if (value == 1)
            {
                _player1PlacedPieces--;
            }
            else if (value == -1)
            {
                _player2PlacedPieces--;
            }

            UICanvas.UpdateUIStones(_player1RemainingPieces, _player2RemainingPieces);

            if ((_player1RemainingPieces == 0 && _player2RemainingPieces == 0) && (_player1PlacedPieces == 3 && _player2PlacedPieces == 3))
            {
                Debug.Log("It's a draw!");
                UICanvas.UpdateUIMessage(UICanvas.Language.PlayerDraw);
                _currentGameState = GameState.GameOver;
                GameOver();
            }
        }
        else if (_previousGameState != GameState.PlayerSetup)
        {
            if (_currentGameState == GameState.Mill)
                {
                if (value == 1)
                {
                    _player1RemainingPieces--;                    
                }
                else if (value == -1)
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
    }

    //Changes the Game State
    //Initially planned to be used as an incrementing method, but this is true only for the first transition from PlayerSetup
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

    //Defines what happens when the game ends
    //For now, only the UI update and camera rotation
    private void GameOver()
    {
        UICanvas.UpdateUIGameOverScreen();
        CameraHolder.ActivateGameOverCamera();
    }

    //Trackes if a player cannot move anymore on the board, updates the UI and ends the game (BoardManager)
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

    //Activates the Mill game state and the UI together with the SetMillState method bellow (Slot)
    //The mill game state ends and returns to the previous state when an opponent's stone is removed
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
