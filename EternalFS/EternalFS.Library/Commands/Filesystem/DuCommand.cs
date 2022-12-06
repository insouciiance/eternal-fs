using EternalFS.Library.Filesystem;
using EternalFS.Library.Filesystem.Accessors;

namespace EternalFS.Library.Commands.Filesystem;

[Command("du", true)]
public partial class DuCommand
{
    public CommandExecutionResult Execute(ref CommandExecutionContext context)
    {
        DiskUsageInfo usageInfo = EternalFileSystemAccessorHelper.GetDiskUsageInfo(context.Accessor);
        context.Writer.AppendLine("Disk usage info:");
        context.Writer.AppendLine($"Bytes occupied (total): {usageInfo.BytesAllocatedTotal} / {context.FileSystem.Size}");
        context.Writer.AppendLine($"Bytes occupied (actual): {usageInfo.BytesAllocatedUsed} / {context.FileSystem.Size}");
        context.Writer.Append($"Entries allocated: {usageInfo.EntriesAllocated}");
        return CommandExecutionResult.Default;
    }
}
