using TMPro;
using UnityEngine;

public class RecordScript : MonoBehaviour
{
    public TextMeshProUGUI nickname;
    public TextMeshProUGUI score;
    public TextMeshProUGUI enemiesKilled;
    public TextMeshProUGUI date;
    public TextMeshProUGUI rank;

    public void Initialize(ScoreEntry scoreEntry, int rank)
    {
        nickname.text = scoreEntry.playerNickname;
        score.text = scoreEntry.totalScore.ToString();
        enemiesKilled.text = scoreEntry.totalEnemiesKilled.ToString();
        date.text = scoreEntry.date;
        this.rank.text = rank.ToString();
    }
}
