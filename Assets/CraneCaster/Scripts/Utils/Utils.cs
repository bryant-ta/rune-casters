public static class Utils {
    public static T GetRandomEnum<T>() {
        System.Array arr = System.Enum.GetValues(typeof(T));
        return (T) arr.GetValue(UnityEngine.Random.Range(0, arr.Length));
    }
}