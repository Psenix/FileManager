using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace FileManager
{

    public partial class MainWindow : Window
    {
        List<string> fileList = new List<string>();
        List<string> favoriteList = new List<string>();
        readonly string mainDrive = Environment.SystemDirectory.Substring(0, 3);
        readonly string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\FileManager\\";
        string item = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            Directory.CreateDirectory(folderPath);
            GetFavorite();
            LoadFiles(mainDrive);
            PathText.Text = mainDrive;
            DriveList.ItemsSource = DriveInfo.GetDrives().ToList();
        }

        private void LoadFiles(string path)
        {
            if (Directory.Exists(path))
            {
                RefreshFavoriteSymbol();
                fileList = Directory.GetFiles(path).ToList();
                fileList.AddRange(Directory.GetDirectories(path).ToList());
                for (int i = 0; i < fileList.Count; i++)
                {
                    fileList[i] = fileList[i].Remove(0, path.Length);
                }
                FileList.ItemsSource = fileList;
                FileList.Items.Refresh();
            }
            else
            {
                try
                {
                    Process.Start(path);
                }
                catch
                {
                    MessageBox.Show("File could not be executed");
                }
                PathText.Text = path.Remove(path.Length - item.Length, item.Length);

            }
        }

        private void RefreshFavoriteSymbol()
        {
            if (File.ReadAllLines(folderPath + "\\Favorites").Contains(PathText.Text))
            {
                FavoriteBtn.Content = "♥";
            }
            else
            {
                FavoriteBtn.Content = "♡";
            }
        }

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            item = e.AddedItems[0].ToString() + "\\";
            PathText.Text = Path.Combine(PathText.Text, item);
            DriveList.SelectionChanged -= DriveList_SelectionChanged;
            DriveList.SelectedItem = null;
            DriveList.SelectionChanged += DriveList_SelectionChanged;
            FileList.SelectionChanged -= FileList_SelectionChanged;
            LoadFiles(PathText.Text);
            FileList.SelectionChanged += FileList_SelectionChanged;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PathText.Text != mainDrive)
            {
                PathText.Text = PathText.Text.Substring(0, PathText.Text.LastIndexOf("\\"));
                PathText.Text = PathText.Text.Substring(0, PathText.Text.LastIndexOf("\\") + 1);
                LoadFiles(PathText.Text);
                FileList.SelectionChanged -= FileList_SelectionChanged;
                FileList.SelectedItem = null;
                FileList.SelectionChanged += FileList_SelectionChanged;
            }
            else
            {
                MessageBox.Show("Can´t go further back");
            }
        }

        private void DriveList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PathText.Text = e.AddedItems[0].ToString();
            LoadFiles(PathText.Text);
        }

        private void FavoriteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = FavoriteList.SelectedIndex;
            var favorites = File.ReadAllLines(folderPath + "Favorites");
            PathText.Text = favorites[i];
            FileList.SelectionChanged -= FileList_SelectionChanged;
            LoadFiles(PathText.Text);
            FileList.SelectionChanged += FileList_SelectionChanged;
        }

        private void FavoriteBtn_Click(object sender, RoutedEventArgs e)
        {
            var favorites = File.ReadAllLines(folderPath + "Favorites").ToList();
            if (favorites.Contains(PathText.Text))
            {
                for (int i = 0; i < favorites.Count; i++)
                {
                    if (favorites[i] == PathText.Text)
                    {
                        favorites.RemoveAt(i);
                    }
                }
                File.WriteAllLines(folderPath + "Favorites", favorites);
            }
            else
            {
                File.AppendAllText(folderPath + "Favorites", PathText.Text + Environment.NewLine);
            }
            FavoriteList.SelectionChanged -= FavoriteList_SelectionChanged;
            GetFavorite();
            FavoriteList.SelectionChanged += FavoriteList_SelectionChanged;
            RefreshFavoriteSymbol();
        }


        private void GetFavorite()
        {
            if (File.Exists(folderPath + "Favorites"))
            {

                favoriteList = File.ReadAllLines(folderPath + "Favorites").ToList();
                for (int i = 0; i < favoriteList.Count; i++)
                {
                    favoriteList[i] = favoriteList[i].Remove(favoriteList[i].Length - 1, 1);
                    favoriteList[i] = favoriteList[i].Substring(favoriteList[i].LastIndexOf("\\") + 1, favoriteList[i].Length - favoriteList[i].LastIndexOf("\\") - 1);
                }
                FavoriteList.ItemsSource = favoriteList;
                FavoriteList.Items.Refresh();
            }
        }

        private void FavoriteList_LostFocus(object sender, RoutedEventArgs e)
        {
            FavoriteList.SelectionChanged -= FavoriteList_SelectionChanged;
            FavoriteList.SelectedItem = null;
            FavoriteList.SelectionChanged += FavoriteList_SelectionChanged;
        }
    }
}
