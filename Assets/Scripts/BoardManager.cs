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

    public Transform SlotsParentGameObject;

    [SerializeField]
    private int[,] CurrentStonePositionsOnTheBoardMatrix = new int[7, 7];
    [SerializeField]
    //private Slot[,] SlotsWithPlayerStones = new Slot[7, 7];

    private void Awake()
    {
        Debug.Log("BoardManager awakes!");
    }

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

        AllSlots = new List<Slot>();

        Debug.Log("Updating AllSlotsList...");
        UpdateAllSlotsList();

    }

    private void UpdateAllSlotsList()
    {
        AllSlots.Clear();

        foreach (Transform child in SlotsParentGameObject)
        {
            AllSlots.Add(child.gameObject.GetComponent<Slot>());
        }

        //for (int i = 0; i < SlotsRingAList.Count; i++)
        //{
        //    AllSlots.Add(SlotsRingAList[i]);
        //}
        //
        //for (int i = 0; i < SlotsRingBList.Count; i++)
        //{
        //    AllSlots.Add(SlotsRingBList[i]);
        //}
        //
        //for (int i = 0; i < SlotsRingCList.Count; i++)
        //{
        //    AllSlots.Add(SlotsRingCList[i]);
        //}
    }

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

    public void SetBoardState(Vector2Int boardMatrixPosition, Slot slot)
    {
        int x = boardMatrixPosition.x;
        int y = boardMatrixPosition.y;

        Debug.Log("Setting Board state and rewriting matrix element.");
        CurrentStonePositionsOnTheBoardMatrix[x, y] = slot.ReturnStoneValue();
        //Debug.Log("ovdje mi je stone value" + slot.ReturnStoneValue());
        //SlotsWithPlayerStones[x, y] = slot;

        UpdateAllSlotsList();

        //CanPlayerMoveOnTheBoard();

        PrintBoardState();
    }

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
}
