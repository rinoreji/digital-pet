using System.IO;
using System.Text;

namespace DigitalPetApp.Services;

public class FileLoggingService : ILoggingService, IDisposable
{
    private readonly string logFilePath;
    private readonly object fileLock = new();

    public FileLoggingService(string? path = null)
    {
        var baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
        logFilePath = path ?? Path.Combine(baseDir, "digitalpet.log");
        TryWriteLine("=== Log Start " + DateTime.Now + " ===");
    }

    public void Info(string message) => Write("INFO", message);
    public void Warn(string message) => Write("WARN", message);
    public void Error(string message, Exception? ex = null) => Write("ERROR", message + (ex != null ? " :: " + ex : ""));

    private void Write(string level, string message)
    {
        TryWriteLine($"{DateTime.Now:O} [{level}] {message}");
    }

    private void TryWriteLine(string line)
    {
        try
        {
            lock (fileLock)
            {
                File.AppendAllText(logFilePath, line + Environment.NewLine, Encoding.UTF8);
            }
        }
        catch { /* ignore */ }
    }

    public void Dispose()
    {
        TryWriteLine("=== Log End " + DateTime.Now + " ===");
    }
}
