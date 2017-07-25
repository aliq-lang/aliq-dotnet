using Aliq;

public static class Logic
{
    public static void Init(IDataBinding dataBinding)
    {
        var a = new ExternalInput<string>();
        dataBinding.Set(a, "input");
        dataBinding.Set(a.Select(v => v.Length).Average(), "output");
    }
}
