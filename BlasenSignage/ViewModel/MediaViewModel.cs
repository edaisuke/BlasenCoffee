using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BlasenSignage.ViewModel
{
    public class MediaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public MediaViewModel()
        {
        }




        public ICommand LoadedCommand { get; private set; } = new ActionCommand((obj) =>
        {
            var args = (RoutedEventArgs)obj;
            var media = (MediaElement)args.Source;

            var location = Assembly.GetEntryAssembly().Location;
            var directory = Path.GetDirectoryName(location);
            var source = Path.Combine(directory, "Media");
            var sourcePath = Path.Combine(source, "デジタルサイネージ1001.mp4");

            media.Source = new Uri(sourcePath, UriKind.RelativeOrAbsolute);
            media.Play();
        });


        public ICommand MediaEndedCommand { get; private set; } = new ActionCommand((obj) =>
        {
            var args = (RoutedEventArgs)obj;
            var media = (MediaElement)args.Source;
            media.Position = TimeSpan.FromMilliseconds(1);
            media.Play();
        });



        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
