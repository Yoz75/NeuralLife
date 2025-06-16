
namespace NeuralLife.Simulation.Objects
{
    public class Spike : SimulationObject
    {
        public Spike()
        {
            Color = new Color(100, 100, 100);
        }
        private static float Damage = 100000f;

        protected override void OnUpdate()
        {
            var neighbors = Simulation.GetNeighbors(Position);
            foreach(var neighbor in neighbors)
            {
                if(neighbor is Agent)
                {
                    (neighbor as Agent).Damage(Damage);
                }
            }
        }
    }
}
