using CAI;
using Spectre.Console;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NeuralLife
{
    internal class Program
    {
        private static AppInterface BukashkiInterface;
        private static App BukashkiApp = null;

        private static void Main(string[] args)
        {
            Thread appThread = new(() => { BukashkiApp = new(); BukashkiApp.Run(); });
            appThread.Start();

            BukashkiInterface = new("Neural Life", isCatchExceptions: true);

            #region Commands
            BukashkiInterface.AddCommand(new("stats", "show simulation stats and settings values",
                () => { LockAndExecute(BukashkiApp, ShowSettings); }));

            BukashkiInterface.AddCommand(new("exit", "exit simulation",
                () => { LockAndExecute(BukashkiApp, ExitSimulation); }));

            BukashkiInterface.AddCommand(new("restart", "restart simulation",
                () => { LockAndExecute(BukashkiApp, BukashkiApp.StartSimulation); }));

            BukashkiInterface.AddCommand<bool>(new("fdisp", "is food dispawn?",
                (isDispawn) => { SimulationSettings.EnableFoodDispawn = isDispawn; }, "fdisp [true/false]"));

            BukashkiInterface.AddCommand<bool>(new("autodiv", "are cells divide when they want it?",
                (doAutoDivide) => { SimulationSettings.EnableAutoDivide = doAutoDivide; }, "autodiv [true/false]"));
            #endregion

            BukashkiInterface.Start();

            appThread.Join();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LockAndExecute<T>(T lockObject, Action method)
        {
            lock(lockObject)
            {
                method();
            }
        }

        private static void ShowSettings()
        {
            Table settingsTable = new Table().AddColumns("name", "value");

            settingsTable.AddRow("EnableFoodDispawn", SimulationSettings.EnableFoodDispawn.ToString());
            settingsTable.AddRow("FoodLifeTime", SimulationSettings.FoodLifeTime.ToString());
            settingsTable.AddRow("FoodSpawnCount", SimulationSettings.FoodSpawnCount.ToString());
            settingsTable.AddRow("AllowColonialism", SimulationSettings.AllowColonialism.ToString());
            settingsTable.AddRow("EnableAutoDivide", SimulationSettings.EnableAutoDivide.ToString());

            BukashkiInterface.WriteInfo(settingsTable);
        }

        private static void ExitSimulation()
        {
            BukashkiApp.Stop();
            BukashkiInterface.Quit();
        }
    }
}

