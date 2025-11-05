using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelkiltSharp.Examples
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    // Publisher class with queue-based processing
    public class OutputPublisher : IDisposable
    {
        private readonly List<Action<string>> _subscribers = new List<Action<string>>();
        private readonly BlockingCollection<string> _messageQueue = new BlockingCollection<string>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Task _processingTask;
        private readonly int _delayMilliseconds;

        public OutputPublisher(int delayMilliseconds = 250)
        {
            _delayMilliseconds = delayMilliseconds;
            _processingTask = Task.Run(ProcessMessages);
        }

        // Subscribe to output events
        public void Subscribe(Action<string> handler)
        {
            if (handler != null && !_subscribers.Contains(handler))
            {
                lock (_subscribers)
                {
                    _subscribers.Add(handler);
                }
            }
        }

        // Unsubscribe from output events
        public void Unsubscribe(Action<string> handler)
        {
            lock (_subscribers)
            {
                _subscribers.Remove(handler);
            }
        }

        // Publish output (non-blocking - just queues the message)
        public void Publish(string message = "")
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                _messageQueue.Add(message);
            }
        }

        // Background processing of queued messages
        private void ProcessMessages()
        {
            try
            {
                foreach (var message in _messageQueue.GetConsumingEnumerable(_cts.Token))
                {
                    try
                    {
                        // Get snapshot of subscribers to avoid lock during notification
                        Action<string>[] subscribersCopy;
                        lock (_subscribers)
                        {
                            subscribersCopy = _subscribers.ToArray();
                        }

                        // Notify all subscribers
                        foreach (var subscriber in subscribersCopy)
                        {
                            try
                            {
                                subscriber(message);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in subscriber: {ex.Message}");
                            }
                        }

                        // Delay between messages
                        if (_delayMilliseconds > 0)
                        {
                            Thread.Sleep(_delayMilliseconds);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
        }

        // Wait for all queued messages to be processed
        public void Flush(int timeoutMilliseconds = 30000)
        {
            // Mark that no more messages will be added
            _messageQueue.CompleteAdding();

            // Wait for processing to complete
            if (!_processingTask.Wait(timeoutMilliseconds))
            {
                Console.WriteLine("Warning: Flush timeout - some messages may not have been processed");
            }
        }

        public void Dispose()
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                _messageQueue.CompleteAdding();
            }

            _cts.Cancel();

            try
            {
                _processingTask.Wait(TimeSpan.FromSeconds(5));
            }
            catch (AggregateException)
            {
                // Ignore cancellation exceptions
            }

            _messageQueue.Dispose();
            _cts.Dispose();
        }
    }

    //public class OutputPublisher
    //{
    //    private readonly List<Action<string>> _subscribers = new List<Action<string>>();

    //    // Subscribe to output events
    //    public void Subscribe(Action<string> handler)
    //    {
    //        if (handler != null && !_subscribers.Contains(handler))
    //        {
    //            _subscribers.Add(handler);
    //        }
    //    }

    //    // Unsubscribe from output events
    //    public void Unsubscribe(Action<string> handler)
    //    {
    //        _subscribers.Remove(handler);
    //    }

    //    // Publish output to all subscribers
    //    public void Publish(string message = "")
    //    {
    //        foreach (var subscriber in _subscribers)
    //        {
    //            subscriber(message);
    //        }
    //    }
    //}
}
