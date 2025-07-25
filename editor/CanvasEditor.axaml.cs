using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using core;
using components;
using Avalonia.Controls.Shapes;
using System.Security.Cryptography;
using paramlab_cs;
using Avalonia.VisualTree;
using System.Text.Json.Serialization;


namespace Editor
{

    public interface EditorPlugin
    {
        string Name { get; }
        void Initialize(CanvasEditor editor);
    }


    public partial class CanvasEditor : UserControl
    {

        public MainWindow owner { set; get; }
        public List<ComponentModel> componentsData = new List<ComponentModel>();
        public List<EditorPlugin> plugins = new List<EditorPlugin>();

        public Dictionary<string, DragResizeAdorner> Adorners = new();
        public Dictionary<Control, string> components = new();//ctrl id
        public Dictionary<string, Control> _components = new();//id ctrl 
        public ContextMenu contextMenu = new ContextMenu();

        public Control Current_ctrl = new Control();
        public double currentX = 0;
        public double currentY = 0;

        public Action SaveToFile;

        public Action LoadFromFile;

        public Action<KeyEventArgs> Hotkey;

        public Action<string> Delete;
        public Action UnDo;



        public CanvasEditor(MainWindow win)
        {

            var t = ComponentManager.Instance.GetAllTypes();
            foreach (var c in t)
            {
                contextMenu.Items.Add(new MenuItem
                {
                    Header = c.Key,
                    Command = ContextMenuHandlerCommand,
                    CommandParameter = c.Key,
                });
            }
            InitializeComponent();
            EditorCanvas.PointerPressed += PointerPressedHandler;
            Delete += RemoveComponent;
            owner = win;
            owner.KeyDown += OnKeyDownHandler;


        }

        public void AddPlugin(EditorPlugin plugin)
        {
            plugin.Initialize(this);
            plugins.Add(plugin);
        }

        [RelayCommand]
        private void ContextMenuHandler(string description)
        {
            AddComponent(description, currentX, currentY);
        }

        private void OnKeyDownHandler(object? sender, KeyEventArgs e)
        {
            Hotkey?.Invoke(e);
            if (e.Key == Key.Delete)
            {
                Delete?.Invoke(components[Current_ctrl]);
            }
        }
        private void PointerPressedHandler(object? sender, PointerPressedEventArgs args)
        {
            if (args.Source is Control clickedControl)
            {
                // Console.WriteLine($"找到 sender控件: {sender.GetType().Name}");
                var canvas = this.FindControl<Canvas>("EditorCanvas"); // 替换为你的 Canvas 名称
                if (canvas == null) return;

                Control? current = clickedControl;

                // 一直向上查找，直到其 Parent 是 canvas 为止
                while (current != null && current.GetVisualParent() is Control parent)
                {
                    if (parent == canvas)
                    {
                        // 找到 Canvas 的直接子控件
                        // Console.WriteLine($"找到 Canvas 子控件: {current.GetType().Name}, Name: {current.Name}");
                        // Console.WriteLine($"hash{current.GetHashCode()}");
                        Current_ctrl = current;
                        break;
                    }

                    current = parent;
                }

            }

            var point = args.GetCurrentPoint(sender as Control);
            this.currentX = point.Position.X;
            this.currentY = point.Position.Y;
            if (point.Properties.IsLeftButtonPressed)
            {
            }
            if (point.Properties.IsRightButtonPressed)
            {
                contextMenu.Open(EditorCanvas);
            }

        }

        public void AddComponent(string description, double x, double y, string? id = null)
        {
            if (id == null) id = Guid.NewGuid().ToString();
            Control ctrl = ComponentManager.Instance.CreateFromBase(description, id);
            components[ctrl] = id;
            _components[id] = ctrl;
            var comp = ComponentManager.Instance.GetComponent(id);
            componentsData.Add(new ComponentModel
            {
                id = id,
                description = description,
                Subs = comp.Subs,
                Pubs = comp.Pubs,
                X = x,
                Y = y
            });
            ctrl.PointerPressed += RightPressed;
            DragResizeAdorner addone = new DragResizeAdorner();
            addone.AttachThumbs(EditorCanvas, ctrl, x, y);
            Adorners[id] = addone;

        }

        public void RemoveComponent(string id)
        {
            EditorCanvas.Children.Remove(_components[id]);
            Adorners[id].Dispose();
            ComponentManager.Instance.DisposComponent(id);
            Adorners.Remove(id);
        }

        private void RightPressed(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);
            if (point.Properties.IsRightButtonPressed)
            {
                e.Handled = true; // 阻止传给 Canvas
                if (sender is Control ctrl)
                {
                    var id = components.TryGetValue(ctrl, out var componentId) ? componentId : throw new ArgumentException("Component not found.");
                    var pal = ComponentManager.Instance.CreateParamPanel(id);
                    if (pal is Window win)
                    {
                        win.Closing += (sender, e) =>
                        {
                            if (!owner._forceClose)
                            {
                                e.Cancel = true;
                                win.Hide();
                            }

                        };
                        win.Show(owner);
                    }
                }

            }

        }


        public void LoadComponents(List<ComponentModel> data)
        {
            EditorCanvas.Children.Clear();
            componentsData.Clear();
            foreach (var model in data)
            {
                AddComponent(model.description, model.X, model.Y, model.id);
                LoadComponentparam(model.id, model.Subs, model.Pubs);
            }

        }

        public void LoadComponentparam(string id, List<string> Subs, List<string> Pubs)
        {
            var comp = ComponentManager.Instance.GetComponent(id);
            if (comp != null)
            {
                foreach (var sub in Subs)
                {
                    comp.AddSubscribe(sub, null);
                }
                foreach (var pub in Pubs)
                {
                    comp.AddPublisher(pub, null);
                }
            }

        }

    }

    public class ComponentModel
    {
        public string id { get; set; } = "";
        public string description { get; set; } = "";

        public List<string> Subs { get; set; } = new();

        public List<string> Pubs { get; set; } = new List<string>();


        public double X { get; set; }
        public double Y { get; set; }
    }


}