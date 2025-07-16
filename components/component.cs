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
        public List<string> Subs { set; get; } = new List<string>();
        public List<string> Pubs { set; get; } = new List<string>();
    }

}