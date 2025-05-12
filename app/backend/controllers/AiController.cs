using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

[ApiController]
[Route("ai")]
public class AiController : ControllerBase
{
    private static readonly ConcurrentDictionary<string, ConversationState> conversationStates = new();

    [HttpPost]
    public IActionResult Post([FromBody] UserMessageWithId message)
    {
        string convId = message.Id ?? Guid.NewGuid().ToString();
        var state = conversationStates.GetOrAdd(convId, _ => new ConversationState("start"));
        string response = "";
        object? jobSpec = null;
        switch (state.Step)
        {
            case "start":
                if (message.Message.Trim().Equals("I want bank data", StringComparison.OrdinalIgnoreCase))
                {
                    conversationStates[convId] = state with { Step = "awaiting_bank" };
                    response = "Which bank?";
                }
                else
                {
                    response = "Please say 'I want bank data' to begin.";
                }
                break;
            case "awaiting_bank":
                conversationStates[convId] = state with { Step = "awaiting_data_type", Bank = message.Message.Trim() };
                response = "What do you need: transactions, balances, or both?";
                break;
            case "awaiting_data_type":
                if (string.IsNullOrEmpty(state.Bank))
                {
                    response = "Please specify a bank first.";
                    break;
                }
                if (message.Message.Trim().Equals("Transactions", StringComparison.OrdinalIgnoreCase))
                {
                    jobSpec = new {
                        type = "bank-data-request",
                        bank = state.Bank,
                        data = "transactions",
                        date_range = new { from = "2024-01-01", to = "2024-01-31" },
                        user_id = "user-123"
                    };
                    conversationStates[convId] = state with { Step = "awaiting_confirm", PendingJobSpec = jobSpec };
                    response = "Here is your job spec. Type 'confirm' to submit.";
                }
                else
                {
                    response = "Sorry, only 'Transactions' is supported in this demo.";
                }
                break;
            case "awaiting_confirm":
                response = "Review your job spec and use the Confirm button to submit.";
                jobSpec = state.PendingJobSpec;
                break;
            case "done":
                response = "This conversation is complete. Start a new one to create another job.";
                break;
            default:
                response = "Sorry, I didn't understand that.";
                break;
        }
        return Ok(new { response, jobSpec, id = convId });
    }
}
