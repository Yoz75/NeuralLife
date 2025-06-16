
namespace NeuralLife
{
    public static class SimulationSettings
    {        
        public static bool EnableFoodDispawn = true;
        public static uint FoodLifeTime = 50;
        public static float FoodSpawnCount = 0.01f;
        public static bool AllowColonialism = false;
        //I don`t know how to name it, but:
        //false - bukashki divide at health of 50
        //true - bukashki divide when WillDivide > 0
        public static bool EnableAutoDivide = false;
    }
}
