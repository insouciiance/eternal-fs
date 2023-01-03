using System.IO;
using System.Linq;
using System.Text;
using EternalFS.Commands.Utils;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Filesystem.Accessors.Pipeline;

namespace EternalFS.Commands.Tests.Filesystem;

public abstract class FilesystemCommandTestBase : CommandTestBase
{
    protected FilesystemCommandTestBase(ITestOutputHelper @out) : base(@out) { }

    protected static CommandExecutionContext CreateFileSystemContext()
    {
        CommandExecutionContext context = CreateContext();
        CommandHelper.RunCommand("mkfs -n=TestFS -s=100000", ref context);
        return context;
    }

    protected static void AssertFile(string name, ref CommandExecutionContext context, string? content = null)
    {
        SubEntryInfo info = new(context.CurrentDirectory.FatEntryReference, Encoding.UTF8.GetBytes(name));
        EternalFileSystemEntry entry = context.Accessor.LocateSubEntry(info);

        if (content is null)
            return;
        
        using EternalFileSystemFileStream stream = new(context.FileSystem, entry.FatEntryReference);

        byte[] byteContent = new byte[entry.Size];
        stream.Read(byteContent, 0, content.Length);
        string contentString = Encoding.UTF8.GetString(byteContent);

        Assert.Equal(content, contentString);
    }

    protected static void AssertEntries(int count, ref CommandExecutionContext context)
    {
        Assert.Equal(count, context.Accessor.EnumerateEntries(EternalFileSystemMounter.RootDirectoryEntry, SearchOption.AllDirectories).Count());
    }

    protected static bool HasIndexer(AccessorPipelineElement accessor)
    {
        if (accessor is EternalFileSystemIndexerAccessor)
            return true;

        if (accessor.Next is not { } next)
            return false;

        return HasIndexer(next);
    }
}
