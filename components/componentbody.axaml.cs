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

        public Control GetComponentBody()
        {

            return this;
        }
    }


}