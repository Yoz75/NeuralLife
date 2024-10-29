
namespace NeuralLife.Simulation.Objects
{
    public class Food : SimulationObject
    {
        public Food()
        {
            Color = new Color(180, 0, 0);
        }

        private const uint LifeTime = 100;

        protected override void OnUpdate(ObjectEnvironmentData data)
        {
        }

    }
}
