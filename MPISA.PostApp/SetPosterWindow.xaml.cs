using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MPISA.PostApp
{
    public partial class SetPosterWindow : Window
    {
        public string PosterFilename { get; set; }

        public SetPosterWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetPosterImage();
        }

        private void SetPosterImage()
        {
            if (!String.IsNullOrEmpty(PosterFilename))
            {
                imagePoster.Source = new BitmapImage(new Uri(PosterFilename));
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files|*.jpeg;*.jpg;*.png",
                Title = "Plakatas"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                PosterFilename = openFileDialog.FileName;
                SetPosterImage();
            }
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
