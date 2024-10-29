using SFML.Window;
using System;
using System.Collections.Generic;

namespace NeuralLife.Input
{
    public class SFMLInput : IInput
    {
        private Dictionary<Keys, bool> KeysStates = new Dictionary<Keys, bool>();
        private Dictionary<Keys, bool> PreviousFrameStates = new Dictionary<Keys, bool>();

        public void Update()
        {
            foreach(Keys key in Enum.GetValues(typeof(Keys)))
            {
                if(KeysStates.ContainsKey(key))
                {
                    PreviousFrameStates[key] = KeysStates[key];
                }
                else
                {
                    PreviousFrameStates[key] = false;
                }
                KeysStates[key] = Keyboard.IsKeyPressed((Keyboard.Key)key);
            }
        }

        public bool IsKeyPressed(Keys key)
        {
            return KeysStates[key];
        }

        public bool IsKeyDown(Keys key)
        {
            if(!PreviousFrameStates[key] && KeysStates[key])
            {
                return true;
            }
            return false;
        }
    }
}
