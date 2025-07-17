using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Tmds.DBus.Protocol;

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

        public abstract string Id { get; set; }

        public abstract List<string> Subs { set; get; }

        public abstract List<string> Pubs { set; get; }

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

        public virtual Control GetParamView()
        {
            return CreateParamView()!;
        }

        protected abstract Control CreateView();

        protected abstract Control CreateParamView();


        protected void Subscribe<T>(string eventName, Action<T> handler)
        {
            EventHub.Instance.Subscribe(eventName, handler);

        }

        protected void UnSubscribe<T>(string eventName, Action<T> handler)
        {
            EventHub.Instance.Unsubscribe(eventName, handler);

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
        private static readonly Lazy<ComponentManager> _instance = new(() => new ComponentManager());
        public static ComponentManager Instance => _instance.Value;
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

        public Control CreateParamPanel(string Id)
        {
            var comp = Components[Id];
            if (comp != null)
            {
                return comp.GetParamView();
            }
            throw new ArgumentException($"Component with description '' not found.");
        }


        public Dictionary<string, Type> GetAllTypes() => ComponentTypes;

        public VisualComponentBase? GetComponent(string id)
        {
            if (Components.TryGetValue(id, out var component))
            {
                return component;
            }
            return null;
        }


    }
}



