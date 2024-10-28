
using System.Runtime.CompilerServices;

namespace NeuralLife.Simulation
{
    public struct Color
    {

        public Color()
        {

        }

        public Color(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint AsUint()
        {
            return (uint)(R << 24 | G << 16 | B << 8 | A << 0);
        }

        public byte R;
        public byte G;
        public byte B;
        public byte A;
    }
}
