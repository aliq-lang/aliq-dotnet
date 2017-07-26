# ALIQ

Abstract Language Integrated Query

[![NuGet](https://img.shields.io/nuget/v/Aliq.svg)](https://www.nuget.org/packages/Aliq/)

### Creating a Business Logic Using ALIQ

Use the `Bag` type and extension methods from [Aliq\Extensions.cs](Aliq\Extensions.cs).

### Creating a Back-End Which Execute the Business-Logic

Implement a `Bag<T>.IVisitor` from [Aliq\Bag.cs](Aliq\Bag.cs).

## Back-End Examples

- Enumerable.
- Advanced Enumerable:
  - multi-threading (# of threads/nodes == # of processord).
  - cache tables that will be reused.

## For Developers

[![Build Status](https://travis-ci.org/sergey-shandar/aliq.svg?branch=master)](https://travis-ci.org/sergey-shandar/aliq)
[![Build status](https://ci.appveyor.com/api/projects/status/vcwvhs2fma3tmvwy?svg=true)](https://ci.appveyor.com/project/sergey-shandar/aliq)
