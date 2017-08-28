# ALIQ for .Net

Abstract Language Integrated Query for .Net

[![NuGet](https://img.shields.io/nuget/v/Aliq.svg)](https://www.nuget.org/packages/Aliq/)

### Creating a Business Logic Using ALIQ

Use the `Bag` type and extension methods from [Aliq/Linq/Extensions.cs](Aliq/Linq/Extensions.cs).

### Creating a Back-End Which Execute the Business-Logic

Implement a `Bag<T>.IVisitor` from [Aliq/Bags/Bag.cs](Aliq/Bags/Bag.cs).

## Back-End Examples

- Enumerable.
- Advanced Enumerable:
  - multi-threading (# of threads/nodes == # of processors).
  - cache tables that will be reused.

## For Developers

[![Build Status](https://travis-ci.org/sergey-shandar/aliq-dotnet.svg?branch=master)](https://travis-ci.org/sergey-shandar/aliq-dotnet)
[![Build status](https://ci.appveyor.com/api/projects/status/21j3blj8kuuftpo2?svg=true)](https://ci.appveyor.com/project/sergey-shandar/aliq-dotnet)
