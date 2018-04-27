using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using PromptlyBot;
using System.Threading.Tasks;
using PromptlyBot.Prompts;

namespace SimplePrompt.Topics
{
    public class RootTopicState : ConversationTopicState
    {
        public string Name { get; set; }
        public int? Age { get; set; }
    }

    public class RootTopic : TopicsRoot<BotConversationState, RootTopicState>
    {
        private const string TEST_TOPIC = "testTopic";
        private const string INFO_TOPIC = "infoTopic";
        private static ITopic interruptedTopic;

        public RootTopic(IBotContext context) : base(context)
        {
            this.SubTopics.Add(TEST_TOPIC, (object[] args) =>
            {
                var testTopic = new TestTopic();

                testTopic.Set
                    .OnSuccess((ctx, value) =>
                    {
                        this.ClearActiveTopic();

                        this.State.Name = value;

                        this.OnReceiveActivity(ctx);
                    });

                return testTopic;
            });

            this.SubTopics.Add(INFO_TOPIC, (object[] args) =>
            {
                var infoTopic = new InfoTopic();

                infoTopic.Set
                    .OnSuccess((ctx, value) =>
                    {
                        this.ClearActiveTopic();
                        interruptedTopic = null;

                        this.State.Name = value;
                        this.OnReceiveActivity(ctx);
                    });

                return infoTopic;
            });
        }

        public override Task OnReceiveActivity(IBotContext context)
        {
            if (context.Request.Type == ActivityTypes.Message)
            {
                var message = context.Request.AsMessageActivity();

                // If the user wants to change the topic of conversation...
                if (message.Text.ToLowerInvariant() == "test")
                {
                    // Set the active topic and let the active topic handle this turn.
                    this.SetActiveTopic(TEST_TOPIC)
                            .OnReceiveActivity(context);
                    return Task.CompletedTask;
                }

                if (message.Text.ToLowerInvariant() == "topic")
                {
                    this.SetActiveTopic(INFO_TOPIC)
                        .OnReceiveActivity(context);
                    return Task.CompletedTask;
                }

                if (message.Text == "interrupt")
                {
                    interruptedTopic = ActiveTopic;

                    this.SetActiveTopic(TEST_TOPIC)
                        .OnReceiveActivity(context);

                    return Task.CompletedTask;
                }

                // Check to see if there is an active topic.
                if (this.HasActiveTopic)
                {
                    // Let the active topic handle this turn by passing context to it's OnReceiveActivity().
                    this.ActiveTopic.OnReceiveActivity(context);
                    return Task.CompletedTask;
                }

                if(interruptedTopic != null)
                {
                    this.SetActiveTopic(interruptedTopic)
                        .OnReceiveActivity(context);

                    return Task.CompletedTask;
                }

                // Now that you have the state you need (age and name), use it!
                context.SendActivity($"Please pick an option: test or topic");
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}