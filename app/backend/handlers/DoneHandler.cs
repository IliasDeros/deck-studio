namespace DeckStudio.Handlers;

public static class DoneHandler
{
    public static (string response, object? jobSpec, ConversationState newState) Handle(UserMessageWithId message, ConversationState state)
    {
        return ("This conversation is complete. Start a new one to create another job.", null, state);
    }
}
