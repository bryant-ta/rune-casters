using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    // Debug Values
    public const float PieceShrinkFactor = 0.5f;
    public const float MinPunchDashDistance = 15f;
    public const float PunchCooldown = 1f;
    public const float SpellInstantiatePosOffset = 2.5f;
    public const float SpellDuration = 15f; // TODO: not hardcode spell lifespan

    
    // Paths
    public const string PhotonPrefabsPath = "PhotonPrefabs/";
}

/***************************    Custom Properties    ***************************/

public enum CustomPropertiesKey {
    Hp,
    Shield,
}

public static class CustomPropertiesLookUp {
    public static Dictionary<CustomPropertiesKey, string> LookUp = new() {
        { CustomPropertiesKey.Hp, "Hp" },
        { CustomPropertiesKey.Shield, "Shield" },
    };
}

/***************************    Tags    ***************************/

public enum Tags {
    Player,
    Pickup,
    Punch,
    Spell,
    Wall,
}

public static class TagsLookUp {
    public static Dictionary<Tags, string> LookUp = new() {
        { Tags.Player, "Player" },
        { Tags.Pickup, "Pickup" },
        { Tags.Punch, "Punch" },
        { Tags.Spell, "Spell" },
        { Tags.Wall, "Wall" },
    };
}