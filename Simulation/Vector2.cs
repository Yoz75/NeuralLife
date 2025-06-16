
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace NeuralLife.Simulation
{
    public struct Vector2<T> where T : INumber<T>
    {
        public T X;
        public T Y;

        public Vector2(T x, T y)
        {
            X = x;
            Y = y;
        }

        public static Vector2<T> Zero => new Vector2<T>(T.Zero, T.Zero);
        public static bool operator ==(Vector2<T> left, Vector2<T> right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Vector2<T> left, Vector2<T> right)
        {
            return !(left == right);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return this == (Vector2<T>)obj;
        }

        public override int GetHashCode()
        { 
            return HashCode.Combine(X.GetHashCode(), Y.GetHashCode());
        }
    }
}
