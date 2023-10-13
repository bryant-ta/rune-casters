using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    public const string PhotonPrefabsPath = "PhotonPrefabs/";
}

/***************************    Custom Properties    ***************************/
public enum CustomPropertiesKey {
    Hp,
}

public static class CustomPropertiesLookUp {
    public static Dictionary<CustomPropertiesKey, string> LookUp = new() {
        { CustomPropertiesKey.Hp, "Hp" },
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