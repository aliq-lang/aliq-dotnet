# Protocol For Distributed Back-End

## Distributed Back-End Interface

```cs
interface IBinding
{
	void Set<T>(Bag<T> bag, string objectId);
}
```

Where `objectId` is a name of data object, for example a file name.

## Binding

A DLL should have a function `Aliq.Init(Aliq.IBinding binding)` which is called on each node.