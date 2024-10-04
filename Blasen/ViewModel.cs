using Blasen.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Blasen
{
    public class ViewModel : IDataContext
    {
        public ViewModel()
        {
            var model = new Model();
            this.Margin = model.Margin;
        }


        private Control control;




        public int Margin { get; }




        public ICommand Open { get; private set; }


        public ICommand Exit { get; private set; }




        public void OnAttached(Window window)
        {
            if (window is not null)
            {
                this.Open = new OpenCommand(window);
                this.Exit = new ExitCommand(window);
            }
        }

        public void OnDetached(Window window)
        {
        }
    }
}
