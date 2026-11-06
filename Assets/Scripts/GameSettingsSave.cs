using System;
using System.IO;
using UnityEngine;
[Serializable]
public class GameSettingsSave
{
    public float AudioVolume;
    public float MusicVolume;
    public float EnemyVolume;
    public bool playMusic;
    public bool showFPS;
}

public static class GameSettingsSaveSystem
{
    public static string fileName = "gameSettingsSave.json";
    public static string directory = Path.Combine(Application.persistentDataPath, "settingsSaves");
    public static string savesPath = Path.Combine(directory, fileName);
    public static void Save(GameSettingsSave gameSettings)
    {
        var GS = GameSettings.GS;
        try
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            gameSettings.AudioVolume = GS.audioFloat;
            gameSettings.MusicVolume = GS.musicFloat;
            gameSettings.EnemyVolume = GS.enemyFloat;
            gameSettings.showFPS = GS.showFPS;
            gameSettings.playMusic = GS.canPlayMusic;
            string file = JsonUtility.ToJson(gameSettings, true);
            File.WriteAllText(savesPath, file);
        }
        catch
        {
            Debug.LogWarning("Settings could not ve saved");
        }
    }

    public static GameSettingsSave Load()
    {
        if (!File.Exists(savesPath))
        {
            return new GameSettingsSave();
        }

        try
        {
            string file = File.ReadAllText(savesPath);
            GameSettingsSave GSS = JsonUtility.FromJson<GameSettingsSave>(file);

            if (GSS == null)
            {
                GSS = new GameSettingsSave();
            }

            return GSS;
        }
        catch
        {
            Debug.LogWarning("Settings could not be loaded");
            return new GameSettingsSave();
        }
    }
} // Ive spent on audio system 6 hours - no rest for the wicked
