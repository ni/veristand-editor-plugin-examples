using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using NationalInstruments.Core;
using NationalInstruments.Shell;

namespace NationalInstruments.VeriStand.CustomApplication
{
    /// <summary>
    /// Defines the entry point for a Custom VeriStand Application
    /// </summary>
    public static class ShellApp
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                DefaultLoggingConfiguration.Setup();

                // Custom splash screen
                ApplicationSplashScreen splashScreen = null;
                if (IsOnlyInstance())
                {
                    var windowsSplashScreen = new SplashScreen("Resources/SplashScreen.png");
                    splashScreen = new ApplicationSplashScreen(windowsSplashScreen);
                    splashScreen.Show();
                }

                var startupInfo = new CustomControlsExamples.CustomApplicationStartupInfo(splashScreen);
                PreferencesHelper.Initialize(startupInfo.ApplicationInfo);
                ShellApplication.RunStandalone(startupInfo);
            }
            finally
            {
                DefaultLoggingConfiguration.Cleanup();
            }
        }

        private static bool IsOnlyInstance()
        {
            var current = Process.GetCurrentProcess();
            return Process.GetProcessesByName(current.ProcessName)
                .All(process => process.Id == current.Id
                    || Version(process) != Version(current));
        }

        private static string Version(Process process) => process.MainModule.FileVersionInfo.ProductVersion;
    }
}
