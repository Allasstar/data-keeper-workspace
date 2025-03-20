using System.Collections.Generic;
using DataKeeper.Attributes;
using DataKeeper.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

[StaticClassInspector()]
public static class GameSettings
{
    private static float MasterVolumePrivate = 0.15f;

    public static float MasterVolume = 0.75f;
    public static bool MusicEnabled = true;
    public static int DifficultyLevel = 2;
    public static string PlayerName = "Player";
    public static Color ThemeColor = Color.blue;
    public static Vector3 SpawnPoint = new Vector3(0, 1, 0);

    public static int PropertyMaxPlayers { get; set; } = 10;
    
    private static int PrivateProp{ get; set; } = 1;
    public static int PrivateSetProp { get; private set; } = 3;


    public static readonly int MaxPlayers = 4; // This will be shown but not editable
    
    public static float CalculatedValue => MasterVolume * 100;  // Read-only property

    [SerializeReference, SerializeReferenceSelector]
    public static ValueBase ValueBaseProp;
    
    public static Reactive<int> ReactiveInt = new Reactive<int>(2);
    public static ReactivePref<bool> ReactivePrefBool = new ReactivePref<bool>(true, "bool_key");
    
    public static int[] IntArray = new int[] { 1, 2, 3, 4, 5 };
    public static List<float> FloatList = new List<float> {1, 2, 3, 4, 5};
    
    public static Dictionary<int, bool> BoolDictionary = new Dictionary<int, bool>() { { 1, true }, { 2, false }, { 3, false } };

}