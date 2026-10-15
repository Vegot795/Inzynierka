using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScoreEntry 
{
    public string playerNickname;
    public int totalEnemiesKilled;
    public int totalScore;
    public string date;
}

[Serializable]
public class StatListClass
{
    public List<ScoreEntry> StatList = new List<ScoreEntry>();
}
