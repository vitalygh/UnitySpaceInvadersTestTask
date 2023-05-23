using UnityEngine.Events;

public interface IGameController
{
    void GameOver();
    void Win();
    void Restart();

    int Score { get; set; }
    int HP { get; set; }
    int Level { get; set; }

    UnityAction<int> OnScoreChanged { get; set; }
    UnityAction<int> OnHPChanged { get; set; }
    UnityAction<int> OnLevelChanged { get; set; }
}
