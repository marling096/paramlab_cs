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




    public class OpencvView : Control
    {

        public OpencvView()
        {
        }
        public Control CreateComponentView()
        {
            ComponentBody body = new ComponentBody();
            OpenCvModelView OpencvView = new OpenCvModelView();
            var grid = new Grid
            {
                Width = 237,
                Height = 539
            };

            grid.Children.Add(OpencvView);
            return grid;
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

    public class OpencvComponent : VisualComponentBase
    {

        public override List<string> Subs { set; get; } = new List<string>();

        public override List<string> Pubs { set; get; } = new List<string>();
        private Lazy<OpencvView> ComponentView = new Lazy<OpencvView>();

        public OpencvComponent(string id)
        {
            Id = id;
        }

        public static string Description { get; } = "Opencv";
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
                Subs.Add(_eventName);
                Subscribe<string>(_eventName, OnEvent);
            }

        }

        private void OnEvent(string data)
        {
            //show data
            Console.WriteLine($"Event received: {data}");
            foreach (var sub in Subs)
            {
                Console.WriteLine($"Subscribed to {sub}");
            }
        }

        public override void RegisterPublisher(string? _eventName)
        {
            //do nothing
            if (_eventName != null)
            {
                Pubs.Add(_eventName);
                Publish<string>(_eventName, "test success");
            }
        }

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



