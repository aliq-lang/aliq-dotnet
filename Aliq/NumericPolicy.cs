using System;

namespace Aliq
{
    public struct NumericPolicy : INumericPolicy<decimal>, INumericPolicy<double>
    {
        public static NumericPolicy Instance => new NumericPolicy();

        decimal INumericPolicy<decimal>.Add(decimal a, decimal b) 
            => a + b;

        double INumericPolicy<double>.Add(double a, double b)
            => a + b;

        decimal INumericPolicy<decimal>.Div(decimal a, decimal b)
            => a / b;

        double INumericPolicy<double>.Div(double a, double b)
            => a / b;

        decimal INumericPolicy<decimal>.FromLong(long v)
            => v;

        double INumericPolicy<double>.FromLong(long v)
            => v;
    }
}
