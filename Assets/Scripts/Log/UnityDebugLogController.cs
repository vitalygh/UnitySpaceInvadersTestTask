using UnityEngine;

public class UnityDebugLogController : MonoBehaviour, ILogController
{
    public void Notify(string message)
    {
        Debug.Log(message);
    }

    public void Warning(string message)
    {
        Debug.LogWarning(message);
    }

    public void Error(string message)
    {
        Debug.LogError(message);
    }
}
