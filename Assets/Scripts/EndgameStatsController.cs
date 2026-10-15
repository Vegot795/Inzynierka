using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
public static class EndgameStatsController
{
    public static string fileName = "playerScores.json";
    public static int maxPositionsDisplayed = 20;
    public static string savesPath = Path.Combine(Application.persistentDataPath, fileName);
    
    public static void Save(StatListClass statList)
    {
        try
        {
            string file = JsonUtility.ToJson(statList, true);
            File.WriteAllText(savesPath, file);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[EndgameStatController] - file could not be saved");
        }
    }

    public static StatListClass Load()
    {
        if (!File.Exists(savesPath))
        {
            return new StatListClass();
        }

        try
        {
            string file = File.ReadAllText(savesPath);
            StatListClass statList = JsonUtility.FromJson<StatListClass>(file);

            if (statList == null)
            {
                statList = new StatListClass();
            }
            if (statList.StatList == null)
            {
                statList.StatList = new List<ScoreEntry>();
            }

            return statList;
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[EndgameStatController] - file could not be read");
            return new StatListClass();
        }
    }

    public static void AddEntry(ScoreEntry entry)
    {
        StatListClass statList = Load();
        statList.StatList.Add(entry);
        statList.StatList = statList.StatList
            .OrderByDescending(entry => entry.totalScore)
            .Take(maxPositionsDisplayed)
            .ToList();

        Save(statList);
    }

}
