using System.IO;
using UnityEngine;

public static class ConfigReader
{
    public static SimpleConfig GetConfigFromJson(string path = "/Resources/config.json")
    {
        string fullPath = Application.dataPath + path;
        string jsonString = File.ReadAllText(fullPath);
        var output = JsonUtility.FromJson<SimpleConfig>(jsonString);
        return output;
    }
}


public class SimpleConfig
{
    public int wave_count;
    public int mobs_per_wave;
    public int mob_damage;
}