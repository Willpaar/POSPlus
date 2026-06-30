using System;
using System.Windows;
using System.Windows.Controls;
using OrderPlus.ViewModels;
using Common.Abstract;

namespace OrderPlus.Windows
{
    public class MainWindow : BaseWindow
    {
        public MainViewModel MainViewModel { get; set; }

        private TextBlock WelcomeText { get; set; }

        private TextBlock MenuText { get; set; }

        private Button IdkButton { get; set; }


        public MainWindow()
        {
        }

        public override void InitializeViewModel()
        {
            MainViewModel = (MainViewModel)Activator.CreateInstance(typeof(MainViewModel));
        }

        public override void InitializeWindow()
        {
            WelcomeText = new TextBlock
            {
                Text = "Welcome to OrderPlus!",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 24
            };
            AddToGrid(WelcomeText);

            MenuText = new TextBlock
            {
                Text = MainViewModel.ActiveMenu.Name,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 24
            };
            AddToGrid(MenuText);

            IdkButton = new Button
            {
                Content = "Go to Next Page",
                Width = 150,
                Height = 35,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 20, 20)
            };
            AddToGrid(IdkButton);


        }

        public override void RegisterEvents()
        {
            base.RegisterEvents();

            IdkButton.Click += NavButton_Click;
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow menuWin = new MenuWindow();
            menuWin.Show();
            this.Close();
        }

    }
}