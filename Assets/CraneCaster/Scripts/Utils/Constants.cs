using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    // Debug Values
    public const float PieceShrinkFactor = 0.5f;
    public const float SpellInstantiatePosOffset = 2.5f;
    public const float SpellDuration = 10f; // TODO: not hardcode spell lifespan
    
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
    Pickup,
    Spell,
}

public static class TagsLookUp {
    public static Dictionary<Tags, string> LookUp = new() {
        { Tags.Pickup, "Pickup" },
        { Tags.Spell, "Spell" },
    };
}