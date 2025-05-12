using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DeckStudio.Handlers;

[ApiController]
[Route("ai")]
public class AiController : ControllerBase
{
    private static readonly ConcurrentDictionary<string, ConversationState> conversationStates = new();

    private static readonly Dictionary<string, IStateHandler> handlers = new()
    {
        { "start", new StartHandler() },
        { "awaiting_bank", new AwaitingBankHandler() },
        { "awaiting_data_type", new AwaitingDataTypeHandler() },
        { "awaiting_confirm", new AwaitingConfirmHandler() },
        { "done", new DoneHandler() }
    };

    [HttpPost]
    public IActionResult Post([FromBody] UserMessageWithId message)
    {
        string convId = message.Id ?? Guid.NewGuid().ToString();
        var state = conversationStates.GetOrAdd(convId, _ => new ConversationState("start"));
        string response;
        object? jobSpec;
        ConversationState newState = state;

        if (handlers.TryGetValue(state.Step, out var handler))
        {
            (response, jobSpec, newState) = handler.Handle(message, state);
        }
        else
        {
            response = "Sorry, I didn't understand that.";
            jobSpec = null;
        }
        conversationStates[convId] = newState;
        return Ok(new { response, jobSpec, id = convId });
    }
}
