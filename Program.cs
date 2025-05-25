// See https://aka.ms/new-console-template for more information
using MyAgentMcp.Tools;
using System.Threading.Tasks;

var tools = new List<ITool> { new WeatherTool(), new JiraTool() };
var agent = new McpAgent(tools);

Console.WriteLine("Agent MCP démarré.");
while (true)
{
    Console.WriteLine("Saisissez votre prompt (ou 'exit' pour quitter) :");
    string? input = Console.ReadLine();
    if (input == null)
    {
        Console.WriteLine("Aucune saisie détectée.");
        continue;
    }
    if (input.Trim().ToLower() == "exit")
    {
        Console.WriteLine("Au revoir !");
        break;
    }
    // Appel asynchrone à OpenAI
    string llmResponse = await agent.SendPromptToLlmAsync(input, "");
    string result = agent.ExecuteToolFromLlmResponse(llmResponse);
    Console.WriteLine($"Résultat : {result}");
}
