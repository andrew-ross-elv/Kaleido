namespace Kaleido.Abstractions.Attributes;

/// <summary>Declares paging support for a value set.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class PageableAttribute : Attribute
{
    /// <summary>Creates paging metadata.</summary>
    /// <param name="defaultSize">Default page size when the request does not specify one.</param>
    /// <param name="maxSize">Maximum allowed page size.</param>
    /// <param name="cursorSupported">Whether cursor-style paging is supported.</param>
    public PageableAttribute(int defaultSize, int maxSize, bool cursorSupported)
    {
        DefaultSize = defaultSize;
        MaxSize = maxSize;
        CursorSupported = cursorSupported;
    }

    public int DefaultSize { get; }
    public int MaxSize { get; }
    public bool CursorSupported { get; }
}
