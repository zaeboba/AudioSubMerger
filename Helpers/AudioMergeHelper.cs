using AudioSubMerger.Models;
using Xabe.FFmpeg;

namespace AudioSubMerger.Helpers;

public class AudioMergeHelper
{
    private readonly string _audioDirectoriesPath;
    private readonly string[] _audioExtensions;

    public AudioMergeHelper(IEnumerable<string> audioExtensions, string audioDirectoriesPath)
    {
        _audioExtensions = audioExtensions.ToArray();
        _audioDirectoriesPath = audioDirectoriesPath;
    }

    public async Task<IEnumerable<AudioStreamInfo>> GetAudioStreamsWithTitlesAsync(
        IMediaInfo mediaInfo,
        string videoName)
    {
        Console.WriteLine("Получаем аудио дорожки");

        var audioStreams = new List<AudioStreamInfo>();
        var audioDirectories = GeneralHelper.GetDirectoriesFromPath(_audioDirectoriesPath)
            .OrderBy(t => t.Name)
            .ToList();
        var numberedAudioDirectories = audioDirectories.Where(t => char.IsDigit(t.Name.FirstOrDefault())).ToList();
        var otherAudioDirectories = audioDirectories.Except(numberedAudioDirectories);
        foreach (var audioDirectory in numberedAudioDirectories)
        {
            var stream = await GetStreamFromDirectory(videoName, audioDirectory);
            if (stream == null)
            {
                continue;
            }

            audioStreams.Add(stream);
        }

        audioStreams.AddRange(mediaInfo.AudioStreams.Select(originalAudioStream =>
            new AudioStreamInfo(originalAudioStream, originalAudioStream.Title, originalAudioStream.Language)));

        foreach (var audioDirectory in otherAudioDirectories)
        {
            var stream = await GetStreamFromDirectory(videoName, audioDirectory);
            if (stream == null)
            {
                continue;
            }

            audioStreams.Add(stream);
        }

        return audioStreams;
    }

    public void AddAudioStreamsToConversion(IConversion conversion, List<AudioStreamInfo> audioStreams)
    {
        foreach (var audioStream in audioStreams)
        {
            conversion.AddStream(audioStream.Stream);
        }

        var audioStreamIndex = 0;
        foreach (var audioStream in audioStreams)
        {
            if (!string.IsNullOrEmpty(audioStream.Title))
            {
                conversion.AddParameter($" -metadata:s:a:{audioStreamIndex} title=\"{audioStream.Title}\"");
            }

            if (!string.IsNullOrEmpty(audioStream.Language))
            {
                conversion.AddParameter($" -metadata:s:a:{audioStreamIndex} language=\"{audioStream.Language}\"");
            }

            audioStreamIndex++;
        }
    }

    private async Task<AudioStreamInfo?> GetStreamFromDirectory(string videoName, DirectoryInfo audioDirectory)
    {
        var audioFile = audioDirectory
            .GetFiles($"*{videoName}*")
            .FirstOrDefault(t => _audioExtensions.Contains(t.Extension));

        if (audioFile == null)
        {
            return null;
        }

        var audioMediaInfo = await FFmpeg.GetMediaInfo(audioFile.FullName);
        var audioStream = audioMediaInfo.AudioStreams.FirstOrDefault();
        if (audioStream == null)
        {
            return null;
        }

        //remove numbers from beginning of directory name
        var audioTitle = audioDirectory.Name.TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9').Trim();

        return new AudioStreamInfo(audioStream, audioTitle, audioStream.Language);
    }
}