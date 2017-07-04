using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aliq
{
    public static class Extensions
    {
        public static Const<T> ToConstBag<T>(this T value)
            => new Const<T>(value);

        public static GroupBy<T, K> GroupBy<T, K>(
            this Bag<T> input,
            Expression<Func<T, K>> getKey,
            Expression<Func<T, T, T>> reduce)
            => new GroupBy<T, K>(input, getKey, reduce);


        public static DisjointUnion<T> DisjointUnion<T>(this Bag<T> a, Bag<T> b)
            => new DisjointUnion<T>(a, b);

        public static SelectMany<T, I> SelectMany<T, I>(
            this Bag<I> input, Expression<Func<I, IEnumerable<T>>> map)
            => new SelectMany<T, I>(input, map);

        public static Bag<T> Select<T, I>(
            this Bag<I> input, Expression<Func<I, T>> map)
            => input.SelectMany(i => new[] { map.Compile()(i) });

        public static Bag<T> Aggregate<T>(this Bag<T> input, Expression<Func<T, T, T>> reduce)
            => input.GroupBy(_ => Void.Instance, reduce);

        public static Bag<T> Aggregate<T>(
            this Bag<T> input, T value, Expression<Func<T, T, T>> reduce)
            => value.ToConstBag().DisjointUnion(input).Aggregate(reduce);

        public static Bag<R> Aggregate<T, R>(
            this Bag<T> input,
            T value,
            Expression<Func<T, T, T>> reduce,
            Expression<Func<T, R>> map)
            => input.Aggregate(value, reduce).Select(map);

        public static Bag<bool> All<T>(this Bag<T> input, Expression<Func<T, bool>> map)
            => input.Select(map).Aggregate(true, (a, b) => a && b);

        public static Bag<bool> Any<T>(this Bag<T> input, Expression<Func<T, bool>> map)
            => input.Select(map).Aggregate(false, (a, b) => a || b);

        public static Bag<bool> Any<T>(this Bag<T> input)
            => input.Any(_ => true);

        public static Bag<T> BagAverage<P, T>(this P policy, Bag<T> input)
            where P : struct, INumericPolicy<T>
            => input
                .Select(v => Tuple.Create(1, v))
                .Aggregate((a, b) => Tuple.Create(a.Item1 + b.Item1, new P().Add(a.Item2, b.Item2)))
                .Select(v => new P().Div(v.Item2, new P().FromLong(v.Item1)));

        public static Bag<decimal> Average<T>(this Bag<decimal> input)
            => NumericPolicy.Instance.BagAverage(input);

        public static Bag<double> Average<T>(this Bag<double> input)
            => NumericPolicy.Instance.BagAverage(input);
    }
}
