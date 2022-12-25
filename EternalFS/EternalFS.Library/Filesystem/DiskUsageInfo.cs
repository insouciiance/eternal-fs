namespace EternalFS.Library.Filesystem;

public readonly struct DiskUsageInfo
{
    public readonly long BytesAllocatedTotal;

    public readonly long BytesAllocatedActual;

    public readonly long EntriesAllocated;

    public DiskUsageInfo(long bytesAllocatedTotal, long bytesAllocatedActual, long entriesAllocated)
    {
        BytesAllocatedTotal = bytesAllocatedTotal;
        BytesAllocatedActual = bytesAllocatedActual;
        EntriesAllocated = entriesAllocated;
    }
}
