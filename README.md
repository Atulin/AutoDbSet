[![NuGet Version](https://img.shields.io/nuget/v/Atulin.NpgsqlSourceGenerator?style=for-the-badge)](https://www.nuget.org/packages/Atulin.NpgSqlSourceGenerator/)
![NuGet Downloads](https://img.shields.io/nuget/dt/Atulin.NpgsqlSourceGenerator?style=for-the-badge)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/Atulin/AutoDbSet/nuget.yml?style=for-the-badge)
[![GitHub License](https://img.shields.io/github/license/Atulin/NpgsqlSourceGenerators?style=for-the-badge)](./LICENSE)

# AutoDbSet

Automagically add `DbSet<T>`s to your `DbContext`

## Usage

Place `[AutoDbSet]` attribute on the database models you want to register...

```cs
using AutoDbSetGenerators;

namespace AutoDbSet.Demo;

[AutoDbSet]
public class Person
{
	public required string Name { get; set; }
	public required DateOnly Birthday { get; set; }
	public required float Height { get; set; }
}
```

Place `[AutoDbContext]` attribute on your `DbContext` and make it `partial`...

```cs
using AutoDbSetGenerators;
using Microsoft.EntityFrameworkCore;

namespace AutoDbSet.Demo;

[AutoDbContext]
public partial class MyCoolDbContext : DbContext
{
}
```

And watch the magic happen!

```cs
namespace AutoDbSet.Demo;

[System.Runtime.CompilerServices.CompilerGeneratedAttribute]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
[global::System.CodeDom.Compiler.GeneratedCode("AutoDbSet", "1.0.0.0")]
public partial class MyCoolDbContext
{
    public required Microsoft.EntityFrameworkCore.DbSet<AutoDbSet.Demo.Person> Persons { get; init; }
}
```
