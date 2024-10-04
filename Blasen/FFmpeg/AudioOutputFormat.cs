using FFmpeg.AutoGen;

namespace Blasen.FFmpeg
{
    public abstract class AudioOutputFormat
    {
        public abstract AVSampleFormat AVSampleFormat { get; }

        public abstract int SizeOf { get; }
    }
}
