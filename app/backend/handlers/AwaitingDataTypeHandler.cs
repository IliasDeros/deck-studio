namespace DeckStudio.Handlers;

public class AwaitingDataTypeHandler : IStateHandler
{
    public (string response, object? jobSpec, ConversationState newState) Handle(UserMessageWithId message, ConversationState state)
    {
        if (string.IsNullOrEmpty(state.Bank))
        {
            return ("Please specify a bank first.", null, state);
        }
        if (message.Message.Trim().Equals("Transactions", StringComparison.OrdinalIgnoreCase))
        {
            var jobSpec = new {
                type = "bank-data-request",
                bank = state.Bank,
                data = "transactions",
                date_range = new { from = "2024-01-01", to = "2024-01-31" },
                user_id = "user-123"
            };
            return ("Here is your job spec. Type 'confirm' to submit.", jobSpec, state with { Step = "awaiting_confirm", PendingJobSpec = jobSpec });
        }
        else
        {
            return ("Sorry, only 'Transactions' is supported in this demo.", null, state);
        }
    }
}
