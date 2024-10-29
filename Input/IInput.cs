
namespace NeuralLife.Input
{
    public interface IInput
    {
        public void Update();
        public bool IsKeyPressed(Keys key);
        public bool IsKeyDown(Keys key);
    }
}
