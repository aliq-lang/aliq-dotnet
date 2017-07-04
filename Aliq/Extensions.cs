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
            => input.Select(v => v.Numeric<T, P>());

        public static Bag<Numeric<decimal, DecimalPolicy>> NumericBag(this Bag<decimal> input)
            => input.NumericBag(default(DecimalPolicy));

        public static Bag<Numeric<double, DoublePolicy>> NumericBag(this Bag<double> input)
            => input.NumericBag(default(DoublePolicy));

        public static Bag<Numeric<int, IntPolicy>> NumericBag(this Bag<int> input)
            => input.NumericBag(default(IntPolicy));

        public static Bag<Numeric<long, LongPolicy>> NumericBag(this Bag<long> input)
            => input.NumericBag(default(LongPolicy));

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

        public static Bag<Numeric<T, P>> NumericBag<T, P>(this Bag<T?> input, P _ = default(P))
            where P : struct, INumericPolicy<T>
            where T : struct
            => input.NotNullValues().NumericBag<T, P>();

        public static Bag<Numeric<decimal, DecimalPolicy>> NumericBag(this Bag<decimal?> input)
            => input.NumericBag(default(DecimalPolicy));

        public static Bag<Numeric<double, DoublePolicy>> NumericBag(this Bag<double?> input)
            => input.NumericBag(default(DoublePolicy));

        public static Bag<Numeric<int, IntPolicy>> NumericBag(this Bag<int?> input)
            => input.NumericBag(default(IntPolicy));

        public static Bag<Numeric<long, LongPolicy>> NumericBag(this Bag<long?> input)
            => input.NumericBag(default(LongPolicy));

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

        public static Bag<decimal?> Average(this Bag<decimal?> input)
            => input.NumericBag().NullableAverage();

        public static Bag<double?> Average(this Bag<double?> input)
            => input.NumericBag().NullableAverage();

        public static Bag<int?> Average(this Bag<int?> input)
            => input.NumericBag().NullableAverage();

        public static Bag<long?> Average(this Bag<long?> input)
            => input.NumericBag().NullableAverage();
    }
}
