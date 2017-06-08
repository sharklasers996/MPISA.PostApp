using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace MPISA.PostApp
{
    public partial class AddVideoWindow : Window
    {
        public string VideoLink { get; set; }

        public AddVideoWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveClick();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void textBoxVideoLink_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                SaveClick();
            }
        }

        private bool IsVideoLinkValid()
        {
            var m = Regex.Match(textBoxVideoLink.Text, @"(watch\?v=|embed/)(?<id>[^&\?]+)");
            if (m.Success)
            {
                VideoLink = "https://www.youtube.com/embed/" + m.Groups["id"].Value;
                return true;
            }

            MessageBox.Show("Neteisingas formatas", "Susitvarkyk reikalus", MessageBoxButton.OK, MessageBoxImage.Warning);

            return false;
        }

        private void SaveClick()
        {
            if (IsVideoLinkValid())
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
