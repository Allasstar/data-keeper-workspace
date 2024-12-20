using DataKeeper.Base;
using DataKeeper.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData SO", menuName = "DataKeeper Examples/PlayerData SO")]
public class PlayerDataSO : SO
{
    [field: SerializeField] public ReactivePref<string> PlayerName { private set; get; } = new ReactivePref<string>("John Doe", "player_name");

    [field: SerializeField] public Reactive<int> PlayerHealth { private set; get; } = new Reactive<int>(100);
    
    // Called on game start
    public override void Initialize()
    {
        PlayerHealth.Value = 100; // Reset
        DK.Data.Reg<PlayerDataSO>(this); // Register this Scriptable object
    }
}
