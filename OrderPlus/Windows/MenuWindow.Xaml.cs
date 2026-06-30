using System;
using System.Windows;
using System.Windows.Controls;
using OrderPlus.ViewModels;

namespace OrderPlus.Windows
{
    public class MenuWindow : BaseWindow
    {
        public MenuViewModel MenuViewModel { get; set; }

        private TextBlock WelcomeText { get; set; }

        public override void InitializeViewModel()
        {
            MenuViewModel = (MenuViewModel)Activator.CreateInstance(typeof(MenuViewModel));
        }

        public override void InitializeWindow()
        {
            WelcomeText = new TextBlock
            {
                Text = "This is the menu",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 24
            };
            AddToGrid(WelcomeText);

        }
    }
}
