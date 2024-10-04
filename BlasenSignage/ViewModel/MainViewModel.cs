using BlasenSignage.Services;
using Microsoft.Xaml.Behaviors.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace BlasenSignage.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private HttpServerService httpServerService;

        private bool topmost = false;

        private Cursor cursor = Cursors.None;

        private bool isShowCursor = false;

        private double rowSplitterHeight = 10d;

        private string row1Height = "*";

        private string row2Height = "Auto";

        private string row3Height = "*";



        public MainViewModel()
        {
            this.SetFullScreen();
        }



        public void SetFullScreen()
        {
            this.RowSplitterHeight = 0d;
            this.Row1Height = "*";
            this.Row2Height = "0";
            this.Row3Height = "0";
        }



        public ICommand LoadedCommand { get; } = new ActionCommand((obj) =>
        {
            if (obj is Window window)
            {
                if (window.DataContext is MainViewModel model)
                {
                    model.httpServerService = new HttpServerService();
                    model.httpServerService.Start();
                }
            }
        });



        public ICommand ExitCommand { get; private set; } = new ActionCommand((obj) =>
        {
            if (obj is Window window)
            {
                if (window.DataContext is MainViewModel model)
                {
                    model.httpServerService.Stop();
                    window.Close();
                }
            }
        });



        public ICommand ToggleCurosrCommand { get; private set; } = new ActionCommand((obj) =>
        {
            if (obj is Window window)
            {
                if (window.DataContext is MainViewModel model)
                {
                    if (model.Cursor == Cursors.None)
                    {
                        model.Cursor = Cursors.AppStarting;
                    }
                    else
                    {
                        model.Cursor = Cursors.None;
                    }
                }
            }
        });


        public ICommand ToggleTopmostCommand { get; private set; } = new ActionCommand((obj) =>
        {
            if (obj is Window window)
            {
                if (window.DataContext is MainViewModel model)
                {
                    model.Topmost = !model.Topmost;
                }
            }
        });




        public bool Topmost
        {
            get => topmost;
            set
            {
                topmost = value;
                OnPropertyChanged();
            }
        }





        public Cursor Cursor
        {
            get => cursor;
            set
            {
                cursor = value;
                OnPropertyChanged();
                IsShowCursor = cursor != Cursors.None;
            }
        }



        public bool IsShowCursor
        {
            get => isShowCursor;
            set
            {
                isShowCursor = value;
                OnPropertyChanged();
            }
        }
           



        public double RowSplitterHeight
        {
            get => rowSplitterHeight;
            set
            {
                rowSplitterHeight = value;
                OnPropertyChanged();
            }
        }


        public string Row1Height
        {
            get => row1Height;
            set
            {
                row1Height = value;
                OnPropertyChanged();
            }
        }


        public string Row2Height
        {
            get => row2Height;
            set
            {
                row2Height = value;
                OnPropertyChanged();
            }
        }

        public string Row3Height
        {
            get => row3Height;
            set
            {
                row3Height = value;
                OnPropertyChanged();
            }
        }


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
