using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.PropertyGrid.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using core;

namespace components
{




    public class DisplayView : Control
    {

        public DisplayView()
        {
        }
        public Control CreateComponentView()
        {
            // var grid = new Grid
            // {
            //     Width = 200,
            //     Height = 100,
            //     Background = Brushes.Blue,
            // };
            // var canvas = new Canvas
            // {
            //     Background = new SolidColorBrush(Color.FromArgb(64, 0, 0, 255))
            // };
            // grid.Children.Add(canvas);
            // return grid;
            ComponentBody body = new ComponentBody();
            var text = new TextBlock { Text = "动态添加的文本", Margin = new Thickness(0, 0, 0, 5) };
            body.Context.Child = text;
            return body.Body;
        }

        public Control CreateEditView(object param)
        {
            var win = new Window
            {
                Width = 500,
                Height = 300,
                Title = "组件编辑",
                CornerRadius = new CornerRadius(1),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                SystemDecorations = SystemDecorations.Full, // 有边框与标题栏
                CanResize = true
            };
            var propertyGrid = new PropertyGrid
            {
                BorderThickness = new Thickness(1),
                Background = Brushes.Black,
                CornerRadius = new CornerRadius(0.5),
                Margin = new Thickness(1),
                DataContext = param,
            };
            win.Content = propertyGrid;
            return win;
        }

    }

    public class DisplayComponent : VisualComponentBase
    {


        public override List<string> Pubs { set; get; } = new List<string>();
        public override List<string> Subs { set; get; } = new List<string>();

        private Dictionary<string, List<Action<Object>>> Sub_handler = new();
        private Lazy<DisplayView> ComponentView = new Lazy<DisplayView>();

        public DisplayComponent(string id)
        {
            Id = id;
        }

        public static string Description { get; } = "显示器";
        public override string Id { get; set; }

        public override void Mount()
        {

        }

        public override void Unmount()
        {
            foreach (var sub in Sub_handler)
            {
                foreach (var handler in sub.Value)
                {
                    UnRegisterSubscriptions(sub.Key, handler);
                }

            }

        }


        public override void RegisterSubscriptions(string? _eventName, Action<Object>? handler)
        {
            if (_eventName != null)
            {
                Subs.Add(_eventName);
                if (handler != null)
                    Sub_handler[_eventName].Add(handler);

                Subscribe<Object>(_eventName, OnEvent);
            }

        }
        public override void UnRegisterSubscriptions(string? _eventName, Action<Object>? handler)
        {
            Subs.Remove(_eventName);
            if (handler != null)
                Sub_handler[_eventName].Remove(handler);
            UnSubscribe<Object>(_eventName, OnEvent);
        }


        private void OnEvent(Object data)
        {
            //show data
            Console.WriteLine($"Event received: {data}");
            foreach (var sub in Subs)
            {
                Console.WriteLine($"Subscribed to {sub}");
            }
        }

        public override void RegisterPublisher(string? _eventName, object? message = null)
        {

            if (_eventName != null)
            {
                Pubs.Add(_eventName);
                Publish<Object>(_eventName, message);
            }
        }
        public override void UnRegisterPublisher(string? _eventName) { }
        protected override Control CreateParamView()
        {
            var dataContext = new
            {
                subs = Subs,
                pubs = Pubs
            };

            return ComponentView.Value.CreateEditView(dataContext);

        }
        protected override Control CreateView()
        {
            return ComponentView.Value.CreateComponentView();
        }
    }

}



