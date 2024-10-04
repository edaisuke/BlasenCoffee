using System.Runtime.InteropServices;

namespace Blasen.FFmpeg
{
    public class AudioData : IDisposable
    {
        public int Samples { get; set; }

        public int SampleRate { get; set; }

        public int Channel { get; set; }

        public int SizeOf { get; set; }


        public IntPtr Data { get; set; }



        public unsafe ReadOnlySpan<byte> AsSpan()
        {
            return new ReadOnlySpan<byte>(Data.ToPointer(), Samples * Channel * SizeOf);
        }


        public void Dispose()
        {
            Marshal.FreeHGlobal(Data);
        }
    }
}
