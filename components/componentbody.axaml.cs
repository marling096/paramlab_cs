using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform.Storage;
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


namespace components
{
    public partial class ComponentBody : UserControl
    {

        public ComponentBody()
        {
            InitializeComponent();

        }

        public void SetTitle(string title)
        {

            this.FindControl<TextBlock>("Component_title").Text = title ??
            throw new ArgumentNullException(nameof(title), "Title cannot be null");
        }

        public void AddContnet(Control ctrl)
        {
            var border = this.FindControl<Border>("Context");
            if (border == null)
            {
                Console.WriteLine("⚠️ 未找到名为 'Context' 的 Border");
                return;
            }
            border.Child = ctrl ?? throw new ArgumentNullException(nameof(ctrl));

        }

    }


}