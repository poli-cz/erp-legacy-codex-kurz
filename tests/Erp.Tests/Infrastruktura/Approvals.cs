using System.Runtime.CompilerServices;

namespace Erp.Tests.Infrastruktura;

/// <summary>
/// Jednoduché approval testy (bez NuGet balíčku). Očekávaný výstup leží v Approved/{Třída}.{Test}.approved.txt.
/// Když se výstup liší, vznikne vedle soubor .received.txt (v gitu ignorovaný) a test spadne.
/// Schválení změny = zkontrolovat diff a přejmenovat .received.txt na .approved.txt.
/// </summary>
public static class Approvals
{
    public static void Verify(string actual, [CallerFilePath] string callerFile = "", [CallerMemberName] string testName = "")
    {
        string dir = Path.Combine(Path.GetDirectoryName(callerFile)!, "Approved");
        string cls = Path.GetFileNameWithoutExtension(callerFile);
        string approvedPath = Path.Combine(dir, cls + "." + testName + ".approved.txt");
        string receivedPath = Path.Combine(dir, cls + "." + testName + ".received.txt");
        string normalized = actual.Replace("\r\n", "\n");

        string? approved = File.Exists(approvedPath) ? File.ReadAllText(approvedPath).Replace("\r\n", "\n") : null;
        if (approved == normalized)
        {
            if (File.Exists(receivedPath))
                File.Delete(receivedPath);
            return;
        }

        Directory.CreateDirectory(dir);
        File.WriteAllText(receivedPath, normalized);
        Assert.Fail(approved == null
            ? "Chybí schválený výstup. Zkontroluj " + receivedPath + " a přejmenuj na .approved.txt."
            : "Výstup se liší od schváleného. Porovnej:\n  " + approvedPath + "\n  " + receivedPath);
    }
}
