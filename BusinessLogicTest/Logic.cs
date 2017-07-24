using Aliq;

public static class Logic
{
    public static void Init(IDataBinding dataBinding)
    {
        dataBinding.Set(new ExternalInput<string>(), "input");
    }
}
