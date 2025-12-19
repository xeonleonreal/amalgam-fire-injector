using System;
using Microsoft.Win32;
using System.Diagnostics;

namespace SteamMonitor
{
    public class SteamLoginChecker
    {
        public static bool steam_logged_in = false;
        public static string activeUserId = "0";
        public static void CheckSteamLogin()
        {
            steam_logged_in = false;
            bool isSteamRunning = IsSteamProcessRunning();
            (bool isLoggedIn, string userId) = CheckSteamLoginViaRegistry();
            steam_logged_in = isLoggedIn;
            activeUserId = userId;
        }
        private static bool IsSteamProcessRunning()
        {
            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    try
                    {
                        if (process.ProcessName.ToLower().Contains("steam"))
                        {
                            return true;
                        }
                    }
                    catch { continue; }
                }
            }
            catch { }
            return false;
        }
        private static (bool isLoggedIn, string userId) CheckSteamLoginViaRegistry()
        {
            string userId = "0";

            try
            {
                using (RegistryKey activeProcessKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam\ActiveProcess"))
                {
                    if (activeProcessKey != null)
                    {
                        object activeUserValue = activeProcessKey.GetValue("ActiveUser");
                        userId = activeUserValue?.ToString() ?? "0";
                        bool loggedIn = userId != "0";
                        return (loggedIn, userId);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("something go wrong :(");
            }

            return (false, userId);
        }
    }
}