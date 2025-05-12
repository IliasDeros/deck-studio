namespace DeckStudio.Handlers;

public interface IStateHandler
{
    (string response, object? jobSpec, ConversationState newState) Handle(UserMessageWithId message, ConversationState state);
}
