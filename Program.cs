using AudioSubMerger;
using AudioSubMerger.Helpers;
using CommandLine;
using Xabe.FFmpeg;

LineOptions options = new LineOptions();

// читаем агрументы
CommandLine.Parser.Default.ParseArguments<LineOptions>(args)
    .WithParsed(RunOptions)
    .WithNotParsed(HandleParseError);


var audioPath = GeneralHelper.IsAbsolutePath(options.AudioPath)
    ? options.AudioPath
    : Path.Combine(options.VideoPath, options.AudioPath);

var subtitlePath = GeneralHelper.IsAbsolutePath(options.SubtitlesPath)
    ? options.SubtitlesPath
    : Path.Combine(options.VideoPath, options.SubtitlesPath);

var audioMerger = new AudioMergeHelper(options.AudioExtensions, audioPath);
var subtitleMerger = new SubtitleMergeHelper(options.SubtitleExtensions, subtitlePath);

var videoFiles = GeneralHelper.GetFilesFromDirectoryWithExtensions(options.VideoPath, options.VideoExtensions);
foreach (var videoFile in videoFiles)
{
    Console.WriteLine($"Обрабатываем {videoFile.FullName}");
    var fileName = videoFile.Name;
    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
    var fileDirectory = videoFile.DirectoryName;
    var newDirectory = Path.Combine(fileDirectory, "Merged");
    Directory.CreateDirectory(newDirectory);

    var mergedFilePath = Path.Combine(newDirectory, $"{fileName}");
    if (File.Exists(mergedFilePath))
    {
        Console.WriteLine($"Файл {mergedFilePath} уже существует, пропускаем");
        continue;
    }

    var mediaInfo = await FFmpeg.GetMediaInfo(videoFile.FullName);
    var conversion = FFmpeg.Conversions
        .New()
        .AddStream(mediaInfo.VideoStreams);

    var audioStreams = (await audioMerger.GetAudioStreamsWithTitlesAsync(mediaInfo, fileNameWithoutExtension)).ToList();
    audioMerger.AddAudioStreamsToConversion(conversion, audioStreams);

    var subtitleStreams = (await subtitleMerger.GetSubtitleStreamsAsync(mediaInfo, fileNameWithoutExtension)).ToList();
    subtitleMerger.AddSubtitleStreamsToConversion(conversion, subtitleStreams);

    conversion.SetOutput(mergedFilePath);
    conversion.AddParameter("-c:v copy -c:a copy -c:s srt -shortest -disposition:s:0 0");
    conversion.OnProgress += async (_, args) =>
    {
        //Show all output from FFmpeg to console
        if (args.Percent % 5 != 0)
        {
            return;
        }

        await Console.Out.WriteLineAsync($"[{args.Duration}/{args.TotalLength}][{args.Percent}%] {fileName}");
    };

    await conversion.Start();
}

void RunOptions(LineOptions opts)
{
    options = opts;
}

void HandleParseError(IEnumerable<Error> errs)
{
    Console.WriteLine("Поправь параметры и попробуй снова");
    Environment.Exit(0);
}