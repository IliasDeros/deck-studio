namespace DeckStudio.Handlers;

public static class AwaitingConfirmHandler
{
    public static (string response, object? jobSpec, ConversationState newState) Handle(UserMessageWithId message, ConversationState state)
    {
        return ("Review your job spec and use the Confirm button to submit.", state.PendingJobSpec, state);
    }
}
