using AudioSubMerger.Models;
using Xabe.FFmpeg;

namespace AudioSubMerger.Helpers;

public class SubtitleMergeHelper(IEnumerable<string> subtitleExtensions, string subtitleDirectoriesPath)
{
    private readonly string[] _subtitleExtensions = subtitleExtensions.ToArray();

    public async Task<IEnumerable<SubtitleStreamInfo>> GetSubtitleStreamsAsync(
        IMediaInfo mediaInfo,
        string videoName)
    {
        Console.WriteLine("Получаем субтитры");

        var subtitleStreams = new List<SubtitleStreamInfo>();
        var subtitleDirectories = GeneralHelper.GetDirectoriesFromPath(subtitleDirectoriesPath)
            .OrderBy(t => t.Name)
            .ToList();
        var numberedSubtitleDirectories =
            subtitleDirectories.Where(t => char.IsDigit(t.Name.FirstOrDefault())).ToList();
        var otherSubtitleDirectories = subtitleDirectories.Except(numberedSubtitleDirectories);
        foreach (var subtitleDirectory in numberedSubtitleDirectories)
        {
            var stream = await GetStreamFromDirectory(videoName, subtitleDirectory);
            if (stream == null)
            {
                continue;
            }

            subtitleStreams.Add(stream);
        }

        subtitleStreams.AddRange(mediaInfo.SubtitleStreams.Select(originalSubtitleStream =>
            new SubtitleStreamInfo(originalSubtitleStream, originalSubtitleStream.Title)));

        foreach (var subtitleDirectory in otherSubtitleDirectories)
        {
            var stream = await GetStreamFromDirectory(videoName, subtitleDirectory);
            if (stream == null)
            {
                continue;
            }

            subtitleStreams.Add(stream);
        }

        return subtitleStreams;
    }

    public void AddSubtitleStreamsToConversion(IConversion conversion, List<SubtitleStreamInfo> subtitleStreams)
    {
        var subtitleStreamIndex = 0;

        foreach (var subtitleStream in subtitleStreams)
        {
            conversion.AddStream(subtitleStream.Stream);
            if (!string.IsNullOrWhiteSpace(subtitleStream.Name))
            {
                conversion.AddParameter($" -metadata:s:s:{subtitleStreamIndex} title=\"{subtitleStream.Name}\"");
            }

            subtitleStreamIndex++;
        }
    }

    private async Task<SubtitleStreamInfo?> GetStreamFromDirectory(string videoName, DirectoryInfo subtitleDirectory)
    {
        var subtitleFile = subtitleDirectory
            .GetFiles($"*{videoName}*")
            .FirstOrDefault(t => _subtitleExtensions.Contains(t.Extension));

        if (subtitleFile == null)
        {
            return null;
        }

        var subtitleMediaInfo = await FFmpeg.GetMediaInfo(subtitleFile.FullName);
        var subtitleStream = subtitleMediaInfo.SubtitleStreams.FirstOrDefault();
        if (subtitleStream == null)
        {
            return null;
        }

        return new SubtitleStreamInfo(subtitleStream, subtitleDirectory.Name);
    }
}