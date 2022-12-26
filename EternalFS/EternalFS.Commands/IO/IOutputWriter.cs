namespace EternalFS.Commands.IO;

public interface IOutputWriter
{
    const string ERROR_STRING = "ERROR";

    const string WARNING_STRING = "WARNING";

    void Write(string text, OutputLevel level);

    void Flush();
}
