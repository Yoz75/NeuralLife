using SFML.Window;

namespace NeuralLife.Input
{
    public class SFMLInput : IInput
    {
        public bool IsKeyPressed(Keys key)
        {
            if(Keyboard.IsKeyPressed((Keyboard.Key)key))
            {
                return true;
            }
            return false;
        }
    }
}
