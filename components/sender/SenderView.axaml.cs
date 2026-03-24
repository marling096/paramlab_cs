using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;

namespace components
{
    public class InputSubmitHandlerWrapper : IInputSubmitHandler
    {
        private readonly Action<string> _action;

        public InputSubmitHandlerWrapper(Action<string> action)
        {
            _action = action;
        }

        public void OnSubmit(string input) => _action(input);
    }

    public partial class SenderView : UserControl
    {
        public SenderView(IInputSubmitHandler? handler = null)
        {
            InitializeComponent();
            this.DataContext = new InputSubmitPanelViewModel(handler);
        }
        public SenderView(Action<string> onSubmit)
    : this(new InputSubmitHandlerWrapper(onSubmit))
        {
        }

        public InputSubmitPanelViewModel ViewModel => (InputSubmitPanelViewModel)this.DataContext!;
    }


}
