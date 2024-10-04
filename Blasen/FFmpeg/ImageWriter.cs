using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Blasen.FFmpeg
{
    public class ImageWriter
    {
        private readonly Int32Rect rect;
        private readonly WriteableBitmap writeableBitmap;



        public ImageWriter(int width, int height, WriteableBitmap writeableBitmap)
        {
            this.rect = new Int32Rect(0, 0, width, height);
            this.writeableBitmap = writeableBitmap;
        }


        public void WriteFrame(ManagedFrame frame, FrameConverter frameConverter)
        {
            var bitmap = writeableBitmap;
            bitmap.Lock();
            try
            {
                var ptr = bitmap.BackBuffer;
                frameConverter.ConvertFrameDirect(frame, ptr);
                bitmap.AddDirtyRect(rect);
            }
            finally
            {
                bitmap.Unlock();
            }
        }
    }
}
