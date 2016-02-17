using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace TelerikOutlookInspiredApp
{
    public static class Commands
    {
        private static readonly ICommand exitCommand = new DelegateCommand(OnExitCommandExecuted, CanExitCommandExecute);
       
        public static ICommand ExitCommand
        {
            get { return exitCommand; }
        }

        private static bool CanExitCommandExecute(object obj)
        {
            return Application.Current != null && Application.Current.MainWindow != null;
        }

        private static void OnExitCommandExecuted(object obj)
        {
            Application.Current.MainWindow.Close();
        }
    }
}
