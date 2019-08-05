using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

[CreateAssetMenu(fileName = "New Environment Set", menuName = "New Environment Set/Environment Set", order = 1)]
public class Environment : ScriptableObject {

    [Header("UI Display Font")]
    public Font Font;

    [Header("Slot Mash Material")]
    public Material Slots;

    [Header("Player Stones Prefabs")]
    public Stone StoneP1Prefab;
    public Stone StoneP2Prefab;

    [Header("Environment Scenery - Prefab")]
    public GameObject EnvironmentGroup;

    [Header("PostProcessing Profile")]
    public PostProcessingProfile Profile;

    [Header("UI Display Images")]
    public Sprite PlayerImage;
    public Sprite MessageImage;
}
