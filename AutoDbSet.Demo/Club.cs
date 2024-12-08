using System.Numerics;

namespace AutoDbSet.Demo;

[AutoDbSetGenerators.AutoDbSet]
public class Club
{
	public required string Name { get; set; }
	public required Vector2 Location { get; set; }
}