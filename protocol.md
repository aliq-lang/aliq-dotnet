# Protocol For Distributed Back-End

## Distributed Back-End Interface

```cs
interface IBinding
{
	void SetInput<T>(ExternalInput<T> input, string objectId);
	void SetOutput<T>(Bag<T> output, string objectId);
}
```

## Binding

A DLL should have a function `Aliq.Init(Aliq.IBinding binding)` which is called on each node.