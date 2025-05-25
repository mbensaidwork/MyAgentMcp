using MyAgentMcp.Tools;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

public class McpAgent
{
    private readonly List<ITool> _tools;
    private readonly string _openAiApiKey = ""; // Remplacez par votre clé API
    private readonly string _openAiEndpoint = "https://api.openai.com/v1/chat/completions";

    public McpAgent(IEnumerable<ITool> tools)
    {
        _tools = tools.ToList();
    }

    public string GetToolsDefinition()
    {
        return JsonSerializer.Serialize(_tools.Select(t => new { t.Name, t.Description }));
    }

    // Simule l'envoi d'un prompt + contexte + outils à un LLM via MCP
    public async Task<string> SendPromptToLlmAsync(string prompt, string context)
    {
        var toolsDef = GetToolsDefinition();
        var systemPrompt = $"Tu es un agent qui choisit l'outil le plus adapté à partir de la liste suivante : {toolsDef}. Pour chaque question utilisateur, réponds uniquement avec un JSON de la forme: {{\"tool\":<nom outil>,\"arguments\":<arguments>}}.";
        var userPrompt = $"Contexte: {context}\nQuestion: {prompt}";
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);
        var body = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            max_tokens = 100,
            temperature = 0
        };
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync(_openAiEndpoint, content);
        var responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine("\n[DEBUG] Réponse brute du LLM :");
        Console.WriteLine(responseString);
        using var doc = System.Text.Json.JsonDocument.Parse(responseString);
        if (doc.RootElement.TryGetProperty("error", out var errorProp))
        {
            var errorMsg = errorProp.GetProperty("message").GetString();
            Console.WriteLine($"[ERREUR API OpenAI] : {errorMsg}");
            return "{\"tool\":\"\",\"arguments\":\"\"}";
        }
        var message = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        Console.WriteLine("[DEBUG] Contenu du message LLM :");
        Console.WriteLine(message);
        return message ?? "";
    }

    public string ExecuteToolFromLlmResponse(string llmResponse)
    {
        var doc = JsonDocument.Parse(llmResponse);
        var toolName = doc.RootElement.GetProperty("tool").GetString();
        var arguments = doc.RootElement.GetProperty("arguments").GetString();
        var tool = _tools.FirstOrDefault(t => t.Name == toolName);
        if (tool == null) return $"Outil '{toolName}' non trouvé.";
        return tool.Execute(arguments ?? "");
    }
}
