namespace AutoDbSet.Demo;

[AutoDbSetGenerators.AutoDbSet]
public class Veto
{
	public required string Name { get; set; }
	public required DateTime VetoDate { get; set; }
}