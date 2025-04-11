
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
        private float AgentSpawnCount = 0.008f;
        private const uint FoodLifetimeStep = 5;
        private const float FoodSpawnStep = 0.0005f;
        private const float SpikeSpawnCount = 0.001f;

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

                simulation.RandomFillIfNull<Food>(SimulationSettings.FoodSpawnCount);

            }

            void StartSimulation()
            {
                simulation = new((int)width, (int)height);

                simulation.RandomFill<Food>(SimulationSettings.FoodSpawnCount);
                simulation.RandomFill<Agent>(AgentSpawnCount);
            }

            void GetInput()
            {
                while(true)
                {
                    lock(Renderer)
                    {
                        if(!Renderer.IsWindowActive)
                        {
                            continue;
                        }
                    }
                    Input.Update();
                    lock(InvokeOnUpdate)
                    {
                        if(Input.IsKeyDown(Keys.S))
                        {
                            Renderer.ShowSimulationSettings();
                        }
                        if(Input.IsKeyDown(Keys.R))
                        {
                            InvokeOnUpdate.Add(StartSimulation);
                        }

                        if(Input.IsKeyDown(Keys.Space))
                        {
                            InvokeOnUpdate.Add(() => SimulationSettings.IsFoodDispawn = !SimulationSettings.IsFoodDispawn);
                        }

                        if(Input.IsKeyDown(Keys.T))
                        {
                            if(SimulationSettings.FoodLifeTime < uint.MaxValue)
                            {
                                InvokeOnUpdate.Add(() => SimulationSettings.FoodLifeTime += FoodLifetimeStep);
                            }
                        }
                        else if(Input.IsKeyDown(Keys.Y))
                        {
                            if(SimulationSettings.FoodLifeTime >= FoodLifetimeStep)
                            { 
                                InvokeOnUpdate.Add(() => SimulationSettings.FoodLifeTime -= FoodLifetimeStep);
                            }
                        }

                        if(Input.IsKeyDown(Keys.Q))
                        {
                            InvokeOnUpdate.Add(() => simulation.RandomFill<Spike>(SpikeSpawnCount));
                        }

                        //Plus near backspace also will work
                        if(Input.IsKeyDown(Keys.Add) || Input.IsKeyDown(Keys.Equal))
                        {
                            InvokeOnUpdate.Add(() => SimulationSettings.FoodSpawnCount += FoodSpawnStep);
                        }
                        else if(Input.IsKeyDown(Keys.Subtract))
                        {
                            InvokeOnUpdate.Add(() => SimulationSettings.FoodSpawnCount -= FoodSpawnStep);
                        }

                        if(Input.IsKeyDown(Keys.C))
                        {
                            SimulationSettings.AllowColonialism = !SimulationSettings.AllowColonialism;
                        }
                    }
                }
            }
        }
    }
}
