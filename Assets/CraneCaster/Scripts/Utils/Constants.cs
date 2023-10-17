using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    // Debug Values
    public const float PieceShrinkFactor = 0.5f;
    public const float SpellInstantiatePosOffset = 2.5f;
    public const float SpellLifespan = 10f; // TODO: not hardcode spell lifespan
    
    // Paths
    public const string PhotonPrefabsPath = "PhotonPrefabs/";
}

/***************************    Custom Properties    ***************************/
public enum CustomPropertiesKey {
    Hp,
    SpellLifespan,
}

public static class CustomPropertiesLookUp {
    public static Dictionary<CustomPropertiesKey, string> LookUp = new() {
        { CustomPropertiesKey.Hp, "Hp" },
        { CustomPropertiesKey.SpellLifespan, "SpellLifespan" },
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