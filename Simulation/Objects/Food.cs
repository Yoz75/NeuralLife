
namespace NeuralLife.Simulation.Objects
{
    public class Food : SimulationObject
    {
        public Food()
        {
            Color = new Color(180, 0, 0);
        }

        private uint LifeTime;

        protected override void OnUpdate(ObjectEnvironmentData data)
        {
            if(SimulationSettings.IsFoodDispawn)
            {
                LifeTime++;
            }
            if(LifeTime >= SimulationSettings.FoodLifeTime)
            {
                ShallDestroyOnUpdate = true;
                return;
            }
        }

    }
}
