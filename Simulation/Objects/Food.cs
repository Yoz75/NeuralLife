
namespace NeuralLife.Simulation.Objects
{
    public class Food : SimulationObject
    {
        public static bool IsDispawn = true;
        public static uint UpdatesLifeTime = 15;

        public Food()
        {
            Color = new Color(180, 0, 0);
        }

        private uint LifeTime;

        protected override void OnUpdate(ObjectEnvironmentData data)
        {
            if(IsDispawn)
            {
                LifeTime++;
            }
            if(LifeTime >= UpdatesLifeTime)
            {
                ShallDestroyOnUpdate = true;
                return;
            }
        }

    }
}
