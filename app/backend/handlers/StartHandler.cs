namespace DeckStudio.Handlers;

public class StartHandler : IStateHandler
{
    public (string response, object? jobSpec, ConversationState newState) Handle(UserMessageWithId message, ConversationState state)
    {
        if (message.Message.Trim().Equals("I want bank data", StringComparison.OrdinalIgnoreCase))
        {
            return ("Which bank?", null, state with { Step = "awaiting_bank" });
        }
        else
        {
            return ("Please say 'I want bank data' to begin.", null, state);
        }
    }
}
