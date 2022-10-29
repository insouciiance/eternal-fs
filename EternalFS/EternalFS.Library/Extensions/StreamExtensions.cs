using System.IO;
using System.Runtime.InteropServices;

namespace EternalFS.Library.Extensions;

public static class StreamExtensions
{
    public static T MarshalReadStructure<T>(this Stream stream)
        where T : struct
    {
        int structSize = Marshal.SizeOf<T>();
        byte[] bytes = new byte[structSize];

        int bytesRead = stream.Read(bytes, 0, bytes.Length);

        if (bytesRead < structSize)
            throw new IOException($"Unable to read {typeof(T)}: fewer bytes read than requested ({bytesRead}/{structSize}).");
        
        GCHandle pBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        T result = Marshal.PtrToStructure<T>(pBytes.AddrOfPinnedObject());

        return result;
    }

    public static void MarshalWriteStructure<T>(this Stream stream, T value)
        where T : struct
    {
        int structSize = Marshal.SizeOf<T>();
        
        byte[] bytes = new byte[structSize];
        GCHandle pBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        Marshal.StructureToPtr(value, pBytes.AddrOfPinnedObject(), false);
        
        stream.Write(bytes);
    }
}
