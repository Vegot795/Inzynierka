using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public int currentScorePoints;
    void Start()
    {
        currentScorePoints = 0;
    }

    public void AddScore(int scoreToAdd)
    {
        currentScorePoints = currentScorePoints + scoreToAdd;
    }

    public void RemoveScore(int scoreToRemove)
    {
        currentScorePoints = currentScorePoints - scoreToRemove;
    }
}
