using System;
using System.IO;
using System.Text;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Filesystem.Accessors;

namespace EternalFS.Commands.IO;

public class FileEntryOutputWriter : IOutputWriter
{
    private readonly IEternalFileSystemAccessor _accessor;

    private readonly EternalFileSystemFatEntry _fatEntry;

    private readonly byte[] _subEntryName;

    private readonly bool _append;

    private readonly StringBuilder _builder = new();

    public FileEntryOutputWriter(in SubEntryInfo info, IEternalFileSystemAccessor accessor, bool append = false)
    {
        _accessor = accessor;
        _fatEntry = info.FatEntry;
        _append = append;

        _subEntryName = new byte[info.Name.Length];
        info.Name.CopyTo(_subEntryName);
    }

    public void Write(string text, OutputLevel level)
    {
        text = level switch
        {
            OutputLevel.Error => $"{IOutputWriter.ERROR_STRING}: {text}",
            OutputLevel.Warning => $"{IOutputWriter.WARNING_STRING}: {text}",
            _ => text
        };

        _builder.Append(text);
    }

    public void Flush()
    {
        string output = _builder.ToString();

        if (output.EndsWith(Environment.NewLine))
            output = output[..^Environment.NewLine.Length];

        MemoryStream source = new(Encoding.UTF8.GetBytes(output));
        _accessor.WriteFile(new(_fatEntry, _subEntryName), source, _append);
    }
}
