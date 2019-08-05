using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardRings { A, B, C };

public class BoardManager : MonoBehaviour {

    [Header("List of Slots on the three board rings")]
    public List<Slot> SlotsRingAList = new List<Slot>();
    public List<Slot> SlotsRingBList = new List<Slot>();
    public List<Slot> SlotsRingCList = new List<Slot>();
    public List<Slot> AllSlots = new List<Slot>();

    [Header("Parent For All The Slots")]
    public Transform SlotsParentGameObject;

    //Only for DebugLog visualization ATM
    //TODO: Undo system :)
    private int[,] CurrentStonePositionsOnTheBoardMatrix = new int[7, 7];
    
    private int counterMill = 0;
    private int _counterBoard = 0;

    private void Awake()
    {
        Debug.Log("BoardManager awakes!");
    }

    //Sets all the properties of the slots
    //Hard coded board and slots logic
    public void InitializeBoard()
    {
        Debug.Log("Initializin Board Matrix...");
        InitializeBoardMatrix();

        //Set A slot Ring, RingOrder and Matrix Position
        Debug.Log("Setting Board A Ring...");
        for (int i = 0; i < SlotsRingAList.Count; i++)
        {
            SlotsRingAList[i].SetRingAndOrder(BoardRings.A, i);
            SlotsRingAList[i].SetMyMatrixPosition(this);
        }

        //Set B slot Ring, RingOrder and Matrix Position
        Debug.Log("Setting Board B Ring...");
        for (int i = 0; i < SlotsRingBList.Count; i++)
        {
            SlotsRingBList[i].SetRingAndOrder(BoardRings.B, i);
            SlotsRingBList[i].SetMyMatrixPosition(this);
        }

        //Set C slot Ring, RingOrder and Matrix Position
        Debug.Log("Setting Board C Ring...");
        for (int i = 0; i < SlotsRingCList.Count; i++)
        {
            SlotsRingCList[i].SetRingAndOrder(BoardRings.C, i);
            SlotsRingCList[i].SetMyMatrixPosition(this);
        }

        //Set A slots' neighbours
        Debug.Log("Setting A Ring slots' neighbours...");
        for (int i = 0; i < SlotsRingAList.Count; i++)
        {
            SlotsRingAList[i].SetMyNeighbours(this);
        }

        //Set B slots' neighbours
        Debug.Log("Setting B Ring slots' neighbours...");
        for (int i = 0; i < SlotsRingBList.Count; i++)
        {
            SlotsRingBList[i].SetMyNeighbours(this);
        }

        //Set C slots' neighbours
        Debug.Log("Setting C Ring slots' neighbours...");
        for (int i = 0; i < SlotsRingCList.Count; i++)
        {
            SlotsRingCList[i].SetMyNeighbours(this);
        }

        //One (easy) way of getting to all the slots, for any purpose
        AllSlots = new List<Slot>();

        Debug.Log("Updating AllSlotsList...");
        UpdateAllSlotsList();
    }

    //Adding the slots from the project hierarchy
    //For some reason the GameManager was not responding to the objects (need research)
    private void UpdateAllSlotsList()
    {
        AllSlots.Clear();

        foreach (Transform child in SlotsParentGameObject)
        {
            AllSlots.Add(child.gameObject.GetComponent<Slot>());
        }
    }

    //Only for DebugLog visualization ATM
    //TODO: Undo system :)
    public void InitializeBoardMatrix()
    {
        for (int i = 0; i < CurrentStonePositionsOnTheBoardMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < CurrentStonePositionsOnTheBoardMatrix.GetLength(1); j++)
            {
                CurrentStonePositionsOnTheBoardMatrix[i, j] = 0;
            }
        }

