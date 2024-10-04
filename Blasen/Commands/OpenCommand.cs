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
using System.Windows.Input;
using System.Windows.Media;

namespace Blasen.Commands
{
    public class OpenCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;


        public OpenCommand(ViewModel vm)
        {
            this.viewModel = vm;
        }


        public OpenCommand(Control control)
        {
            this.control = control;
        }



        private readonly ViewModel viewModel;

        private readonly Control control;


        public VideoPlayController Player { get; private set; }



        public bool CanExecute(object parameter)
        {
            return true;
        }


        public void Execute(object parameter)
        {
            if (parameter is Image image)
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "すべてのファイル(*.*)|*.*";

                // ダイアログを表示
                var result = dialog.ShowDialog();

                // 開くを選択されたら
                if (result == true)
                {
                    this.Player = new VideoPlayController();
                    this.Player.OpenFile(dialog.FileName);

                    var presentationSource = PresentationSource.FromVisual(image);
                    Matrix matrix = presentationSource.CompositionTarget.TransformFromDevice;
                    var dpiX = (int)Math.Round(96 * (1 / matrix.M11));
                    var dpiY = (int)Math.Round(96 * (1 / matrix.M22));

                    var bitmap = this.Player.CreateBitmap(dpiX, dpiY);

                    Binding binding = new Binding();
                    binding.Source = image;
                    binding.Path = new PropertyPath("Source");
                    image.SetBinding(Image.SourceProperty, binding);
                }
            }
        }
    }
}
