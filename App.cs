
using NeuralLife.Input;
using NeuralLife.Rendering;
using NeuralLife.Simulation;
using NeuralLife.Simulation.Objects;
using System;
using System.Collections.Generic;
using System.Threading;

namespace NeuralLife
{
    public class App
    {
        private IRenderer Renderer;
        private IInput Input;

        private List<Action> InvokeOnUpdate = new List<Action>();

        private Thread InputThread;
        private float FoodSpawnCount = 0.002f;
        private float AgentSpawnCount = 0.008f;
        private const uint FoodLifetimeStep = 5;
        private const float FoodSpawnStep = 0.0005f;
        private const float SpikeSpawnCount = 0.001f;

        public SimulationSettings GetSettings()
        {
            SimulationSettings settings = new SimulationSettings();
            settings.IsFoodDispawn = Food.IsDispawn;
            settings.FoodSpawnCount = FoodSpawnCount;
            settings.FoodLifeTime = Food.UpdatesLifeTime;
            return settings;
        }

        public void Run()
        {
            uint width = 160;
            uint height = 96;

            Renderer = new SFMLRenderer();
            Input = new SFMLInput();

            Renderer.Setup(width, height, "wow!");

            Simulation.Simulation simulation = new((int)width, (int)height);
            Color[,] colors;

            StartSimulation();

            InputThread = new Thread(GetInput);
            InputThread.Name = "Input handle thread";
            InputThread.Start();

            while(true)
            {
                lock(InvokeOnUpdate)
                {
                    foreach(var action in InvokeOnUpdate)
                    {
                        action.Invoke();
                    }
                }

                InvokeOnUpdate.Clear();

                simulation.Update();
                colors = simulation.AsColors();
                Renderer.Update();
                Renderer.Render(colors);

                simulation.RandomFillIfNull<Food>(FoodSpawnCount);

                Thread.Sleep(10);
            }

            void StartSimulation()
            {
                simulation = new((int)width, (int)height);

                simulation.RandomFill<Food>(FoodSpawnCount);
                simulation.RandomFill<Agent>(AgentSpawnCount);
            }

            void GetInput()
            {
                while(true)
                {
                    Input.Update();
                    lock(InvokeOnUpdate)
                    {
                        if(Input.IsKeyDown(Keys.S))
                        {
                            Renderer.ShowSimulationSettings(GetSettings());
                        }
                        if(Input.IsKeyDown(Keys.R))
                        {
                            InvokeOnUpdate.Add(StartSimulation);
                        }

                        if(Input.IsKeyDown(Keys.Space))
                        {
                            InvokeOnUpdate.Add(() => Food.IsDispawn = !Food.IsDispawn);
                        }

                        if(Input.IsKeyDown(Keys.T))
                        {
                            InvokeOnUpdate.Add(() => Food.UpdatesLifeTime += FoodLifetimeStep);
                        }
                        else if(Input.IsKeyDown(Keys.Y))
                        {
                            InvokeOnUpdate.Add(() => Food.UpdatesLifeTime -= FoodLifetimeStep);
                        }

                        if(Input.IsKeyDown(Keys.Q))
                        {
                            InvokeOnUpdate.Add(() => simulation.RandomFill<Spike>(SpikeSpawnCount));
                        }

                        //Plus near backspace also will work
                        if(Input.IsKeyDown(Keys.Add) || Input.IsKeyDown(Keys.Equal))
                        {
                            InvokeOnUpdate.Add(() => FoodSpawnCount += FoodSpawnStep);
                        }
                        else if(Input.IsKeyDown(Keys.Subtract))
                        {
                            InvokeOnUpdate.Add(() => FoodSpawnCount -= FoodSpawnStep);
                        }
                    }
                }
            }
        }
    }
}
