using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace PromptlyBot.Topics
{
    public class ActiveTopicState
    {
        public string Key;
        public object State;
    }

    // TODO:
    // New Topic can come in and pause the current conversation/Prompt
    // After new prompt is done, the old one is resumed with the old OnReceive

    // Store old OnReceive
    // Put it into Topic
    // Execute it, if oldOnreceiveActivities is not empty

    public abstract class ResumableConversationTopic<TState> : ConversationTopic<TState> where TState : ConversationTopicState, new()
    {

    }
}
