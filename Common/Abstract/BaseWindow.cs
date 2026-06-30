using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OrderPlus.ViewModels;

namespace OrderPlus.Windows
{
    public abstract class BaseWindow : Window
    {
        public Grid MainGrid {get; private set;}

        public BaseWindow()
        {
            GenerateGrid();
            InitializeViewModel();
            InitializeWindow();
            RegisterEvents();
        }
        private void GenerateGrid()
        {
            SetFullScreen();

            MainGrid = new Grid();
            this.Content = MainGrid;
        }

        public void AddToGrid(UIElement e)
        {
            MainGrid.Children.Add(e);  
        }

        public abstract void InitializeViewModel();

        public abstract void InitializeWindow();

        public virtual void RegisterEvents()
        {
            this.PreviewKeyDown += BaseWindow_PreviewKeyDown;
        }

        private void BaseWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                ToggleFullScreen();
            }
        }

        private void SetFullScreen()
        {
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
        }

        private void UnSetFullscreen()
        {
            this.WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private bool IsFullScreen()
        {
            return this.WindowStyle == WindowStyle.None;
        }

        private void ToggleFullScreen()
        {
            if (!IsFullScreen())
            {
                SetFullScreen();
            }
            else
            {
                UnSetFullscreen();
            }
        }
    }
}