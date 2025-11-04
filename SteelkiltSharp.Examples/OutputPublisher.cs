using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelkiltSharp.Examples
{
    public class OutputPublisher
    {
        private readonly List<Action<string>> _subscribers = new List<Action<string>>();

        // Subscribe to output events
        public void Subscribe(Action<string> handler)
        {
            if (handler != null && !_subscribers.Contains(handler))
            {
                _subscribers.Add(handler);
            }
        }

        // Unsubscribe from output events
        public void Unsubscribe(Action<string> handler)
        {
            _subscribers.Remove(handler);
        }

        // Publish output to all subscribers
        public void Publish(string message = "")
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber(message);
            }
        }
    }
}
