using EternalFS.Commands.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Filesystem.Accessors;

namespace EternalFS.Commands.Filesystem;

[Command("du", true)]
public partial class DuCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        DiskUsageInfo usageInfo = EternalFileSystemAccessorHelper.GetDiskUsageInfo(context.Accessor);
        context.Writer.Info("Disk usage info:");
        context.Writer.Info($"Bytes occupied (total): {usageInfo.BytesAllocatedTotal} / {context.FileSystem.Size}");
        context.Writer.Info($"Bytes occupied (actual): {usageInfo.BytesAllocatedActual} / {context.FileSystem.Size}");
        context.Writer.Info($"Entries allocated: {usageInfo.EntriesAllocated}");
        return CommandExecutionResult.Default;
    }
}
