using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Environment Set", menuName = "New Environment Set/Environment Set", order = 1)]
public class Environment : ScriptableObject {

    public Font Font;

    public Material Slots;

    public Stone StoneP1Prefab;
    public Stone StoneP2Prefab;

    public GameObject EnvironmentGroup;

    public Sprite PlayerImage;
    public Sprite MessageImage;
}
