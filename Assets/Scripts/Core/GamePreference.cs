using System.Collections.Generic;
using System.IO;


public enum Language
{
    English
}



public static class GamePreference
{
    public static int SFXVolume
    {
        get => Get(nameof(SFXVolume), 100);
        set => Set(nameof(SFXVolume), value);
    }
    public static int MusicVolume
    {
        get => Get(nameof(MusicVolume), 100);
        set => Set(nameof(MusicVolume), value);
    }
    public static bool FullScreen
    {
        get => Get(nameof(FullScreen), true);
        set => Set(nameof(FullScreen), value);
    }
    public static int Width
    {
        get => Get(nameof(Width), 1920);
        set => Set(nameof(Width), value);
    }
    public static int Height
    {
        get => Get(nameof(Height), 1080);
        set => Set(nameof(Height), value);
    }

    //Input stuff
    public static string MoveInput = "Move";
    public static string JumpButton = "Jump";
    public static string SwingButton = "Swing";
    public static string CrouchButton = "Crouch";
    public static string PauseButton = "Pause";



    private static Dictionary<string, object> data = new Dictionary<string, object>();
    public static string filePath = "";

    public static void SetPath(string path) => filePath = path;

    public static void SaveToFile()
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            Dictionary<string, object> sanitizedData = new Dictionary<string, object>(data);
            //Save at path
        }
    }

    public static void LoadFromFile()
    {
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            //Load at path
        }
    }
    private static void Set<T>(string key, T value)
    {
        data[key] = value;
        SaveToFile();
    }

    private static T Get<T>(string key, T defaultValue = default)
    {
        if (data.TryGetValue(key, out object value))
        {
            if (value is T tValue)
                return tValue;

            try { return (T)System.Convert.ChangeType(value, typeof(T)); }
            catch { }
        }
        return defaultValue;
    }







}
