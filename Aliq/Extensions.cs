using NumericPolicies;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Expression<Func<T, T, T>> reduce,
            IEqualityComparer<K> comparer)
            => new GroupBy<T, K>(input, getKey, reduce, comparer);

        public static GroupBy<T, K> GroupBy<T, K>(
            this Bag<T> input,
            Expression<Func<T, K>> getKey,
            Expression<Func<T, T, T>> reduce)
            => input.GroupBy(getKey, reduce, EqualityComparer<K>.Default);

        public static DisjointUnion<T> DisjointUnion<T>(this Bag<T> a, Bag<T> b)
            => new DisjointUnion<T>(a, b);

        public static SelectMany<T, I> SelectMany<T, I>(
            this Bag<I> input, Expression<Func<I, IEnumerable<T>>> map)
            => new SelectMany<T, I>(input, map);

        public static Bag<T> Select<T, I>(
            this Bag<I> input, Expression<Func<I, T>> map)
            => input.SelectMany(i => new[] { map.Compile()(i) });

        public static Bag<T> Where<T>(this Bag<T> input, Expression<Func<T, bool>> p)
            => input.SelectMany(v => p.Compile()(v) ? new[] { v } : Enumerable.Empty<T>());

        public static Bag<T> Aggregate<T>(this Bag<T> input, Expression<Func<T, T, T>> reduce)
            => input.GroupBy(_ => default(Void), reduce);

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

        public static Bag<Numeric<T, P>> NumericBag<T, P>(this Bag<T> input, P _ = default(P))
            where P : struct, INumericPolicy<T>
            => input.Select(v => v.Numeric(default(P)));

        public static Bag<Numeric<decimal, DecimalPolicy>> NumericBag(this Bag<decimal> input)
            => input.NumericBag(default(DecimalPolicy));

        public static Bag<Numeric<double, DoublePolicy>> NumericBag(this Bag<double> input)
            => input.NumericBag(default(DoublePolicy));

        public static Bag<Numeric<int, Int32Policy>> NumericBag(this Bag<int> input)
            => input.NumericBag(default(Int32Policy));

        public static Bag<Numeric<long, Int64Policy>> NumericBag(this Bag<long> input)
            => input.NumericBag(default(Int64Policy));

        public static Bag<Numeric<float, SinglePolicy>> NumericBag(this Bag<float> input)
            => input.NumericBag(default(SinglePolicy));

        public static Bag<MomentList1<T, P>> MomentList1<T, P>(this Bag<Numeric<T, P>> input)
            where P : struct, INumericPolicy<T>
            => input
                .Select(v => v.MomentList1())
                .Aggregate(0L.ToNumeric<T, P>().MomentList1(), (a, b) => a.Add(b));

        public static Bag<T> Average<T, P>(this Bag<Numeric<T, P>> input)
            where P : struct, INumericPolicy<T>
            => input
                .MomentList1()
                .Where(v => v.Count != 0)
                .Select(v => v.Average);

        public static T? Nullable<T>(this T value)
            where T : struct
            => new T?(value);

        public static Bag<T> NotNullValues<T>(this Bag<T?> input)
            where T : struct
            => input.SelectMany(v => v == null ? Enumerable.Empty<T>() : new[] { v.Value });

        public static Bag<T> NotNullValues<T, I>(this Bag<I> input, Expression<Func<I, T?>> map)
             where T : struct
            => input.Select(map).NotNullValues();

        public static Bag<T?> NullableAverage<T, P>(this Bag<Numeric<T, P>> input)
            where P : struct, INumericPolicy<T>
            where T : struct
            => input
                .MomentList1()
                .Select(v => v.Count != 0 ? v.Average.Nullable() : null);

        public static Bag<decimal> Average(this Bag<decimal> input)
            => input.NumericBag().Average();

        public static Bag<double> Average(this Bag<double> input)
            => input.NumericBag().Average();

        public static Bag<int> Average(this Bag<int> input)
            => input.NumericBag().Average();

        public static Bag<long> Average(this Bag<long> input)
            => input.NumericBag().Average();

        public static Bag<float> Average(this Bag<float> input)
            => input.NumericBag().Average();

        public static Bag<decimal?> Average(this Bag<decimal?> input)
            => input.NotNullValues().NumericBag().NullableAverage();

        public static Bag<double?> Average(this Bag<double?> input)
            => input.NotNullValues().NumericBag().NullableAverage();

        public static Bag<int?> Average(this Bag<int?> input)
            => input.NotNullValues().NumericBag().NullableAverage();

        public static Bag<long?> Average(this Bag<long?> input)
            => input.NotNullValues().NumericBag().NullableAverage();

        public static Bag<float?> Average(this Bag<float?> input)
            => input.NotNullValues().NumericBag().NullableAverage();

        public static Bag<decimal> Average<T>(
            this Bag<T> input, Expression<Func<T, decimal>> map)
            => input.Select(map).NumericBag().Average();

        public static Bag<double> Average<T>(
            this Bag<T> input, Expression<Func<T, double>> map)
            => input.Select(map).NumericBag().Average();

        public static Bag<int> Average<T>(
            this Bag<T> input, Expression<Func<T, int>> map)
            => input.Select(map).NumericBag().Average();

        public static Bag<long> Average<T>(
            this Bag<T> input, Expression<Func<T, long>> map)
            => input.Select(map).NumericBag().Average();

        public static Bag<float> Average<T>(
            this Bag<T> input, Expression<Func<T, float>> map)
            => input.Select(map).NumericBag().Average();

        public static Bag<decimal?> Average<T>(
            this Bag<T> input, Expression<Func<T, decimal?>> map)
            => input.NotNullValues(map).NumericBag().NullableAverage();

        public static Bag<double?> Average<T>(
            this Bag<T> input, Expression<Func<T, double?>> map)
            => input.NotNullValues(map).NumericBag().NullableAverage();

        public static Bag<int?> Average<T>(
            this Bag<T> input, Expression<Func<T, int?>> map)
            => input.NotNullValues(map).NumericBag().NullableAverage();

        public static Bag<long?> Average<T>(
            this Bag<T> input, Expression<Func<T, long?>> map)
            => input.NotNullValues(map).NumericBag().NullableAverage();

        public static Bag<float?> Average<T>(
            this Bag<T> input, Expression<Func<T, float?>> map)
            => input.NotNullValues(map).NumericBag().NullableAverage();

        public static Bag<bool> Contains<T>(Bag<T> input, T value)
            => input.Any(v => v.Equals(value));

        public static Bag<T> Sum<T, P>(this Bag<Numeric<T, P>> input)
            where P : struct, INumericPolicy<T>
            => input
                .Aggregate(0L.ToNumeric<T, P>(), (a, b) => a.Add(b))
                .Select(v => v.Value);

        public static Bag<long> Sum(this Bag<long> input)
            => input.NumericBag(default(Int64Policy)).Sum();

        public static Bag<long> Count<T>(this Bag<T> input)
            => input.Select(_ => 1L).Sum();

        public static Bag<long> Count<T>(
            this Bag<T> input, Expression<Func<T, bool>> p)
            => input.Where(p).Count();

        public static Bag<T> DefaultIfEmpty<T>(this Bag<T> input, T value = default(T))
            => input
                .Any()
                .Where(v => !v)
                .Select(_ => value)
                .DisjointUnion(input);

        public static NumberOf<T> NumberOf<T>(this T value, long count)
            => new NumberOf<T>(value, count);

        public static NumberOf<T> NumberOf<T>(this T value)
            => value.NumberOf(1);

        public static Bag<NumberOf<T>> ToNumberOf<T>(this Bag<T> input, long count = 1)
            => input.Select(v => v.NumberOf<T>(count));

        public static Bag<NumberOf<T>> Group<T>(
            this Bag<NumberOf<T>> input, IEqualityComparer<T> comparer)
            => input.GroupBy(v => v.Value, (a, b) => a.Add(b.Count), comparer);

        public static Bag<T> Distinct<T>(this Bag<T> input, IEqualityComparer<T> comparer)
            => input
                .ToNumberOf()
                .Group(comparer)
                .Select(v => v.Value);

        public static Bag<T> Distinct<T>(this Bag<T> input)
            => input.Distinct(EqualityComparer<T>.Default);

        public static Bag<T> Except<T>(
            this Bag<T> input, Bag<T> b, IEqualityComparer<T> comparer)
            => input
                .ToNumberOf()
                .DisjointUnion(b.ToNumberOf(-1))
                .Group(comparer)
                .SelectMany(v => v.Repeat());

        public static Bag<T> Except<T>(this Bag<T> input, Bag<T> b)
            => input.Except(b, EqualityComparer<T>.Default);
    }
}
