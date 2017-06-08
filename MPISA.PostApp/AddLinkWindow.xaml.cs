using System;
using System.Windows;
using System.Windows.Input;

namespace MPISA.PostApp
{
    public partial class AddLinkWindow : Window
    {
        public string Text { get; set; }
        public string Link { get; set; }

        public AddLinkWindow()
        {
            InitializeComponent();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveClick();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelClick();
        }

        private void textBoxText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Text = textBoxText.Text;
        }

        private void textBoxLink_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Link = textBoxLink.Text;
        }

        private void textBoxText_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SaveClick();
            }
        }

        private void textBoxLink_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SaveClick();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxText.Focus();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void FixLink()
        {
            if (!Link.StartsWith("http://")
                || !Link.StartsWith("https://"))
            {
                Link = "http://" + Link;
            }

            textBoxLink.Text = Link;
        }

        private void SaveClick()
        {
            if (String.IsNullOrEmpty(Text)
                || String.IsNullOrEmpty(Link))
            {
                MessageBox.Show("Nėra nuorodos arba teksto!", "Sutvarkyk Reikalus", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            FixLink();
            DialogResult = true;
            Close();
        }

        private void CancelClick()
        {
            DialogResult = false;
            Close();
        }
    }
}
