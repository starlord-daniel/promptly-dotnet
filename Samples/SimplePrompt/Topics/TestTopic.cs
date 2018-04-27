using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using PromptlyBot;
using PromptlyBot.Prompts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplePrompt.Topics
{
    public class TestTopicState : ConversationTopicState
    {
        public string Sell { get; set; }
    }

    public class TestTopic : ConversationTopic<TestTopicState, string>
    {
        private const string SELL_PROMPT = "sellPrompt";

        public TestTopic() : base()
        {
            this.SubTopics.Add(SELL_PROMPT, (object[] args) =>
            {
                var namePrompt = new TextPrompt();

                namePrompt.Set
                    .OnPrompt("What do you want to sell?")
                    .OnSuccess((ctx, value) =>
                    {
                        this.ClearActiveTopic();

                        this.State.Sell = value;

                        this.OnReceiveActivity(ctx);
                    });

                return namePrompt;
            });
        }

        public override Task OnReceiveActivity(IBotContext context)
        {
            if (context.Request.Type == ActivityTypes.Message)
            {
                // Check to see if there is an active topic.
                if (this.HasActiveTopic)
                {
                    // Let the active topic handle this turn by passing context to it's OnReceiveActivity().
                    this.ActiveTopic.OnReceiveActivity(context);
                    return Task.CompletedTask;
                }

                if (this.State.Sell == null)
                {
                    this.SetActiveTopic("sellPrompt")
                        .OnReceiveActivity(context);
                    return Task.CompletedTask;
                }

                // Now that you have the state you need (age and name), use it!
                context.SendActivity($"You sold {this.State.Sell}");

                this.OnSuccess(context, this.State.Sell);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
