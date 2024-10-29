
using System;

namespace NeuralLife.Simulation
{
    public class SimulationObject
    {
        public string Name;
        public Color Color;
        public bool ShallDestroyOnUpdate;

        public void Update(ObjectEnvironmentData data)
        {
            if(ShallDestroyOnUpdate)
            {
                data.Simulation.DestroyAtPosition(data.Position);
                return;
            }
            OnUpdate(data);
            return;
        }

        protected virtual void OnUpdate(ObjectEnvironmentData data)
        {
            return;
        }

    }
}
