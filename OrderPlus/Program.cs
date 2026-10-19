using System;
using System.Windows;
using Common.Persistance;
using OrderPlus.Windows;

namespace OrderPlus
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Context.PM = new PersistanceManager();

            Application app = new Application();
            MainWindow mainWindow = new MainWindow();
            app.Run(mainWindow);
        }
    }
}