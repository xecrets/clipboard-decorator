using Avalonia.Input;
using Avalonia.Input.Platform;

using System.Threading.Tasks;

namespace ClipboardDecorator;

/// <summary>
/// A clipboard implementation decorator that does nothing, but it may still be useful as a debugging tool,
/// since it enables easy inspection of how the code interacts with the clipboard.
/// </summary>
/// <param name="clipboard">An <see cref="IClipboard"/> implementation to decorate.</param>
internal class MyDefaultClipboard(IClipboard clipboard) : IClipboard
{
    public Task ClearAsync() => clipboard.ClearAsync();

    public Task<object?> GetDataAsync(string format) => clipboard.GetDataAsync(format);

    public Task<string[]> GetFormatsAsync() => clipboard.GetFormatsAsync();

    public Task<string?> GetTextAsync() => clipboard.GetTextAsync();

    public Task SetDataObjectAsync(IDataObject data) => clipboard.SetDataObjectAsync(data);

    public Task SetTextAsync(string? text) => clipboard.SetTextAsync(text);
}
