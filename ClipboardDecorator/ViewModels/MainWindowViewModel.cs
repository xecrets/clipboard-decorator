using Avalonia.Input;
using Avalonia.Input.Platform;

using ReactiveUI;

using System.Reactive;

namespace ClipboardDecorator.ViewModels
{
    public class MainWindowViewModel(IClipboard clipboard) : ViewModelBase
    {
        public MainWindowViewModel()
            : this(null!)
        {
        }

        public string Greeting { get; set; } = "Hello, Avalonia!";

        public ReactiveCommand<Unit, Unit> CheckClipboardCommand =>
            ReactiveCommand.CreateFromTask(async () =>
            {
                Greeting = $"{(await clipboard.GetDataAsync(DataFormats.Files) != null ? "You have files!" : "No files in sight.")}";
                this.RaisePropertyChanged(nameof(Greeting));
            });
    }
}
