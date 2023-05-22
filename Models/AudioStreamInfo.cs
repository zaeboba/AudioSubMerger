using Xabe.FFmpeg;

namespace AudioSubMerger.Models;

public class AudioStreamInfo
{
    public AudioStreamInfo(IStream stream, string title, string language)
    {
        Stream = stream;
        Title = title;
        Language = language;
    }
    public IStream Stream { get; }
    public string Title { get; }
    public string Language { get; }
}