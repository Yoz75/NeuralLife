using NeuralLife.Simulation;

namespace NeuralLife.Rendering
{
    /// <summary>
    /// Something that creates a render window and shows game to the screen
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Are we still can render?
        /// </summary>
        public bool IsWindowActive { get; }

        /// <summary>
        /// Prepare renderer for rendering
        /// </summary>
        /// <param name="xResolution"></param>
        /// <param name="yResolution"></param>
        /// <param name="title"></param>
        public void Setup(uint xResolution, uint yResolution, string title);
        public void Update();
        public void Render(Color[,] screen);
    }
}
