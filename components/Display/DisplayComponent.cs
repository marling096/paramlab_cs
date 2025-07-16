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
            var grid = new Grid
            {
                Width = 200,
                Height = 100,
                Background = Brushes.Blue,
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

        public Params DisplayParams;

        private Lazy<DisplayView> ComponentView = new Lazy<DisplayView>();

        public DisplayComponent(string id)
        {
            Id = id;
            DisplayParams = new Params();
        }

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
                DisplayParams.Subs.Add(_eventName);
                Subscribe<string>(_eventName, OnEvent);
            }

        }

        private void OnEvent(string data)
        {
            //show data
            Console.WriteLine($"Event received: {data}");
            foreach (var sub in DisplayParams.Subs)
            {
                Console.WriteLine($"Subscribed to {sub}");
            }
        }

        public override void RegisterPublisher(string? _eventName)
        {
            //do nothing
            if (_eventName != null)
            {
                DisplayParams.Pubs.Add(_eventName);
                Publish<string>(_eventName, "test success");
            }
        }

        protected override Control CreateParamView()
        {
            return ComponentView.Value.CreateEditView(DisplayParams);

        }
        protected override Control CreateView()
        {
            return ComponentView.Value.CreateComponentView();
        }
    }

}



