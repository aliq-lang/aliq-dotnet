using System.Linq.Expressions;

namespace Aliq
{
    public static class CompiledExpression
    {
        public static CompiledExpression<T> Compiled<T>(this Expression<T> expression)
            => new CompiledExpression<T>(expression);
    }

    public sealed class CompiledExpression<T>
    {
        public CompiledExpression(Expression<T> expression)
        {
            Expression = expression;
            Compiled = expression.Compile();
        }

        public Expression<T> Expression { get; }

        public T Compiled { get; }
    }
}
