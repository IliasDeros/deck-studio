namespace DeckStudio.Handlers;

public static class AwaitingBankHandler
{
    public static (string response, object? jobSpec, ConversationState newState) Handle(UserMessageWithId message, ConversationState state)
    {
        return ("What do you need: transactions, balances, or both?", null, state with { Step = "awaiting_data_type", Bank = message.Message.Trim() });
    }
}
