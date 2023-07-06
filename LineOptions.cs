using CommandLine;

namespace AudioSubMerger;

public class LineOptions
{
    [Option("vid", Required = true, HelpText = "Путь до папки с видео (абсолютный)")]
    public string VideoPath { get; set; } = null!;

    [Option("aud", Default = @"Sounds", Required = false,
        HelpText = "Путь до папки с аудио (можно абсолютный, можно относительный)")]
    public string AudioPath { get; set; } = @"Sounds";

    [Option("sub", Default = @"Subtitles", Required = false,
        HelpText = "Путь до папки с субтитрами (можно абсолютный, можно относительный)")]
    public string SubtitlesPath { get; set; } = @"Subtitles";

    [Option("videoExtensions", Default = new[] { ".mkv" }, Required = false,
        HelpText = "Расширения для аудио через запятую", Separator = ',')]
    public IEnumerable<string> VideoExtensions { get; set; } = null!;

    [Option("audioExtensions", Default = new[] { ".mka" }, Required = false,
        HelpText = "Расширения для аудио через запятую", Separator = ',')]
    public IEnumerable<string> AudioExtensions { get; set; } = null!;

    [Option("subtitleExtensions", Default = new[] { ".ass" }, Required = false,
        HelpText = "Расширения для субтитров через запятую", Separator = ',')]
    public IEnumerable<string> SubtitleExtensions { get; set; } = null!;
}