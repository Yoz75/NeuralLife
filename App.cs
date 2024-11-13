
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

        public void Run()
        {
            uint width = 160;
            uint height = 96;

            Renderer = new SFMLRenderer();
            Input = new SFMLInput();

            Renderer.Setup(width, height, "wow!");

            Simulation.Simulation simulation = new((int)width, (int)height);
            Color[,] colors;

            float foodSpawnCount = 0.002f;
            float agentSpawnCount = 0.008f;
            const uint foodLifetimeStep = 5;
            const float foodSpawnStep = 0.0005f;
            const float spikeSpawnCount = 0.001f;

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

                simulation.RandomFillIfNull<Food>(foodSpawnCount);

                Thread.Sleep(10);
            }

            void StartSimulation()
            {
                simulation = new((int)width, (int)height);

                simulation.RandomFill<Food>(foodSpawnCount);
                simulation.RandomFill<Agent>(agentSpawnCount);
            }

            void GetInput()
            {
                while(true)
                {
                    Input.Update();
                    lock(InvokeOnUpdate)
                    {

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
                            InvokeOnUpdate.Add(() => Food.UpdatesLifeTime += foodLifetimeStep);
                        }
                        else if(Input.IsKeyDown(Keys.Y))
                        {
                            InvokeOnUpdate.Add(() => Food.UpdatesLifeTime -= foodLifetimeStep);
                        }

                        if(Input.IsKeyDown(Keys.Q))
                        {
                            InvokeOnUpdate.Add(() => simulation.RandomFill<Spike>(spikeSpawnCount));
                        }

                        //Plus near backspace also will work
                        if(Input.IsKeyDown(Keys.Add) || Input.IsKeyDown(Keys.Equal))
                        {
                            InvokeOnUpdate.Add(() => foodSpawnCount += foodSpawnStep);
                        }
                        else if(Input.IsKeyDown(Keys.Subtract))
                        {
                            InvokeOnUpdate.Add(() => foodSpawnCount -= foodSpawnStep);
                        }
                    }
                }
            }
        }
    }
}
