using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AutoDbSet;

[Generator]
public class AutoDbSetIncrementalSourceGenerator : IIncrementalGenerator
{
	private const string Namespace = "AutoDbSetGenerators";
	private const string AutoDbSetAttributeName = "AutoDbSetAttribute";
	private const string DbContextAttributeName = "AutoDbContextAttribute";

	private const string Header = // lang=cs
		$"""
		 //------------------------------------------------------------------------------
		 // <auto-generated>
		 //     This code was generated by a tool {ThisAssembly.Info.Title}.
		 //     Runtime Version: {ThisAssembly.Info.Version}
		 //
		 //     Changes to this file may cause incorrect behavior and will be lost if
		 //     the code is regenerated.
		 // </auto-generated>
		 //------------------------------------------------------------------------------
		 """;

	private const string AutoDbSetAttributeSourceCode = // lang=cs
		$$"""
		  {{Header}}
		  #nullable enable
		  namespace {{Namespace}};
		                                           
		  /// <summary>
		  /// Marks the class to generate a DbSet out of
		  /// </summary>
		  [global::System.AttributeUsage(System.AttributeTargets.Class)]
		  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
		  [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		  [global::System.CodeDom.Compiler.GeneratedCode("{{ThisAssembly.Info.Title}}", "{{ThisAssembly.Info.Version}}")]
		  public sealed class {{AutoDbSetAttributeName}} : global::System.Attribute
		  {
		      /// <summary>
		      /// Sets name to be used in the database.
		      /// </summary>
		      public string? Name { get; set; }
		  }
		  #nullable restore   
		  """;

	private const string DbContextAttributeSourceCode = // lang=cs
		$$"""
		  {{Header}}
		  #nullable enable
		  namespace {{Namespace}};
		                                           
		  /// <summary>
		  /// Marks the DbContext to generate DbSets into
		  /// </summary>
		  [global::System.AttributeUsage(System.AttributeTargets.Class)]
		  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
		  [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		  [global::System.CodeDom.Compiler.GeneratedCode("{{ThisAssembly.Info.Title}}", "{{ThisAssembly.Info.Version}}")]
		  public sealed class {{DbContextAttributeName}} : global::System.Attribute
		  {}
		  #nullable restore   
		  """;

	private record struct DbSetData(string Name, string Namespace, bool IsGlobalNamespace, string? CustomName);

	private record struct DbContextData(string Name, string Visibility, string Namespace);

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		// Add the marker attributes to the compilation.
		context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
			"AutoDbSetAttribute.g.cs",
			SourceText.From(AutoDbSetAttributeSourceCode, Encoding.UTF8)));

		context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
			"DbContextAttribute.g.cs",
			SourceText.From(DbContextAttributeSourceCode, Encoding.UTF8)));

		var dbSets = context.SyntaxProvider.ForAttributeWithMetadataName(
			$"{Namespace}.{AutoDbSetAttributeName}",
			(sn, _) => sn is ClassDeclarationSyntax,
			(gasc, _) => new DbSetData(
				gasc.TargetSymbol.Name,
				gasc.TargetSymbol.ContainingNamespace.ToDisplayString(),
				gasc.TargetSymbol.ContainingNamespace.IsGlobalNamespace,
				gasc.Attributes[0].NamedArguments.FirstOrDefault(kv => kv.Key == "Name").Value.Value as string
			)
		).Collect();

		var dbContext = context.SyntaxProvider.ForAttributeWithMetadataName(
			$"{Namespace}.{DbContextAttributeName}",
			(sn, _) => sn is ClassDeclarationSyntax,
			(gasc, _) => new DbContextData(
				gasc.TargetSymbol.Name,
				gasc.TargetSymbol.DeclaredAccessibility.ToString(),
				gasc.TargetSymbol.ContainingNamespace.ToDisplayString()
			)
		).Collect();

		// Generate the source code.
		context.RegisterSourceOutput(dbSets.Combine(dbContext), GenerateCode);
	}


	private static void GenerateCode(SourceProductionContext context, (ImmutableArray<DbSetData>, ImmutableArray<DbContextData>) data)
	{
		var (sets, ctx) = data;
		var dbContext = ctx.Single();

		var code = // lang=cs
			$$"""
			  {{Header}}
			  #nullable enable
			         
			  namespace {{dbContext.Namespace}};

			  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
			  [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
			  [global::System.CodeDom.Compiler.GeneratedCode("{{ThisAssembly.Info.Title}}", "{{ThisAssembly.Info.Version}}")]
			  {{dbContext.Visibility.ToLowerInvariant()}} partial class {{dbContext.Name}}
			  {
			  {{GenerateAllSets(sets)}}
			  }
			  #nullable restore   
			  """;

		context.AddSource($"{dbContext.Name}.g.cs", SourceText.From(code, Encoding.UTF8));
	}

	private static string GenerateDbSet(DbSetData data)
	{
		var ns = data.IsGlobalNamespace
			? "global::"
			: $"{data.Namespace}.";

		var propName = data.CustomName ?? data.Name.Pluralize();

		return $$"""public required Microsoft.EntityFrameworkCore.DbSet<{{ns}}{{data.Name}}> {{propName}} { get; init; }""";
	}

	private static string GenerateAllSets(ImmutableArray<DbSetData> data, int indent = 4)
	{
		var sb = new StringBuilder();
		foreach (var dbSet in data)
		{
			sb.Append(' ', indent);
			sb.AppendLine(GenerateDbSet(dbSet));
		}
		return sb.ToString();
	}
}