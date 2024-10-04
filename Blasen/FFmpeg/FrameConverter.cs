using FFmpeg.AutoGen;
using System;

namespace Blasen.FFmpeg
{
    public unsafe class FrameConverter : IDisposable
    {
        public FrameConverter() { }


        private AVPixelFormat srcFormat;
        private int srcWidth;
        private int srcHeight;

        private AVPixelFormat dstFormat;
        private int dstWidth;
        private int dstHeight;


        private SwsContext* swsContext;


        private bool isDisposed = false;



        public void Configure(AVPixelFormat srcFormat, int srcWidth, int srcHeight, AVPixelFormat dstFormat, int dstWidth, int dstHeight)
        {
            this.srcFormat = srcFormat;
            this.srcWidth = srcWidth;
            this.srcHeight = srcHeight;

            this.dstFormat = dstFormat;
            if (this.dstWidth == dstWidth || this.dstHeight == dstHeight)
            {
                return;
            }
            this.dstWidth = dstWidth;
            this.dstHeight = dstHeight;

            ffmpeg.sws_freeContext(swsContext);
            swsContext = ffmpeg.sws_getContext(srcWidth, srcHeight, srcFormat, dstWidth, dstHeight, dstFormat, 0, null, null, null);

        }



        public unsafe byte* ConvertFrame(ManagedFrame frame)
        {
            return ConvertFrame(frame.Frame);
        }


        public unsafe byte* ConvertFrame(AVFrame* frame)
        {
            byte_ptrArray4 data = default;
            int_array4 lizesize = default;

            byte* buffer = (byte*)ffmpeg.av_malloc((ulong)ffmpeg.av_image_get_buffer_size(dstFormat, srcWidth, srcHeight, 1));

            ffmpeg.av_image_fill_arrays(ref data, ref lizesize, buffer, dstFormat, srcWidth, srcHeight, 1);

            ffmpeg.sws_scale(swsContext, frame->data, frame->linesize, 0, srcHeight, data, lizesize);

            return buffer;
        }


        public unsafe void ConvertFrameDirect(ManagedFrame frame, IntPtr buffer)
        {
            ConvertFrameDirect(frame.Frame, (byte*)buffer.ToPointer());
        }



        public unsafe void ConvertFrameDirect(AVFrame* frame, byte* buffer)
        {
            byte_ptrArray4 data = default;
            int_array4 lizesize = default;

            ffmpeg.av_image_fill_arrays(ref data, ref lizesize, buffer, dstFormat, srcWidth, srcHeight, 1);
            ffmpeg.sws_scale(swsContext, frame->data, frame->linesize, 0, srcHeight, data, lizesize);
        }



        public void Dispose()
        {
            DisposeUnmanaged();
            GC.SuppressFinalize(this);
        }


        ~FrameConverter()
        {
            DisposeUnmanaged();
        }


        private void DisposeUnmanaged()
        {
            if (isDisposed) { return; }

            ffmpeg.sws_freeContext(swsContext);
            isDisposed = true;
        }
    }
}
