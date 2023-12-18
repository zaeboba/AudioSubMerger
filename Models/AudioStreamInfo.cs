using Xabe.FFmpeg;

namespace AudioSubMerger.Models;

public class AudioStreamInfo(IStream stream, string title, string language)
{
    public IStream Stream { get; } = stream;
    public string Title { get; } = title;
    public string Language { get; } = language;
}