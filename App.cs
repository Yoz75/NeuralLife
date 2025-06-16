
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

        private bool IsRunning = true;

        private Simulation.Simulation Simulation;

        public void Stop()
        {
            IsRunning = false;
        }

        public void StartSimulation()
        {
            Simulation = new(Screen.Width, Screen.Height);

            Simulation.RandomFill<Food>(SimulationSettings.FoodSpawnCount);
            Simulation.RandomFill<Agent>(AgentSpawnCount);
        }

        public void Run()
        {
            Renderer = new SFMLRenderer();
            Input = new SFMLInput();

            Renderer.Setup((uint)Screen.Width, (uint)Screen.Height, "wow!");
            Color[,] colors;

            StartSimulation();

            InputThread = new Thread(GetInput);
            InputThread.Name = "Input handle thread";
            InputThread.Start();

            while(IsRunning)
            {
                lock(InvokeOnUpdate)
                {
                    foreach(var action in InvokeOnUpdate)
                    {
                        action.Invoke();
                    }
                }

                InvokeOnUpdate.Clear();

                Simulation.Update();
                colors = GetRenderColors();
                Renderer.Update();
                Renderer.Render(colors);

                Simulation.RandomFillIfNull<Food>(SimulationSettings.FoodSpawnCount);
            }

            void GetInput()
            {
                while(IsRunning)
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
                        if(Input.IsKeyDown(Keys.R))
                        {
                            InvokeOnUpdate.Add(StartSimulation);
                        }

                        if(Input.IsKeyDown(Keys.Space))
                        {
                            InvokeOnUpdate.Add(() => SimulationSettings.EnableFoodDispawn = !SimulationSettings.EnableFoodDispawn);
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
                            InvokeOnUpdate.Add(() => Simulation.RandomFill<Spike>(SpikeSpawnCount));
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

        private Color[,] GetRenderColors()
        {
            switch(Screen.RenderMode)
            {
                case RenderMode.BaseColors:
                    return Simulation.AsColors();
                default:
                    throw new InvalidOperationException($"Invalid render mode {Screen.RenderMode}!");
            }
        }
    }
}
