namespace MyAgentMcp.Tools;

public class WeatherTool : ITool
{
    public string Name => "weather";
    public string Description => "Obtenir la météo pour une ville donnée. Argument: nom de la ville.";
    public string Execute(string arguments)
    {
        // Simule une réponse météo
        return $"La météo à {arguments} est ensoleillée.";
    }
}
