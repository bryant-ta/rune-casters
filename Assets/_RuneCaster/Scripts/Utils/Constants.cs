using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    // Debug Values
    public const float PieceShrinkFactor = 0.5f;
    public const float PiecePlacementOverlayAlpha = 0.6f;
    public const float MinPunchDashDistance = 20f;
    public const float PunchCooldown = 1f;
    public const float SpellInstantiatePosOffset = 2.5f;
    public const float SpellDuration = 15f; // TODO: not hardcode spell lifespan

    // Paths
    public const string PhotonPrefabsPath = "PhotonPrefabs/";
}

public static class CustomPropertiesKey {
    public const string PlayerReady = "PlayerReady";
    public const string PlayerLoaded = "PlayerLoaded";
    public const string Hp = "Hp";
    public const string Shield = "Shield";
}

// just use const for this next time lmao
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