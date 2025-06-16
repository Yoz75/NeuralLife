
namespace NeuralLife
{
    /// <summary>
    /// Simulation render mode
    /// </summary>
    public enum RenderMode
    {
        /// <summary>
        /// None render mode. Choosing this mode is an error
        /// </summary>
        None = 0,
        /// <summary>
        /// Render color of every bukashka. Child's colors are some different from parent's color
        /// </summary>
        BaseColors
    }

    /// <summary>
    /// Class that has all info about screen
    /// </summary>
    public static class Screen
    {
        public const int Width = 160;
        public const int Height = 96;

        public static readonly RenderMode RenderMode =  RenderMode.BaseColors;
    }
}
