using UnityEngine;

public class EndgameStatsController : MonoBehaviour
{
    public PlayerData playerData;
    string SavesPath = Application.persistentDataPath + "playerScores.json";

    public void Awake()
    {
        playerData = PlayerData.Instance;
    }


}
