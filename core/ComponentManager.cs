using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;

namespace core
{

    public interface IVisualComponent : IDisposable
    {
        string Id { get; set; }
        void Initialize();
        void Activate();
        void Deactivate();
        void RegisterSubscriptions(string? eventName);
        void RegisterPublisher(string? eventName);

        Control GetView(); // Avalonia 控件
    }


    public abstract class VisualComponentBase : IVisualComponent
    {
        protected Control? view;
        protected Dictionary<string, Delegate> subscriptions = new();
        protected Dictionary<string, string> Events = new();
        public abstract string Id { get; set; }
        public abstract void Initialize();
        public abstract void Activate();
        public abstract void Deactivate();
        public abstract void RegisterSubscriptions(string? eventName);
        public abstract void RegisterPublisher(string? eventName);

        public virtual Control GetView()
        {
            view ??= CreateView();
            return view!;
        }

        public virtual Control GetParamPanel()
        {
            view ??= CreateParamPanel();
            return view!;
        }

        protected abstract Control CreateView();

        protected abstract Control CreateParamPanel();

        protected void Subscribe<T>(string eventName, Action<T> handler)
        {
            EventHub.Instance.Subscribe(eventName, handler);
            subscriptions[eventName] = handler;
        }

        protected void UnSubscribe<T>(string eventName, Action<T> handler)
        {
            EventHub.Instance.Unsubscribe(eventName, handler);
            subscriptions.Remove(eventName);
        }

        protected void Publish<T>(string eventName, string data)
        {
            EventHub.Instance.Publish(eventName, data);

        }

        public void Dispose()
        {
        }
    }

    public class ComponentManager
    {

        private Dictionary<string, VisualComponentBase> Components = new();
        private Dictionary<string, Type> ComponentTypes = new();

        public void RegisterBase(string @namespace)
        {
            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass &&
                            !t.IsAbstract &&
                            t.Namespace == @namespace &&
                            typeof(VisualComponentBase).IsAssignableFrom(t));
            ComponentTypes = types.ToDictionary(t => t.GetProperty("Description")?.GetValue(null) as string ??
                throw new ArgumentException($"Component {t.Name} does not have a static Description property."),
                                                  t => t);
            Console.WriteLine("Registered base components:");
            foreach (var component in ComponentTypes)
            {
                Console.WriteLine($" - {component.Key}");
            }
        }
        //
        public Control CreateFromBase(string Description, string Id)
        {

            if (ComponentTypes.TryGetValue(Description, out var type))
            {
                var component = (VisualComponentBase)Activator.CreateInstance(type, Id)!;
                component.Initialize();
                component.Activate();
                Components[Id] = component;
                return component.GetView();
            }
            throw new ArgumentException($"Component with description '{Description}' not found.");
        }

        public Control CreateParamPanel(string Description, string Id)
        {

            if (ComponentTypes.TryGetValue(Description, out var type))
            {
                var component = (VisualComponentBase)Activator.CreateInstance(type, Id)!;
                return component.GetParamPanel();
            }
            throw new ArgumentException($"Component with description '{Description}' not found.");
        }

        public void Activate(string Description)
        {
            if (Components.TryGetValue(Description, out var comp))
                comp.Activate();
        }

        public void Deactivate(string Description)
        {
            if (Components.TryGetValue(Description, out var comp))
                comp.Deactivate();
        }

        public Dictionary<string, Type> GetAll() => ComponentTypes;

        public void AddComponentIn(string id, string subscription)
        {
            if (Components.TryGetValue(id, out var comp))
            {
                comp.RegisterSubscriptions(subscription);
            }
            else
            {
                throw new ArgumentException($"Component with id '{id}' not found.");
            }


        }

        public void AddComponentOut(string id, string publisher)
        {
            if (Components.TryGetValue(id, out var comp))
            {
                comp.RegisterPublisher(publisher);
            }
            else
            {
                throw new ArgumentException($"Component with id '{id}' not found.");
            }


        }
    }
}



