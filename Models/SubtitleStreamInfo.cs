using Xabe.FFmpeg;

namespace AudioSubMerger.Models;

public class SubtitleStreamInfo(IStream stream, string? name)
{
    public IStream Stream { get; set; } = stream;
    public string? Name { get; set; } = name;
}