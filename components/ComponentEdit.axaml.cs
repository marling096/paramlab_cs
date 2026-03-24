using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.PropertyGrid.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using core;
using Editor;
namespace components
{

    public partial class Param : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<string> param_subs = new();

        [ObservableProperty]
        public ObservableCollection<string> param_pubs = new();

        public Action<string> param_subs_add;

        public Action<string> param_subs_del;

        public Action<string> param_pubs_add;

        public Action<string> param_pubs_del;
        public Action<string> ChangeAction;
        public Param()
        {
            param_subs.CollectionChanged += param_subs_Changed;
            param_pubs.CollectionChanged += param_pubs_Changed;
        }

        private void param_subs_Changed(object? sender, NotifyCollectionChangedEventArgs e)
        {

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Console.WriteLine("添加了元素");
                    foreach (var newItem in e.NewItems!)
                    {
                        if (newItem is string topic)
                        {
                            Console.WriteLine($"add:{topic}");
                            param_subs_add?.Invoke(topic);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    Console.WriteLine("移除了元素");
                    foreach (var oldItem in e.OldItems!)
                    {
                        if (oldItem is string topic)
                        {
                            Console.WriteLine($"del:{topic}");
                            param_subs_del?.Invoke(topic);
                        }

                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    Console.WriteLine("替换了元素");
                    var olditem = e.OldItems![0];
                    var newitem = e.NewItems![0];

                    if (olditem is string oldtopic && newitem is string newtopic)
                    {
                        Console.WriteLine($"add:{newtopic} , del:{oldtopic}");
                        param_subs_del?.Invoke(oldtopic);
                        param_subs_add?.Invoke(newtopic);
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                    Console.WriteLine("移动了元素");
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Console.WriteLine("集合已重置（清空或重新分配）");
                    break;
            }
        }

        private void param_pubs_Changed(object? sender, NotifyCollectionChangedEventArgs e)
        {

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Console.WriteLine("添加了元素");
                    foreach (var newItem in e.NewItems!)
                    {
                        if (newItem is string topic)
                            param_pubs_add?.Invoke(topic);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    Console.WriteLine("移除了元素");
                    foreach (var oldItem in e.OldItems!)
                    {
                        if (oldItem is string topic)
                            param_pubs_del?.Invoke(topic);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    Console.WriteLine("替换了元素");
                    var olditem = e.OldItems![0];
                    var newitem = e.NewItems![0];

                    if (olditem is string oltopic && newitem is string newtopic)
                    {
                        param_pubs_del?.Invoke(oltopic);
                        param_pubs_add?.Invoke(newtopic);
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                    Console.WriteLine("移动了元素");
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Console.WriteLine("集合已重置（清空或重新分配）");
                    break;
            }
        }


    }
    public partial class ComponentEdit : Window
    {
        public Param component_param { get; set; } = new Param();
        public ComponentEdit()
        {
            InitializeComponent();

            var propertyGrid = new PropertyGrid
            {
                BorderThickness = new Thickness(1),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(0.5),
                Margin = new Thickness(1),
                DataContext = component_param,
            };
            this.Content = propertyGrid;

        }

        public void Bind_list(List<string> subs, List<string> pubs)
        {
            if (subs != null)
            {
                foreach (var sub in subs)
                {
                    component_param.param_subs.Add(sub);
                }
            }
            if (pubs != null)
            {
                foreach (var pub in pubs)
                {
                    component_param.param_pubs.Add(pub);
                }
            }
        }

        public void Bind_SubAction(Action<string> add, Action<string> delete)
        {
            component_param.param_subs_add += add;
            component_param.param_subs_del += delete;

        }

        public void Bind_PubAction(Action<string> add, Action<string> delete)
        {
            component_param.param_pubs_add += add;
            component_param.param_pubs_del += delete;

        }

        public void ComponentEdit_AddSub(string topic)
        {
            component_param.param_subs.Add(topic);
        }

        public void ComponentEdit_AddPub(string topic)
        {
            component_param.param_pubs.Add(topic);
        }



    }
}



