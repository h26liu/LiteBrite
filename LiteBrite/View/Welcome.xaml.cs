/*!	\author		Haohan Liu
	\date		2019-04-20
	\file		Welcome.xaml.cs
*/

using System.Windows;

namespace LiteBrite.View
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        public Welcome()
        {
            InitializeComponent();

            // center the window
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();

            main.Show();
            this.Close();
        }
    }
}
