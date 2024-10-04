using Blasen.FFmpeg;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Blasen
{
    /// <summary>
    /// ScreenLockWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ScreenLockWindow : Window
    {
        public ScreenLockWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            this.Show();

            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result == true)
            {
                videoPlayer = new VideoPlayController();
                videoPlayer.OpenFile(dialog.FileName);

                var presentationSource = PresentationSource.FromVisual(this);
                Matrix matrix = presentationSource.CompositionTarget.TransformFromDevice;
                var dpiX = (int)Math.Round(96 * (1 / matrix.M11));
                var dpiY = (int)Math.Round(96 * (1 / matrix.M22));

                Bitmap = videoPlayer.CreateBitmap(dpiX, dpiY);

                videoPlayer.Play();
            }
        }


        private VideoPlayController videoPlayer;


        public WriteableBitmap Bitmap { get; private set; }
    }
}
