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
    public class InfoTopicState : ConversationTopicState
    {
        public string Name { get; set; }

        public int? Age { get; set; }
    }

    public class InfoTopic : ConversationTopic<InfoTopicState, string>
    {
        private const string AGE_PROMPT = "agePrompt";
        private const string NAME_PROMPT = "namePrompt";

        public InfoTopic() : base()
        {
            this.SubTopics.Add(AGE_PROMPT, (object[] args) =>
            {
                var agePrompt = new IntPrompt();

                agePrompt.Set
                    .OnPrompt("How old are you?")
                    .OnSuccess((ctx, value) =>
                    {
                        this.ClearActiveTopic();

                        this.State.Age = value;

                        this.OnReceiveActivity(ctx);
                    });

                return agePrompt;
            });

            this.SubTopics.Add(NAME_PROMPT, (object[] args) =>
            {
                var namePrompt = new TextPrompt();

                namePrompt.Set
                    .OnPrompt("What is your name?")
                    .OnSuccess((ctx, value) =>
                    {
                        this.ClearActiveTopic();

                        this.State.Name = value;

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

                // If you don't have the state you need, prompt for it
                if (this.State.Name == null)
                {
                    this.SetActiveTopic("namePrompt")
                        .OnReceiveActivity(context);
                    return Task.CompletedTask;
                }

                if (this.State.Age == null)
                {
                    this.SetActiveTopic("agePrompt")
                        .OnReceiveActivity(context);
                    return Task.CompletedTask;
                }

                // Now that you have the state you need (age and name), use it!
                context.SendActivity($"Hello { this.State.Name }! You are { this.State.Age } years old.");

                this.OnSuccess(context, this.State.Name);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
