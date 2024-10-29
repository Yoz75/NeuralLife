
using NeuralLife.Input;
using NeuralLife.Rendering;
using NeuralLife.Simulation;
using NeuralLife.Simulation.Objects;
using System.Threading;

namespace NeuralLife
{
    public class App
    {
        private IRenderer Renderer;
        private IInput Input;
        public void Run()
        {
            uint width = 160; //VideoMode.DesktopMode.Width / 6;
            uint height = 96;// VideoMode.DesktopMode.Height / 6;

            Renderer = new SFMLRenderer();
            Input = new SFMLInput();

            Renderer.Setup(width, height, "wow!");

            float foodSpawnCount = 0.002f;
        //Ok, that is bullshit, but works
        simulationStart:

            Simulation.Simulation simulation = new((int)width, (int)height);
            Color[,] colors;
            simulation.RandomFill<Food>(0.2f);
            simulation.RandomFill<Agent>(0.008f);

            const float foodSpawnStep = 0.0005f;
            const float spikeSpawnCount = 0.001f;

            while(true)            {

                if(Input.IsKeyPressed(Keys.R))
                {
                    goto simulationStart;
                }

                if(Input.IsKeyPressed(Keys.Q))
                {
                    simulation.RandomFill<Spike>(spikeSpawnCount);
                }                

                if(Input.IsKeyPressed(Keys.Add))
                {
                    foodSpawnCount += foodSpawnStep;
                }
                else if(Input.IsKeyPressed(Keys.Subtract))
                {
                    foodSpawnCount -= foodSpawnStep;
                }

                simulation.Update(); 

                colors = simulation.AsColors();
                Renderer.Update();
                Renderer.Render(colors);

                simulation.RandomFillIfNull<Food>(foodSpawnCount);

                Thread.Sleep(10);
            }
        }
    }
}
