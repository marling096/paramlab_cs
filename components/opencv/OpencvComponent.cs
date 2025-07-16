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
using Avalonia.Threading;
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

            return OpenCvModelView.Instance.GetView();
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

    public class OpencvComponent : VisualComponentBase
    {

        public Params OpencvParams;

        private Lazy<OpencvView> ComponentView = new Lazy<OpencvView>();

        public OpencvComponent(string id)
        {
            Id = id;
            OpencvParams = new Params();
        }

        public static string Description { get; } = "Opencv组件";
        public override string Id { get; set; }

        public override void Initialize()
        {
            //初始化ui
            RegisterSubscriptions("paramTest");
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
                OpencvParams.Subs.Add(_eventName);
                Subscribe<string>(_eventName, OnEvent);
            }

        }

        private void OnEvent(string data)
        {
            // try
            // {
            //     Dispatcher.UIThread.InvokeAsync(() =>
            //     {
            //         var button = OpenCvModelView.Instance.FindControl<Button>("sendbutton");
            //         // 可选：执行绑定命令
            //         button.Command?.Execute(button.CommandParameter);
            //     });
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine($"Error handling event: {ex.Message}");
            // }


        }

        public override void RegisterPublisher(string? _eventName)
        {
            //do nothing
            if (_eventName != null)
            {
                OpencvParams.Pubs.Add(_eventName);
                Publish<string>(_eventName, "test success");
            }
        }

        protected override Control CreateParamView()
        {
            return ComponentView.Value.CreateEditView(OpencvParams);

        }
        protected override Control CreateView()
        {
            return ComponentView.Value.CreateComponentView();
        }
    }

}



