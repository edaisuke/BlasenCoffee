using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blasen.FFmpeg
{
    public unsafe class Decoder : IDisposable
    {
        public Decoder()
        {
            var ffmpegRootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"ffmpeg-7.0.2-full_build-shared\bin");
            ffmpeg.RootPath = ffmpegRootPath;
        }


        private bool isDisposed = false;


        private AVFormatContext* formatContext;


        private AVStream* videoStream;

        private AVStream* audioStream;


        private AVCodec* videoCodec;

        private AVCodec* audioCodec;


        private AVCodecContext* videoCodecContext;

        private AVCodecContext* audioCodecContext;



        private bool isAudioFrameEnded;

        private bool isVideoFrameEnded;





        public AVFormatContext FormatContext { get => *formatContext; }

        public AVStream VideoStream { get => *videoStream; }

        public AVStream AudioStream { get => *audioStream; }

        public AVCodec VideoCodec { get => *videoCodec; }

        public AVCodecContext VideoCodecContext { get => *videoCodecContext; }

        public AVCodecContext AudioCodecContext { get => *audioCodecContext; }

        private object sendPackedSyncObject = new object();

        private Queue<AVPacketPtr> videoPackets = new Queue<AVPacketPtr>();
        private Queue<AVPacketPtr> audioPackets = new Queue<AVPacketPtr>();




        public void OpenFile(string path)
        {
            AVFormatContext* formatContext = null;
            ffmpeg.avformat_open_input(&formatContext, path, null, null);

            ffmpeg.avformat_find_stream_info(formatContext, null);

            this.formatContext = formatContext;

            videoStream = GetFirstVideoStream();
            audioStream = GetFirstAudioStream();

            if (videoStream != null)
            {
                videoCodec = ffmpeg.avcodec_find_decoder(videoStream->codecpar->codec_id);
                if (videoCodec == null)
                {
                    Debug.WriteLine("必要な動画デコーダを検出できませんでした。");
                }

                videoCodecContext = ffmpeg.avcodec_alloc_context3(videoCodec);
                if (videoCodecContext is null)
                {
                    throw new InvalidOperationException("動画コーデックのCodecContextの確保に失敗しました。");
                }
                ffmpeg.avcodec_parameters_to_context(videoCodecContext, videoStream->codecpar);
                ffmpeg.avcodec_open2(videoCodecContext, videoCodec, null);
            }
            if (audioStream != null)
            {
                audioCodec = ffmpeg.avcodec_find_decoder(audioStream->codecpar->codec_id);
                if (audioCodec == null)
                {
                    Debug.WriteLine("必要な音声デコーダを検出できませんでした。");
                }
                else
                {
                    audioCodecContext = ffmpeg.avcodec_alloc_context3(audioCodec);
                    if (audioCodecContext == null)
                    {
                        Debug.WriteLine("音声コーデックのCodecContextの確保に失敗しました。");
                    }
                    else
                    {
                        ffmpeg.avcodec_parameters_to_context(audioCodecContext, audioStream->codecpar);
                        ffmpeg.avcodec_open2(audioCodecContext, audioCodec, null);
                    }
                }
            }
        }



        public unsafe ManagedFrame ReadFrame()
        {
            var frame = this.ReadUnsafeFrame();
            if (frame == null)
            {
                return null;
            }

            return new ManagedFrame(frame);
        }



        private AVStream* GetFirstVideoStream()
        {
            for (var i = 0; i < formatContext->nb_streams; ++i)
            {
                var stream = formatContext->streams[i];
                if (stream->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    return stream;
                }
            }
            return null;
        }

        private AVStream* GetFirstAudioStream()
        {
            for (var i = 0; i < (int)formatContext->nb_streams; i++)
            {
                var stream = formatContext->streams[i];
                if (stream->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
                {
                    return stream;
                }
            }
            return null;
        }



        public unsafe ManagedFrame ReadAudioFrame()
        {
            var frame = ReadUnsafeAudioFrame();
            if (frame is null)
            {
                return null;
            }
            return new ManagedFrame(frame);
        }



        public unsafe AVFrame* ReadUnsafeAudioFrame()
        {
            AVFrame* frame = ffmpeg.av_frame_alloc();

            if (ffmpeg.avcodec_receive_frame(audioCodecContext, frame) == 0)
            {
                return frame;
            }
            if (isAudioFrameEnded)
            {
                return null;
            }

            while (SendPacket(audioStream->index) == 0)
            {
                if (ffmpeg.avcodec_receive_frame(audioCodecContext, frame) == 0)
                {
                    return frame;
                }
            }

            isAudioFrameEnded = true;
            ffmpeg.avcodec_send_packet(audioCodecContext, null);
            if (ffmpeg.avcodec_receive_frame(audioCodecContext, frame) == 0)
            {
                return frame;
            }

            return null;
        }



        public int SendPacket(int index)
        {
            lock (sendPackedSyncObject)
            {
                if (index == this.VideoStream.index)
                {
                    if (videoPackets.TryDequeue(out var ptr))
                    {
                        ffmpeg.avcodec_send_packet(videoCodecContext, ptr.Ptr);
                        ffmpeg.av_packet_unref(ptr.Ptr);
                        return 0;
                    }
                }
                if (index == this.AudioStream.index)
                {
                    if (audioPackets.TryDequeue(out var ptr))
                    {
                        ffmpeg.avcodec_send_packet(audioCodecContext, ptr.Ptr);
                        ffmpeg.av_packet_unref(ptr.Ptr);
                        return 0;
                    }
                }


                while (true)
                {
                    var packet = new AVPacket();

                    var result = ffmpeg.av_read_frame(formatContext, &packet);
                    if (result == 0)
                    {
                        if (packet.stream_index == videoStream->index)
                        {
                            if (packet.stream_index == videoStream->index)
                            {
                                ffmpeg.avcodec_send_packet(videoCodecContext, &packet);
                                ffmpeg.av_packet_unref(&packet);
                                return 0;
                            }
                            else
                            {
                                var _packet = ffmpeg.av_packet_clone(&packet);
                                videoPackets.Enqueue(new AVPacketPtr(_packet));
                                continue;
                            }
                        }

                        if (packet.stream_index == audioStream->index)
                        {
                            if (packet.stream_index == index)
                            {
                                ffmpeg.avcodec_send_packet(audioCodecContext, &packet);
                                ffmpeg.av_packet_unref(&packet);
                                return 0;
                            }
                            else
                            {
                                var _packet = ffmpeg.av_packet_clone(&packet);
                                audioPackets.Enqueue(new AVPacketPtr(_packet));
                                continue;
                            }
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }


        public unsafe AVFrame* ReadUnsafeFrame()
        {
            var frame = ffmpeg.av_frame_alloc();

            var vcc = VideoCodecContext;

            if (ffmpeg.avcodec_receive_frame(&vcc, frame) == 0)
            {
                return frame;
            }
            if (isVideoFrameEnded)
            {
                return null;
            }

            var vs = VideoStream;
            var videoStream = &vs;
            int n;
            while ((n = SendPacket(videoStream->index)) == 0)
            {
                if (ffmpeg.avcodec_receive_frame(&vcc, frame) == 0)
                {
                    return frame;
                }
                else { }
            }

            isVideoFrameEnded = true;
            ffmpeg.avcodec_send_packet(&vcc, null);
            if (ffmpeg.avcodec_receive_frame(&vcc, frame) == 0)
            {
                return frame;
            }

            return null;
        }



        ~Decoder()
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

            var codecContext = videoCodecContext;
            var formatContext = this.formatContext;

            ffmpeg.avcodec_free_context(&codecContext);
            ffmpeg.avformat_close_input(&formatContext);

            isDisposed = true;
        }
    }
}
