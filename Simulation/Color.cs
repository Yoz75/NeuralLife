
using System.Runtime.CompilerServices;

namespace NeuralLife.Simulation
{
    public struct Color
    {
        public const int SelfSize = 3;
        public byte R;
        public byte G;
        public byte B;
        public Color()
        {

        }

        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint AsUint()
        {
            return (uint)(R << 24 | G << 16 | B << 8 | 255);
        }
    }
}
