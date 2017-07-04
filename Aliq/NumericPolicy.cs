using System;

namespace Aliq
{
    public struct NumericPolicy : 
        INumericPolicy<decimal>, 
        INumericPolicy<double>, 
        INumericPolicy<int>,
        INumericPolicy<long>
    {
        public static NumericPolicy Instance => new NumericPolicy();

        decimal INumericPolicy<decimal>.Add(decimal a, decimal b) 
            => a + b;

        double INumericPolicy<double>.Add(double a, double b)
            => a + b;

        int INumericPolicy<int>.Add(int a, int b)
            => a + b;

        long INumericPolicy<long>.Add(long a, long b)
            => a + b;

        decimal INumericPolicy<decimal>.Div(decimal a, decimal b)
            => a / b;

        double INumericPolicy<double>.Div(double a, double b)
            => a / b;

        int INumericPolicy<int>.Div(int a, int b)
            => a / b;

        long INumericPolicy<long>.Div(long a, long b)
            => a / b;

        decimal INumericPolicy<decimal>.FromLong(long v)
            => v;

        double INumericPolicy<double>.FromLong(long v)
            => v;

        int INumericPolicy<int>.FromLong(long v)
            => (int)v;

        long INumericPolicy<long>.FromLong(long v)
            => v;
    }
}
