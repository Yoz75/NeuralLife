
namespace NeuralLife.Simulation.Objects
{
    public class Food : SimulationObject
    {
        public Food()
        {
            Color = new Color(180, 0, 0);
        }

        private uint LifeTime;

        protected override void OnUpdate()
        {
            if(SimulationSettings.EnableFoodDispawn)
            {
                LifeTime++;
            }
            if(LifeTime >= SimulationSettings.FoodLifeTime)
            {
                Simulation.Destroy(this);
                return;
            }
        }

    }
}