        PrintBoardState();
    }

    //Only for DebugLog visualization ATM
    //TODO: Undo system :)
    //(Slot)
    public void SetBoardState(Vector2Int boardMatrixPosition, Slot slot)
    {
        int x = boardMatrixPosition.x;
        int y = boardMatrixPosition.y;

        Debug.Log("Setting Board state and rewriting matrix element.");
        CurrentStonePositionsOnTheBoardMatrix[x, y] = slot.ReturnStoneValue();
        
        UpdateAllSlotsList();

        PrintBoardState();
    }

    //Only for DebugLog visualization ATM
    //TODO: Undo system :)
    private void PrintBoardState()
    {
        string boardState = "";

        boardState += "\n";

        for (int j = CurrentStonePositionsOnTheBoardMatrix.GetLength(0) - 1; j >= 0; j--)
        {
            for (int i = 0; i < CurrentStonePositionsOnTheBoardMatrix.GetLength(1); i++)
            {
                boardState += CurrentStonePositionsOnTheBoardMatrix[i, j];
                boardState += "-";
            }
            boardState += "\n";
        }

        Debug.Log(boardState);
    }

    //Checking if a playe can make a move on the board (GameManager, Slot)
    //If not, the game ends for that player
    public void CanPlayerMoveOnTheBoard()
    {
        bool player1Move = false;
        bool player2Move = false;
        
        for (int i = 0; i < AllSlots.Count; i++)
        
        if (AllSlots[i] != null)
            {
            if (AllSlots[i].ReturnStoneValue() > 0)
            {
                if (AllSlots[i].CheckFreeNeighbours())
                {
                    Debug.Log("Player 1 can move.");
                    player1Move = true;
                }   
            }
            else if (AllSlots[i].ReturnStoneValue() < 0)
            {
                if (AllSlots[i].CheckFreeNeighbours())
                {
                    Debug.Log("Player 2 can move.");
                    player2Move = true;
                }   
            }
            else if (AllSlots[i].ReturnStoneValue() == 0)
            {
                //Debug.Log("Stone value is 0");
            }            
        }

        if(player1Move)
            Debug.Log("Player 1 can make a move.");
        else
            Debug.Log("Player 1 can not make any more moves.");

        if (player2Move)
            Debug.Log("Player 2 can make a move.");
        else
            Debug.Log("Player 2 can not make any more moves.");

        if (player1Move == false || player2Move == false)
            GameManager.Instance.ReportPlayerCannotMove(player1Move, player2Move);
    }

    //Adopts slot's visual material when changing the environment (GameManager)
    //Getting the child objects of the slot parent
    public void ApplySlotMaterial(Material slotMaterial)
    {
        foreach (Transform child in SlotsParentGameObject)
        {
            child.GetComponent<Slot>().SetSlotMaterial(slotMaterial);
        }
    }

    //Checking if stones on the board can be destroyed
    //Clicking a stone (Stone) starts the method
    public bool CanThisStoneBeDestroyed(Stone askingForAStone)
    {
        Debug.Log("Checking if the stones can be destroyed...");

        counterMill = 0;
        _counterBoard = 0;

        //Chacking all the slots on the board
        for (int i = 0; i < AllSlots.Count; i++)
        {
            //If a stone in this slot is the same as the recieved opponent stone
            //(the stone that was clicked on)
            if (AllSlots[i].ReturnStoneValue() == askingForAStone.StonePlayerValue)
            {
                //check if it is in a mill
                if (AllSlots[i].CheckForMill() == (3 * askingForAStone.StonePlayerValue))
                {
                    //If it is, mark the stone as undestroyable
                    AllSlots[i].ReturnStone().SetStoneMillBool(true);
                    //and increase the counter of the stones
                    counterMill++;
                    Debug.Log("CheckForMill: " + AllSlots[i].CheckForMill());
                    Debug.Log("Counter: " + counterMill);
                    Debug.Log("Counter Board: " + counterMill);
                } else
                {
                    //This stone is not in a mill, thus it can be destroyed
                    AllSlots[i].ReturnStone().SetStoneMillBool(false);
                    //and the counter is not increased
                }
            }
        }

        for (int i = 0; i < AllSlots.Count; i++)
        {
            if (AllSlots[i].ReturnStoneValue() == askingForAStone.StonePlayerValue)
                _counterBoard++;
        }

        Debug.Log("Counter: " + counterMill);
        Debug.Log("Counter Board: " + _counterBoard);

        if (counterMill == _counterBoard)
        {
            //You can destory any stone since all of them are in a mill
            //(the same number of stones on the board as of the stones that are in a mill
            Debug.Log("All stones are in a mill, you can destroy any stone.");

            GameManager.Instance.UICanvas.UpdateUIMessage(GameManager.Instance.UICanvas.Language.PlayerMillMessage03);
            
            return true;
        } else
        {
            //You can destroy only those stones that are not in a mill,
            //others will be marked and check as not destroyable
            Debug.Log("Not all stones are in a mill, you can destroy only those that are not in a mill.");

            GameManager.Instance.UICanvas.UpdateUIMessage(GameManager.Instance.UICanvas.Language.PlayerMillMessage02);

            return false;
        }
    }
}
