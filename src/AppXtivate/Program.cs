using AppXtivate.ComImports;
using Mono.Options;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static AppXtivate.ComImports.ShellItemHelpers;

namespace appxtivate
{
    // https://github.com/david-risney/AppxUtilities
    // https://code.msdn.microsoft.com/windowsapps/Package-Manager-Inventory-ee821079/sourcecode?fileId=42807&pathId=1141337799
    // https://github.com/jbe2277/waf/wiki/Using-Windows-Runtime-in-a-.NET-desktop-application
    class Program
    {
        enum RunMode
        {
            Auto,
            ActivateFile,
            ActivateApp
        }

        static void ActivateFile(string appUserModelId, string path)
        {
            var appActiveManager = new ApplicationActivationManager();
            var items = SHCreateShellItemArrayFromParsingName(path);
            appActiveManager.ActivateForFile(appUserModelId, items, "Open", out uint _);
        }


        static void ActivateApp(string appUserModelId, string[] args)
        {
            var appActiveManager = new ApplicationActivationManager();
            var argsString = string.Join(
                " ", 
                args.Select(a => "\"" + a.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""));
            appActiveManager.ActivateApplication(appUserModelId, argsString, ActivateOptions.None, out _);
        }

        static void RunAuto(string[] args)
        {
            if (args.Length != 2)
            {
                RunActivateApp(args);
                return;
            }

            try
            {
                RunActivateFile(args);
            }
            catch (COMException ex)
            {
                if ((uint)ex.ErrorCode == 0x80270254)
                {
                    RunActivateApp(args);
                }
                else
                {
                    throw;
                }
            }
        }

        static void RunActivateApp(string[] args)
        {
            if (args.Length < 1)
            {
                throw new InvalidOperationException("Expected AppUserModelId argument");
            }

            ActivateApp(args[0], args.Skip(1).ToArray());
        }

        static void RunActivateFile(string[] args)
        {
            if (args.Length != 2)
            {
                throw new InvalidOperationException("Expected AppUserModelId and file arguments");
            }

            ActivateFile(args[0], args[1]);
        }

        static async Task RunList(string[] args)
        {
            //var packageManager = new Windows.Management.Deployment.PackageManager();
            var catalog = Windows.ApplicationModel.PackageCatalog.OpenForCurrentUser();
            foreach (var package in packageManager.FindPackages())
            {
                Console.WriteLine("{0}", package.DisplayName);
                foreach (var appListEntry in await package.GetAppListEntriesAsync())
                {
                    Console.WriteLine(" - {0}", appListEntry.DisplayInfo.DisplayName);
                }
            }

        }

        static Command RunCommand()
        {
            var runMode = RunMode.Auto;
            return new Command("run", "command help")
            {
                Options = new OptionSet
                {
                    {"auto", _ => runMode = RunMode.Auto},
                    {"activate-file|f", _ => runMode = RunMode.ActivateFile},
                    {"activate-app|a", _ => runMode = RunMode.ActivateApp}
                },
                Run = a =>
                {
                    switch (runMode)
                    {
                        case RunMode.Auto:
                            RunAuto(a.ToArray());
                            break;
                        case RunMode.ActivateFile:
                            RunActivateFile(a.ToArray());
                            break;
                        case RunMode.ActivateApp:
                            RunActivateApp(a.ToArray());
                            break;
                    }
                },
            };
        }

        static int Main(string[] args)
        {
            CommandSet suite = null;
            suite = new CommandSet("AppXtivate")
            {
                "usage: suite-name COMMAND [OPTIONS]+",
                RunCommand(),
                new Command("list")
                {
                    Run = a => { RunList(a.ToArray()).Wait(); }
                }
            };
            return suite.Run(args);
        }

    }
}
