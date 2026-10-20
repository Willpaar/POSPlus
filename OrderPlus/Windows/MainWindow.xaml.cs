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

        private Button LoginButton { get; set; }


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
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 24
            };
            AddToGrid(WelcomeText);


            LoginButton = new Button
            {
                Content = "Login",
                Width = 150,
                Height = 35,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 20, 20)
            };
            AddToGrid(LoginButton);


        }

        public override void RegisterEvents()
        {
            base.RegisterEvents();

            LoginButton.Click += Loginbtn_clk;
        }

        private void Loginbtn_clk(object sender, RoutedEventArgs e)
        {
            OpenNewWindow<MenuWindow>();
        }

    }
}