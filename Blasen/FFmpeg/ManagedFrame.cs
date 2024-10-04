using FFmpeg.AutoGen;
using System;

namespace Blasen.FFmpeg
{
    public unsafe class ManagedFrame : IDisposable
    {
        public ManagedFrame(AVFrame* frame)
        {
            this.frame = frame;
        }


        private readonly AVFrame* frame;

        private bool isDisposed = false;



        public AVFrame* Frame { get =>  frame; }


        ~ManagedFrame()
        {
            DisposeUnmanaged();
        }


        public void Dispose()
        {
            DisposeUnmanaged();
            GC.SuppressFinalize(this);
        }

        private void DisposeUnmanaged()
        {
            if (isDisposed) { return; }

            AVFrame* avFrame = frame;
            ffmpeg.av_frame_free(&avFrame);

            this.isDisposed = true;
        }
    }
}
