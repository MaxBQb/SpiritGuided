using System;

public static class RoomCode
{
    private static string _newValue => Guid.NewGuid().ToString();
    public static string Value { get; private set; } = _newValue;
    public static event Action OnChanged;

    public static void Update()
    {
        Value = _newValue;
        OnChanged?.Invoke();
    }
}