namespace AutoDbSet.Demo;

[AutoDbSetGenerators.AutoDbSet]
public class Puppy
{
	public required string Name { get; set; }
	public required float Weight { get; set; }
	public required int Age { get; set; }
}