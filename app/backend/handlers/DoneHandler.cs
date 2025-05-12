namespace DeckStudio.Handlers;

public class DoneHandler : IStateHandler
{
    public (string response, object? jobSpec, ConversationState newState) Handle(UserMessageWithId message, ConversationState state)
    {
        return ("This conversation is complete. Start a new one to create another job.", null, state);
    }
}
