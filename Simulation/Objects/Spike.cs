
namespace NeuralLife.Simulation.Objects
{
    public class Spike : SimulationObject
    {
        public Spike()
        {
            Color = new Color(100, 100, 100);
        }

        protected override void OnUpdate(ObjectEnvironmentData data)
        {
            var neighbors = data.Simulation.GetNeighbors(data.Position);
            foreach(var neighbor in neighbors)
            {
                if(neighbor is Agent)
                {
                    neighbor.ShallDestroyOnUpdate = true;
                }
            }
        }
    }
}
