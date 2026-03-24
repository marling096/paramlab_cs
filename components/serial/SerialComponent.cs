using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.PropertyGrid.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using core;

namespace components
{


    public class SerialComponent : VisualComponentBase
    {


        public override List<string> Pubs { set; get; } = new List<string>();
        public override List<string> Subs { set; get; } = new List<string>();
        private Dictionary<string, List<Action<Object>>> Sub_handler = new();

        public ComponentBody Comp_Body = new ComponentBody();

        public ComponentEdit Edit = new ComponentEdit();
        public SerialComponent(string id)
        {
            Id = id;
        }

        public static string Description { get; } = "串口组件";
        public override string Id { get; set; }

        public override void Mount()
        {
            // AddSubscribe("paramTest", ReceiveDataEvent);
        }

        public override void Unmount()
        {
            foreach (var sub in Sub_handler)
            {
                UnSubscribe<Object>(sub.Key, ReceiveDataEvent);
            }

        }


        public override void AddSubscribe(string? _eventName, Action<Object>? handler)
        {
            if (_eventName != null && _eventName != "" && !Subs.Contains(_eventName))
            {
                Console.WriteLine($"添加话题 {_eventName}");
                if (!Subs.Contains(_eventName))
                {
                    Subs.Add(_eventName);
                    Sub_handler.Add(_eventName, new List<Action<Object>>());
                    if (handler != null)

                        Sub_handler[_eventName].Add(handler);
                    else
                    {
                        Sub_handler[_eventName].Add(ReceiveDataEvent);
                    }
                    Subscribe<Object>(_eventName, ReceiveDataEvent);
                }
                else
                {
                    if (handler != null)

                        Sub_handler[_eventName].Add(handler);
                    else
                    {
                        Sub_handler[_eventName].Add(ReceiveDataEvent);
                    }
                }


            }

        }
        public override void DeleteSubscribe(string? _eventName, Action<Object>? handler)
        {
            if (_eventName != null && _eventName != "")
            {
                if (Subs.Contains(_eventName))
                {
                    Subs.Remove(_eventName);
                    if (handler != null)
                        Sub_handler[_eventName].Remove(handler);
                    else
                    {
                        Sub_handler[_eventName].Remove(ReceiveDataEvent);
                    }
                    UnSubscribe<Object>(_eventName, ReceiveDataEvent);

                }
            }

        }


        private void ReceiveDataEvent(Object data)
        {
            Dispatcher.UIThread.Post(() =>
            {
            });
            foreach (var sub in Subs)
            {
                Console.WriteLine($"Subscribed to {sub}");
            }
        }

        public override void AddPublisher(string? _eventName, object? message = null)
        {

            if (_eventName != null)
            {
                Pubs.Add(_eventName);
                Publish<Object>(_eventName, message);
            }
        }
        public override void DeletePublisher(string? _eventName) { }

        private bool _isBound = false;
        protected override Control CreateParamView()
        {
            if (!_isBound)
            {
                Edit.Bind_SubAction((topic) =>
                {
                    AddSubscribe(topic, null);
                }, (topic) =>
                {
                    DeleteSubscribe(topic, null);
                });
                Edit.Bind_PubAction((topic) =>
                {
                    AddPublisher(topic, null);
                }, (topic) =>
                {
                    DeletePublisher(topic);
                });
                _isBound = true;
            }

            return Edit;

        }

        protected override Control CreateView()
        {
            SerialPortSettingsView SerialView = new SerialPortSettingsView();

            return SerialView;

        }
    }

}



