using System;
using System.Windows;
using System.Windows.Controls;

namespace OrderPlus.Windows
{
    public class MainWindow : BaseWindow
    {

        private TextBlock WelcomeText { get; set; }

        private Button IdkButton { get; set; }

        public MainWindow()
        {
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