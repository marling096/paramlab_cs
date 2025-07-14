using System;
using System.Collections.Generic;

namespace core
{
    public class EventHub
    {
        private static readonly Lazy<EventHub> _instance = new(() => new EventHub());
        public static EventHub Instance => _instance.Value;

        // 使用泛型事件存储，每个事件名对应多个委托
        private readonly Dictionary<string, List<Delegate>> _typedEvents = new();

        private EventHub() { }

        /// <summary>
        /// 订阅某个事件（带泛型参数）
        /// </summary>
        public void Subscribe<T>(string eventName, Action<T> handler)
        {
            if (!_typedEvents.ContainsKey(eventName))
                _typedEvents[eventName] = new();

            _typedEvents[eventName].Add(handler);
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        public void Unsubscribe<T>(string eventName, Action<T> handler)
        {
            if (_typedEvents.TryGetValue(eventName, out var list))
            {
                list.Remove(handler);
                if (list.Count == 0)
                    _typedEvents.Remove(eventName);
            }
        }

        /// <summary>
        /// 发布事件（传递参数）
        /// </summary>
        public void Publish<T>(string eventName, T arg)
        {
            if (_typedEvents.TryGetValue(eventName, out var list))
            {
                foreach (var del in list.ToArray()) // 拷贝防止迭代中修改
                {
                    if (del is Action<T> handler)
                    {
                        try
                        {
                            handler.Invoke(arg);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[EventHub] Handler error in '{eventName}': {ex}");
                        }
                    }
                }
            }
        }
        public void DelEvent(string eventName)
        {
            if (_typedEvents.TryGetValue(eventName, out var list))
            {
                list.Clear();
                _typedEvents.Remove(eventName);
            }

        }

    }


}

