using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ConfigReader
{
    public static Dictionary<string,string> GetConfigFromJson(string path = "/Resources/config.json")
    {
        Dictionary<string, string> configDictionary = new Dictionary<string, string>();
        string fullPath = Application.dataPath + path;
        string jsonString = File.ReadAllText(fullPath);
        var output = JsonUtility.FromJson<SimpleConfig>(jsonString);
        Debug.Log(output.wave_count);
        return configDictionary;
    }
}


public class SimpleConfig
{
    public int wave_count;
    public int mobs_per_wave;
    public int mob_damage;
}