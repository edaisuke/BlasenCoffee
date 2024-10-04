using System.Windows;
using System.Windows.Input;

namespace Blasen.Commands
{
    public class ExitCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;


        public ExitCommand(Window window)
        {
            this.window = window;
        }

        private readonly Window window;


        public bool CanExecute(object parameter)
        {
            return true;
        }


        public void Execute(object parameter)
        {
            window.Close();
        }
    }
}
