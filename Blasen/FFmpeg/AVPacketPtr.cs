using FFmpeg.AutoGen;

namespace Blasen.FFmpeg
{
    public unsafe struct AVPacketPtr
    {
        public AVPacket* Ptr;

        public AVPacketPtr(AVPacket* ptr)
        {
            Ptr = ptr;
        }
    }
}
