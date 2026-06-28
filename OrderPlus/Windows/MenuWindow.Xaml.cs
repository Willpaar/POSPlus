using System;
using System.Windows;
using System.Windows.Controls;
using Menu = Common.Menu.Menu;

namespace OrderPlus.Windows
{
    public class MenuWindow : BaseWindow
    {
        private TextBlock WelcomeText { get; set; }

        private Menu TestMenu { get; set; }

        private TextBlock MenuText { get; set; }


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

            TestMenu = new Menu();
            TestMenu.Load();

            MenuText = new TextBlock
            {
                Text = TestMenu.Name,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 24
            };
            AddToGrid(MenuText);

        }
    }
}
