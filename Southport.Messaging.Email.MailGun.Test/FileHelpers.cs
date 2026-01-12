using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

public static class FileHelpers
{
    // Returns a FileStream (file remains open until disposed)
    public static Stream OpenFileStream(string relativePath = @"Files\dummy.pdf")
    {
        var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
        var path = Path.Combine(baseDir, relativePath);
        return File.OpenRead(path);
    }

    // Returns a MemoryStream with the file contents (safe to use after original file is closed)
    public static async Task<Stream> OpenFileStreamAsync(string relativePath = @"Files\dummy.pdf")
    {
        var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
        var path = Path.Combine(baseDir, relativePath);
        var ms = new MemoryStream();
        await using var fs = File.OpenRead(path);
        await fs.CopyToAsync(ms);
        ms.Position = 0;
        return ms;
    }
    
    public static async Task<byte[]> StreamToBytesAsync(Stream stream)
    {
        if (stream is MemoryStream ms)
            return ms.ToArray();

        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        return memory.ToArray();
    }
}