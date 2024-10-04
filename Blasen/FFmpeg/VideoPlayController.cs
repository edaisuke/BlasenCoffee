using FFmpeg.AutoGen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Blasen.FFmpeg
{
    public class VideoPlayController
    {
        private static readonly AVPixelFormat ffPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
        private static readonly PixelFormat wpfPixelFormat = PixelFormats.Bgr24;


        public VideoPlayController() { }



        const int frameCap = 4;

        const int waitTime = 10;


        private Decoder decoder;

        private ImageWriter imageWriter;

        private FrameConverter frameConverter;


        private bool isFrameEnded;

        private ConcurrentQueue<ManagedFrame> frames = new ConcurrentQueue<ManagedFrame>();



        public void OpenFile(string path)
        {
            decoder = new Decoder();
            decoder.OpenFile(path);
        }



        public WriteableBitmap CreateBitmap(int dpiX, int dpiY)
        {
            if (decoder is null)
            {
                throw new InvalidOperationException("描画先を作成する前に動画を開く必要があります。");
            }

            var context = decoder.VideoCodecContext;
            var width = context.width;
            var height = context.height;

            var writeableBitmap = new WriteableBitmap(width, height, dpiX, dpiY, wpfPixelFormat, null);
            this.imageWriter = new ImageWriter(width, height, writeableBitmap);

            this.frameConverter = new FrameConverter();
            this.frameConverter.Configure(context.pix_fmt, context.width, context.height, ffPixelFormat, width, height);

            return writeableBitmap;
        }



        public async Task Play()
        {
            await PlayInternal();
        }


        private async Task PlayInternal()
        {
            await Task.Run(ReadFrames);

            AudioData firstData;
            using (var frame = decoder.ReadAudioFrame())
            {
                firstData = AudioFrameConverter.ConvertTo<AudioPCMInt16Format>(frame);
            }

            MemoryStream stream = new();
            AudioPlayer audioPlayer = new();

            stream.Write(firstData.AsSpan());

            while (true)
            {
                using (var frame2 = decoder.ReadAudioFrame())
                {
                    if (frame2 is null)
                    {
                        break;
                    }
                    using (var audioData2 = AudioFrameConverter.ConvertTo<AudioPCMInt16Format>(frame2))
                    {
                        stream.Write(audioData2.AsSpan());
                    }
                }
            }

            stream.Position = 0;
            var source = audioPlayer.FromInt16(stream, firstData.SampleRate, firstData.Channel);
            firstData.Dispose();

            await WaitForBuffer();

            var fps = decoder.VideoStream.r_frame_rate;

            var stopwatch = Stopwatch.StartNew();
            var skipped = 0;
            List<double> delays = new();

            for (var i = 0; ; i++)
            {
                var time = TimeSpan.FromMilliseconds(fps.den * i * 1000L / (double)fps.num);
                if (stopwatch.Elapsed < time)
                {
                    var rem = time - stopwatch.Elapsed;
                    await Task.Delay(rem);
                }

                if (frames.TryDequeue(out var frame))
                {
                    imageWriter.WriteFrame(frame, frameConverter);
                    if (i == 0)
                    {
                        await audioPlayer.Play(source, 50, 500);
                    }
                    frame.Dispose();
                }
                else
                {
                    if (isFrameEnded)
                    {
                        audioPlayer.Dispose();
                        stream.Dispose();
                        return;
                    }

                    skipped++;
                    Debug.WriteLine($"frame skipped(frame={1},total={skipped}/{i})");
                }
            }
        }



        private async Task WaitForBuffer()
        {
            while (true)
            {
                if (frames.Count == frameCap)
                {
                    return;
                }
                await ReadFrames();
            }
        }


        private async Task ReadFrames()
        {
            while (true)
            {
                if (frames.Count < frameCap)
                {
                    var frame = this.decoder.ReadFrame();
                    if (frame is null)
                    {
                        isFrameEnded = true;
                        return;
                    }
                    frames.Enqueue(frame);
                }
                else
                {
                    await Task.Delay(waitTime);
                }
            }
        }
    }
}
