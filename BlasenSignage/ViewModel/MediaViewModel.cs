using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            media.Source = new Uri("C:\\Users\\daisuke\\Downloads\\デジタルサイネージ1001.mp4", UriKind.Relative);
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
