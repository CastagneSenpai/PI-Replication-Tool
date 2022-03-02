using System;
using System.Collections.Generic;

namespace Core
{
    public sealed class Mediator
    {
        private static readonly Mediator instance = new Mediator();
        private readonly Dictionary<string, List<Action<object>>> pl_dict
          = new Dictionary<string, List<Action<object>>>();

        private Mediator() { }

        public static Mediator Instance
        {
            get
            {
                return instance;
            }
        }

        public void Subscribe(string token, Action<object> callback)
        {
            if (!pl_dict.ContainsKey(token))
            {
                pl_dict[token] = new List<Action<object>>();
            }

            pl_dict[token].Add(callback);
        }

        public void Unsubscribe(string token, Action<object> callback)
        {
            pl_dict[token].Remove(callback);

            if (pl_dict[token].Count == 0)
            {
                pl_dict.Remove(token);
            }
        }

        public void Notify(string token, object args = null)
        {
            pl_dict[token].ForEach(callback => callback(args));
        }
    }
}