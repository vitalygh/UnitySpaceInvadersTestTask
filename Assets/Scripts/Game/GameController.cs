using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour, IGameController
{
    private ILogController logController = null;
    private IEntityController[] entityControllers = null;
    private ISerializationController serializationController = null;
    private bool serialized = false;

    public void GameOver()
    {
        Score = 0;
        HP = startHP;
        Level = 0;
        Restart();
    }

    public void Win()
    {
        HP += 1;
        Level += 1;
        Restart();
    }

    public void Restart()
    {
        foreach (var entityController in entityControllers)
            entityController.Restart();
    }

    private int score = 0;
    public int Score
    { 
        get => score;
        set
        {
            OnScoreChanged?.Invoke(value);
            score = value;
        }
    }

    private static readonly int startHP = 3;
    private int hp = startHP;
    public int HP
    {
        get => hp;
        set
        {
            OnHPChanged?.Invoke(value);
            hp = value;
        }
    }

    private int level = 0;
    public int Level
    {
        get => level;
        set
        {
            OnLevelChanged?.Invoke(value);
            level = value;
        }
    }

    public UnityAction<int> OnScoreChanged { get; set; }
    public UnityAction<int> OnHPChanged { get; set; }
    public UnityAction<int> OnLevelChanged { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        logController = GetComponent<ILogController>();
        entityControllers = GetComponents<IEntityController>();
        if ((entityControllers == null) || (entityControllers.Length <= 0))
            logController.Warning(nameof(entityControllers) + " is null or empty");
        serializationController = GetComponent<ISerializationController>();
        if (serializationController == null)
            logController.Warning(nameof(serializationController) + " is null");
        if ((serializationController == null) || !serializationController.Load())
            Restart();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            if (!serialized && serializationController != null)
            {
                serializationController.Save();
                serialized = true;
            }
        }
        else
            serialized = false;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (!serialized && serializationController != null)
            {
                serializationController.Save();
                serialized = true;
            }
        }
        else
            serialized = false;
    }

    private void OnApplicationQuit()
    {
        if (!serialized && (serializationController != null))
            serializationController.Save();
    }
}
