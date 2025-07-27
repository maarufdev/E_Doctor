using System.Runtime.InteropServices;

namespace E_Doctor.WindowsFormApp.Helpers;
public class ConsoleHelper
{
    [DllImport("kernel32.dll")]
    public static extern bool AllocConsole();
}
