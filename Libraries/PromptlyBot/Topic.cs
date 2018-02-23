﻿using Microsoft.Bot.Builder;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PromptlyBot
{
    //[DataContract()]
    //[Serializable]
    public abstract class Topic<TState>
    {
        private TState _state;
        public TState State { get => _state; set => _state = value; }

        public Topic(TState state)
        {
            _state = state;
        }

        public abstract Task OnReceiveActivity(IBotContext context);

        private Action<IBotContext> _onSuccess;
        // TODO: Remove private set, unless needed for DataContract.
        //[DataMember]
        public Action<IBotContext> OnSuccess { get => _onSuccess; set => _onSuccess = value; }


        private Action<IBotContext, string> _onFailure;
        //[DataMember]
        public Action<IBotContext, string> OnFailure { get => _onFailure; set => _onFailure = value; }
    }

    /*public static class TopicExtension
    {
        public static T SetOnSuccess<T>(this T topic, Action<IBotContext> onSuccess) where T : Topic
        {
            topic._onSuccess = onSuccess;
            return topic;
        }

        public static T SetOnFailure<T>(this T topic, Action<IBotContext, string> onFailure) where T : Topic
        {
            topic._onFailure = onFailure;
            return topic;
        }
    }*/

    //[Serializable]
    //[DataContract]
    public abstract class Topic<TState, TValue> : Topic<TState>
    {
        public Topic(TState state) : base(state) { }

        private Action<IBotContext, TValue> _onSuccess;
        // TODO: Remove private set, unless needed for DataContract.
        [DataMember]
        public new Action<IBotContext, TValue> OnSuccess { get => _onSuccess; set => _onSuccess = value; }
    }

    /*public static class TopicTValueExtension
    {
        public static T SetOnSuccess<T, V>(this T topic, Action<IBotContext, V> onSuccess) where T: Topic<V>
        {
            topic._onSuccess = onSuccess;
            return topic;
        }

        public static T SetOnFailure<T, V>(this T topic, Action<IBotContext, string> onFailure) where T : Topic<V>
        {
            topic._onFailure = onFailure;
            return topic;
        }
    }*/
}
