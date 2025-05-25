namespace MyAgentMcp.Tools;

public interface ITool
{
    string Name { get; }
    string Description { get; }
    string Execute(string arguments);
}
