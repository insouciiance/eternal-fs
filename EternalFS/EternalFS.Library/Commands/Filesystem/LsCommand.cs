using System.Collections.Generic;
using System.Text;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;

namespace EternalFS.Library.Commands.Filesystem;

[Command("ls")]
[CommandDoc("Lists all entries in current working directory.")]
public partial class LsCommand
{
    public static CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        EternalFileSystemFatEntry currentDirectory = new EternalFileSystemManager(context.FileSystem).OpenDirectory(context.CurrentDirectory);
        using EternalFileSystemFileStream stream = new(context.FileSystem, currentDirectory);

        byte entriesCount = (byte)stream.ReadByte();

        HashSet<string> subDirectories = new();
        HashSet<string> subFiles = new();

        for (int i = 0; i < entriesCount; i++)
        {
            EternalFileSystemEntry subEntry = stream.MarshalReadStructure<EternalFileSystemEntry>();
            string subEntryName = Encoding.UTF8.GetString(subEntry.SubEntryName).TrimEnd('\0');
            (subEntry.IsDirectory ? subDirectories : subFiles).Add(subEntryName);
        }

        foreach (string subDir in subDirectories)
            context.Writer.WriteLine($"{subDir}/");

        foreach (string subFile in subFiles)
            context.Writer.WriteLine(subFile);

        return new();
    }
}
