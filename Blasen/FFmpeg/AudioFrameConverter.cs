using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Blasen.FFmpeg
{
    public class AudioFrameConverter
    {
        public static unsafe AudioData ConvertTo<TOut>(ManagedFrame frame)
            where TOut : AudioOutputFormat, new()
        {
            return ConvertTo<TOut>(frame.Frame);
        }


        public static unsafe AudioData ConvertTo<TOut>(AVFrame* frame)
            where TOut : AudioOutputFormat, new()
        {
            var output = new TOut();
            var context = ffmpeg.swr_alloc();

            var chLayout = frame->ch_layout;
            ffmpeg.av_opt_set_chlayout(context, "in_ch_layout", &chLayout, 0);
            ffmpeg.av_opt_set_chlayout(context, "out_ch_layout", &chLayout, 0);
            ffmpeg.av_opt_set_int(context, "in_sample_rate", frame->sample_rate, 0);
            ffmpeg.av_opt_set_int(context, "out_sample_rate", frame->sample_rate, 0);
            ffmpeg.av_opt_set_sample_fmt(context, "in_sample_fmt", (AVSampleFormat)frame->format, 0);
            ffmpeg.av_opt_set_sample_fmt(context, "out_sample_fmt", output.AVSampleFormat, 0);
            ffmpeg.swr_init(context);

            var size = output.SizeOf;

            var bufferSize = frame->nb_samples * frame->ch_layout.nb_channels * size;
            var buffer = Marshal.AllocHGlobal(bufferSize);
            byte* ptr = (byte*)buffer.ToPointer();

            ffmpeg.swr_convert(context, &ptr, frame->nb_samples, frame->extended_data, frame->nb_samples);

            return new AudioData()
            {
                Samples = frame->nb_samples,
                SampleRate = frame->sample_rate,
                Channel = frame->ch_layout.nb_channels,
                SizeOf = size,
                Data = buffer,
            };
        }
    }
}
