public interface ILogController
{
    void Notify(string message);
    void Warning(string message);
    void Error(string message);
}
