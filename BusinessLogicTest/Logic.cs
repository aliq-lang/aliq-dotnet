using Aliq;

public static class Logic
{
    public static void Init(IDataBinding dataBinding)
    {
        var a = new ExternalInput<string>();
        var b = a.Select(v => v.Length).Select(v => (double)v).Average();
        var r = b.Select(v => "(" + v + ")");
        var z = r.Select(v => (long)v.Length).Sum();
        var c = r.Count();
        var x = a.Where(v => v.StartsWith("a")).Distinct();

        dataBinding.Set(a, "a");
        dataBinding.Set(b, "b");
        dataBinding.Set(x, "x");
        dataBinding.Set(z, "z");
        dataBinding.Set(c, "c");
    }
}
