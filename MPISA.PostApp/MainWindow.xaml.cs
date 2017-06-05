using Microsoft.Win32;
using MPISA.PostApp.Enums;
using MPISA.PostApp.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MPISA.PostApp
{
    public partial class MainWindow : Window
    {
        private PostType _postType = PostType.Post;
        private DoubleAnimation _fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.Parse("0:0:0.4"));
        private DoubleAnimation _fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.Parse("0:0:0.4"));

        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _poster = string.Empty;
        private List<string> _photos = new List<string>();
        private List<Link> _links = new List<Link>();
        private List<string> _videos = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetPostType(labelPost, PostType.Post);
        }

        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsPostValid())
                {
                    return;
                }

                var post = new object();

                if (_postType == PostType.Post)
                {
                    post = new Post
                    {
                        Text = _description,
                        Title = _title,
                        Links = _links,
                        Embeds = _videos
                    };
                }
                else
                {
                    post = new PhotoAlbum
                    {
                        Text = _description,
                        Title = _title
                    };
                }

                var json = JsonConvert.SerializeObject(post, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                var folderBrowseDialog = new FolderBrowserDialog
                {
                    ShowNewFolderButton = true
                };
                var result = folderBrowseDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    var selectedFolder = folderBrowseDialog.SelectedPath;
                    var postFolderName = DateTime.Now.ToString("yyyy-MM-dd hhmmss");
                    var postPath = string.Empty;

                    if (_postType == PostType.PhotoAlbum)
                    {
                        postPath = System.IO.Path.Combine(selectedFolder, "photo_" + postFolderName);
                    }
                    else
                    {
                        postPath = System.IO.Path.Combine(selectedFolder, "post_" + postFolderName);
                    }

                    if (Directory.Exists(postPath))
                    {
                        System.Windows.MessageBox.Show("Pasirink kitą vietą!", "Opa!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    Directory.CreateDirectory(postPath);

                    var infoJsonPath = System.IO.Path.Combine(postPath, "info.json");
                    File.WriteAllText(infoJsonPath, json);

                    var posterPath = System.IO.Path.Combine(postPath, "poster.png");
                    File.Copy(_poster, posterPath);

                    if (_postType == PostType.PhotoAlbum)
                    {
                        var photoPath = System.IO.Path.Combine(postPath, "photos");
                        Directory.CreateDirectory(photoPath);

                        var photoCount = 1;
                        foreach (var photo in _photos)
                        {
                            var currentPhotoPath = System.IO.Path.Combine(photoPath, photoCount + ".png");
                            File.Copy(photo, currentPhotoPath);
                            photoCount++;
                        }
                    }

                    System.Windows.MessageBox.Show("Išsaugota!", "Valio!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Perduok Stasiui: {ex.Message}\n{ex.StackTrace}", "Kažkas nepavyko", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonAddLink_Click(object sender, RoutedEventArgs e)
        {
            var addLinkWindow = new AddLinkWindow();
            var dialogResult = addLinkWindow.ShowDialog();
            if (dialogResult == true)
            {
                var link = new Link
                {
                    Text = addLinkWindow.Text,
                    Url = addLinkWindow.Link
                };

                _links.Add(link);
                RefreshLinkList();
            }
        }

        private void buttonAddVideo_Click(object sender, RoutedEventArgs e)
        {
            var addVideoWindow = new AddVideoWindow();
            var dialogResult = addVideoWindow.ShowDialog();
            if (dialogResult == true)
            {
                _videos.Add(addVideoWindow.VideoLink);
                RefreshVideoList();
            }
        }

        private void buttonAddPhoto_Click(object sender, RoutedEventArgs e)
        {
            var openFilesDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Title = "Nuotraukos",
                Filter = "Image files|*.jpeg;*.jpg;*.png",
            };
            var result = openFilesDialog.ShowDialog();
            if (result == true)
            {
                _photos.AddRange(openFilesDialog.FileNames.ToList());
                RefreshPhotoList();
            }
        }

        private void buttonPoster_Click(object sender, RoutedEventArgs e)
        {
            var setPosterWindow = new SetPosterWindow
            {
                PosterFilename = _poster
            };
            setPosterWindow.ShowDialog();
            _poster = setPosterWindow.PosterFilename;
        }

        private void labelPost_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetPostType(labelPost, PostType.Post);
        }

        private void labelPhotos_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetPostType(labelPhotos, PostType.PhotoAlbum);
        }

        private void textBoxTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            _title = textBoxTitle.Text;
        }

        private void richTextBoxDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            _description = new TextRange(richTextBoxDescription.Document.ContentStart, richTextBoxDescription.Document.ContentEnd).Text;
        }

        private void listboxLinks_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var index = listboxLinks.SelectedIndex;
                if (index == -1)
                {
                    return;
                }

                _links.RemoveAt(index);
                RefreshLinkList();
            }
        }

        private void listboxVideos_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var index = listboxVideos.SelectedIndex;
                if (index == -1)
                {
                    return;
                }

                _videos.RemoveAt(index);
                RefreshVideoList();
            }
        }

        private void listboxPhotos_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var index = listboxPhotos.SelectedIndex;
                if (index == -1)
                {
                    return;
                }

                _photos.RemoveAt(index);
                RefreshPhotoList();
            }
        }

        private void SetPostType(System.Windows.Controls.Label label, PostType postType)
        {
            if (postType == _postType)
            {
                return;
            }

            labelPhotos.BorderThickness = new Thickness(0, 0, 0, 0);
            labelPost.BorderThickness = new Thickness(0, 0, 0, 0);

            label.BorderThickness = new Thickness(0, 0, 0, 2);
            _postType = postType;
            ToggleControls();
        }

        private void ToggleControls()
        {
            var postControls = new System.Windows.Controls.Control[] {
                labelLinks,
                labelVideos,
                listboxLinks,
                listboxVideos,
                buttonAddLink,
                buttonAddVideo
            };

            var photoControls = new System.Windows.Controls.Control[]
            {
                labelPhotosAdd,
                labelPhotosCount,
                buttonAddPhoto,
                listboxPhotos
            };

            var fadeInControls = postControls;
            var fadeOutControls = photoControls;

            if (_postType == PostType.PhotoAlbum)
            {
                fadeInControls = photoControls;
                fadeOutControls = postControls;
            }

            foreach (var c in fadeOutControls)
            {
                c.BeginAnimation(OpacityProperty, _fadeOutAnimation);
                c.Visibility = Visibility.Hidden;
            }

            foreach (var c in fadeInControls)
            {
                c.BeginAnimation(OpacityProperty, _fadeInAnimation);
                c.Visibility = Visibility.Visible;
            }
        }

        private void RefreshLinkList()
        {
            listboxLinks.Items.Clear();
            foreach (var l in _links)
            {
                listboxLinks.Items.Add($"{l.Text} ({l.Url})");
            }
        }

        private void RefreshVideoList()
        {
            listboxVideos.Items.Clear();
            foreach (var v in _videos)
            {
                listboxVideos.Items.Add(v);
            }
        }

        private void RefreshPhotoList()
        {
            listboxPhotos.Items.Clear();
            foreach (var photo in _photos)
            {
                listboxPhotos.Items.Add(photo);
            }

            labelPhotosCount.Content = _photos.Count;
        }

        private bool IsPostValid()
        {
            var message = string.Empty;

            if (String.IsNullOrEmpty(_poster))
            {
                message = "Nėra plakato!";
            }

            if (_postType == PostType.PhotoAlbum
                && _photos.Count == 0)
            {
                message = "Nėra nuotraukų, nors jų turėtų būti!";
            }

            if (String.IsNullOrEmpty(_title))
            {
                message = "Nėra pavadinimo!";
            }

            if (!String.IsNullOrEmpty(message))
            {
                System.Windows.MessageBox.Show(message, "Opa!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
