namespace EternalFS.Library.Utils;

public interface IPipelineElement<T>
    where T : IPipelineElement<T>
{
    T? Next { get; }

    void SetNext(T? next);
}
