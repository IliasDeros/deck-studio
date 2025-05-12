namespace DeckStudio.Handlers;

public class AwaitingConfirmHandler : IStateHandler
{
    public (string response, object? jobSpec, ConversationState newState) Handle(UserMessageWithId message, ConversationState state)
    {
        return ("Review your job spec and use the Confirm button to submit.", state.PendingJobSpec, state);
    }
}
