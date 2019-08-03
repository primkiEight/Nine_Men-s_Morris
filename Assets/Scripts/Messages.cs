using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Message Board", menuName = "New Message Board/Language", order = 1)]
public class Messages : ScriptableObject {

    [Header("Player Names")]
    public string Player1;
    public string Player2;

    [Header("Game Over Screen")]
    public string PlayerWin;
    public string PlayerNoMovesWin01;
    public string PlayerNoMovesWin02;
    public string PlayerDraw;

    [Header("Player Setup Messages")]
    public string PlayerSetupMessage;

    [Header("Player Move Messages")]
    public string PlayerMoveMessage;
    
    [Header("Mill Messages")]
    public string PlayerMillMessage01;
    public string PlayerMillMessage02;
}