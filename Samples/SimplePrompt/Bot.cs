using Microsoft.Bot;
using PromptlyBot;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using SimplePrompt.Topics;
using System.Threading;
using System;

namespace SimplePrompt
{
    

    public class BotConversationState : PromptlyBotConversationState<RootTopicState>
    {
    }

    public class Bot : IBot
    {
        [NonSerialized]
        Timer t;

        public Task OnReceiveActivity(IBotContext context)
        {
            var rootTopic = new RootTopic(context);
            
            rootTopic.OnReceiveActivity(context);

            return Task.CompletedTask;
        }
    }
}
