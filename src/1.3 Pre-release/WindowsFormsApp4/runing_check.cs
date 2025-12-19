using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace amalgam_fireinjector
{
    public class ProgramMonitor
    {
        
        public static bool WaitForProgram(string programName, int checkInterval = 5000, int timeoutSeconds = 30)
        {
            DateTime startTime = DateTime.Now;
            int checkCount = 0;

            while (true)
            {
                checkCount++;
                if (IsProgramRunning(programName))
                {
                    return true;
                }

                // Check timeout
                if (timeoutSeconds > 0 && (DateTime.Now - startTime).TotalSeconds >= timeoutSeconds)
                {
                    return false;
                }
                Thread.Sleep(checkInterval);
            }
        }
        public static bool IsProgramRunning(string programName)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(programName);
                return processes.Length > 0;
            }
            catch
            {
                return false;
            }
        }
        public static void MonitorAndRestartProgram(string programName, string programPath, int maxRestarts = 3)
        {

            int restartCount = 0;

            while (restartCount < maxRestarts)
            {
                if (WaitForProgram(programName, 3000, 0))
                {
                    while (IsProgramRunning(programName))
                    {
                        Thread.Sleep(5000);
                    }
                    restartCount++;

                    if (restartCount < maxRestarts)
                    {
                        try
                        {
                            Process.Start(programPath);
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}