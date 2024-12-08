namespace AutoDbSet.Demo;

[AutoDbSetGenerators.AutoDbSet(Name = "People")]
public class Person
{
	public required string Name { get; set; }
	public required DateOnly Birthday { get; set; }
	public required float Height { get; set; }
}