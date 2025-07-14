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

    public class Params : ObservableObject
    {
        private string _name = "Display";
        public List<string> subs { get; set; } = new();
        public List<string> pubs { get; set; } = new();
    }

    public class DisplayView : Control
    {

        public DisplayView()
        {
        }
        public Control CreateComponentView()
        {
            var grid = new Grid
            {
                Width = 200,
                Height = 100,
                Background = Brushes.LightGreen
            };
            var canvas = new Canvas
            {

                Background = new SolidColorBrush(Color.FromArgb(64, 0, 0, 255))
            };
            grid.Children.Add(canvas);
            return grid;
        }

        public Control CreateEditView(Params param)
        {
            var win = new Window
            {
                Width = 400,
                Height = 300,
                Title = "组件编辑",
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                SystemDecorations = SystemDecorations.Full, // 有边框与标题栏
                CanResize = true
            };
            var propertyGrid = new PropertyGrid
            {
                Margin = new Thickness(10),
                DataContext = param,
            };
            win.Content = propertyGrid;
            return win;
        }

    }

    public class DisplayComponent : VisualComponentBase
    {

        static Params DisplayParams = new Params();

        private Lazy<DisplayView> ComponentView = new Lazy<DisplayView>();
        public DisplayComponent(string id)
        {
            Id = id;
        }
        private TextBlock? _text;

        public static string Description { get; } = "显示器";
        public override string Id { get; set; }

        public override void Initialize()
        {
            //初始化ui
            RegisterSubscriptions("paramTest");
            RegisterSubscriptions("paramTest1");
            RegisterSubscriptions("paramTest2");
            // RegisterPublisher("Test");

        }

        public override void Activate()
        {
            //激活时可以做一些额外的事情
        }

        public override void Deactivate()
        {
            //停用时可以做一些清理工作

        }

        public override void RegisterSubscriptions(string? _eventName)
        {
            if (_eventName != null)
            {
                DisplayParams.subs.Add(_eventName);
                Subscribe<string>(_eventName, OnEvent);
            }

        }

        private void OnEvent(string data)
        {
            //show data
            Console.WriteLine($"Event received: {data}");
            foreach (var sub in DisplayParams.subs)
            {
                Console.WriteLine($"Subscribed to {sub}");
            }
        }

        public override void RegisterPublisher(string? _eventName)
        {
            //do nothing
            if (_eventName != null)
            {
                DisplayParams.pubs.Add(_eventName);
                Publish<string>(_eventName, "test success");
            }
        }

        protected override Control CreateParamPanel()
        {
            return ComponentView.Value.CreateEditView(DisplayParams);
        }
        protected override Control CreateView()
        {
            return ComponentView.Value.CreateComponentView();
        }
    }

}



