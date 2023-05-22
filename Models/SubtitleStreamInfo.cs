using Xabe.FFmpeg;

namespace AudioSubMerger.Models;

public class SubtitleStreamInfo
{
    public SubtitleStreamInfo(IStream stream)
    {
        Stream = stream;
    }

    public IStream Stream { get; set; }
}