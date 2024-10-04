using FFmpeg.AutoGen;

namespace Blasen.FFmpeg
{
    public class AudioPCMInt16Format : AudioOutputFormat
    {
        public AudioPCMInt16Format() { }


        public override AVSampleFormat AVSampleFormat => AVSampleFormat.AV_SAMPLE_FMT_S16;

        public override int SizeOf => sizeof(ushort);
    }
}
