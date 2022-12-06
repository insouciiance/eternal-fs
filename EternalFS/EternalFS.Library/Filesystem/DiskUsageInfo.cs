namespace EternalFS.Library.Filesystem;

public readonly record struct DiskUsageInfo(int BytesAllocatedUsed, int BytesAllocatedTotal, int EntriesAllocated);
