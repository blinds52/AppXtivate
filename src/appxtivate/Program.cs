using AppXtivate.ComImports;
using Mono.Options;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using static AppXtivate.ComImports.ShellItemHelpers;

namespace appxtivate
{
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
                RunCommand()
            };
            return suite.Run(args);
        }

    }
}
