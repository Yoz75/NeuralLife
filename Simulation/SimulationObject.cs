
namespace NeuralLife.Simulation
{    public class SimulationObject
    {
        public string Name;
        public Color Color;
        public Simulation Simulation;
        public Vector2 Position;

        public void Update()
        {
            OnUpdate();
            return;
        }

        public virtual void OnDestroy()
        {
            return;
        }

        protected virtual void OnUpdate()
        {
            return;
        }
    }
}
