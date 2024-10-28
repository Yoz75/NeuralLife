﻿
using SFML.Graphics;
using SFML.Window;

namespace NeuralLife.Rendering
{
    public class SFMLRenderer : IRenderer
    {
        private RenderWindow Window;
        private Image ScreenBuffer;
        private Sprite ScreenSprite;

        public void Setup(uint xResolution, uint yResolution, string title)
        {
            VideoMode videoMode = new VideoMode(xResolution, yResolution);

            Window = new RenderWindow(videoMode, title, Styles.Default);

            Window.Closed += (_, _) => Window.Close();

            ScreenBuffer = new Image(xResolution, yResolution);
            ScreenSprite = new Sprite();
        }

        public void Update()
        {
            Window.DispatchEvents();
        }

        public void Render(Simulation.Color[,] screen)
        {
            for(uint i = 0; i < screen.GetLength(0); i++)
            {
                for(uint j = 0; j < screen.GetLength(1); j++)
                {
                    ScreenBuffer.SetPixel(i, j, new Color(screen[i, j].AsUint()));
                }
            }
            Texture gpuAllocatedScreen = new Texture(ScreenBuffer);
            ScreenSprite.Texture = gpuAllocatedScreen;

            RenderStates states = new(BlendMode.Alpha);

            ScreenSprite.Draw(Window, states);
            Window.Display();
        }
    }
}
