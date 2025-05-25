namespace MyAgentMcp.Tools;

public class JiraTool : ITool
{
    public string Name => "jira";
    public string Description => "Créer un ticket Jira. Argument: description du ticket.";
    public string Execute(string arguments)
    {
        // Simule la création d'un ticket Jira
        return $"Ticket Jira créé avec la description : {arguments}";
    }
}
