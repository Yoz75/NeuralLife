
using System;

namespace NeuralLife.Simulation
{
    public class SimulationObject
    {
        public string Name;
        public Color Color;
        public bool ShallDestroyOnUpdate;

        public virtual void Update(ObjectEnvironmentData data)
        {
            if(ShallDestroyOnUpdate)
            {
                data.Simulation.DestroyAtPosition(data.Position);
                return;
            }
            return;
        }

    }
}
