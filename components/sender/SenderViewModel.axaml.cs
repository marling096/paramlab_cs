using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace components
{

    public interface IInputSubmitHandler
    {
        void OnSubmit(string input);
        
    }

    public partial class InputSubmitPanelViewModel : ObservableObject
    {
        private readonly IInputSubmitHandler? _handler;

        public InputSubmitPanelViewModel(IInputSubmitHandler? handler = null)
        {
            _handler = handler;
        }

        [ObservableProperty]
        private string inputText = string.Empty;

        [RelayCommand]
        private void Submit()
        {
            if (_handler != null)
            {
                _handler.OnSubmit(InputText);
            }
            else
            {
                Console.WriteLine($"[默认行为] 提交内容: {InputText}");
            }
        }
    }
}

