using NeuralLife.Simulation;

namespace NeuralLife.Rendering
{
    public interface IRenderer
    {
        public bool IsWindowActive { get; }
        public void Setup(uint xResolution, uint yResolution, string title);
        public void Update();
        public void Render(Color[,] screen);
        public void ShowSimulationSettings(SimulationSettings settings);
    }
}
