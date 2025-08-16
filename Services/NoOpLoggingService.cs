namespace DigitalPetApp.Services;

public class NoOpLoggingService : ILoggingService
{
    public void Info(string message) { }
    public void Warn(string message) { }
    public void Error(string message, Exception? ex = null) { }
}
