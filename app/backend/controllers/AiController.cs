using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using DeckStudio.Handlers;

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
        ConversationState newState = state;
        switch (state.Step)
        {
            case "start":
                (response, jobSpec, newState) = StartHandler.Handle(message, state);
                break;
            case "awaiting_bank":
                (response, jobSpec, newState) = AwaitingBankHandler.Handle(message, state);
                break;
            case "awaiting_data_type":
                (response, jobSpec, newState) = AwaitingDataTypeHandler.Handle(message, state);
                break;
            case "awaiting_confirm":
                (response, jobSpec, newState) = AwaitingConfirmHandler.Handle(message, state);
                break;
            case "done":
                (response, jobSpec, newState) = DoneHandler.Handle(message, state);
                break;
            default:
                response = "Sorry, I didn't understand that.";
                break;
        }
        conversationStates[convId] = newState;
        return Ok(new { response, jobSpec, id = convId });
    }
}
