using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClipboardDecorator;

/// <summary>
/// A clipboard implementation for Linux w/Gnome that supports text and files, since the
/// Avalonia implementation only supports text.
/// </summary>
/// <param name="clipboard">An <see cref="IClipboard"/> implementation to decorate.</param>
/// <param name="storageProvider">A <see cref="IStorageProvider"/> instance to get files with.</param>
internal partial class MyLinuxClipboard(IClipboard clipboard, IStorageProvider storageProvider) : IClipboard
{
    public Task ClearAsync() => clipboard.ClearAsync();

    public async Task<object?> GetDataAsync(string format)
    {
        string[] formats = await GetFormatsAsync();

        async Task<IStorageItem[]?> GetStorageItemsAsync()
        {
            object? data = await clipboard.GetDataAsync("x-special/gnome-copied-files");
            if (data is not byte[] bytes)
            {
                return null;
            }

            string files = Encoding.UTF8.GetString(bytes);
            MatchCollection matches = ClipboardFilesRegex().Matches(files);
            IEnumerable<Uri> uris = matches.Select(m => new Uri(m.Groups[1].Value));
            List<IStorageItem> items = [];
            foreach (Uri uri in uris)
            {
                IStorageItem? item = await storageProvider.TryGetFileFromPathAsync(uri);
                if (item != null)
                {
                    items.Add(item);
                    continue;
                }
                item = await storageProvider.TryGetFolderFromPathAsync(uri);
                if (item != null)
                {
                    items.Add(item);
                    continue;
                }
            }
            return items.Count == 0 ? null : ([.. items]);
        }

        if (format == DataFormats.Text && formats.Contains(DataFormats.Text))
        {
            return await GetTextAsync();
        }

        if (format == DataFormats.Files && formats.Contains(DataFormats.Files))
        {
            return await GetStorageItemsAsync();
        }

        return null;
    }

    public async Task<string[]> GetFormatsAsync()
    {
        string[] clipboardFormats = await clipboard.GetFormatsAsync() ?? [];

        if (clipboardFormats.Contains("x-special/gnome-copied-files"))
        {
            return [DataFormats.Files,];
        }

        if (clipboardFormats.Contains("Text"))
        {
            return [DataFormats.Text,];
        }

        return [];
    }

    public Task<string?> GetTextAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task SetDataObjectAsync(IDataObject data) => throw new NotImplementedException();

    public Task SetTextAsync(string? text) => clipboard.SetTextAsync(text);

    [GeneratedRegex("\n(file://.*)")]
    private static partial Regex ClipboardFilesRegex();
}
