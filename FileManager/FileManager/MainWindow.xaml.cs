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
        string mainDrive = Environment.SystemDirectory.Substring(0, 3);

        public MainWindow()
        {
            InitializeComponent();
            LoadFiles(mainDrive);
            PathText.Text = mainDrive;
        }

        private void LoadFiles(string path)
        {
            if (Directory.Exists(path))
            {
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
            }
        }

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PathText.Text = Path.Combine(PathText.Text, e.AddedItems[0].ToString() + "\\");
            FileList.SelectionChanged -= FileList_SelectionChanged;
            LoadFiles(PathText.Text);
            FileList.SelectionChanged += FileList_SelectionChanged;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if(PathText.Text != mainDrive)
            {
                PathText.Text = PathText.Text.Substring(0, PathText.Text.LastIndexOf("\\"));
                PathText.Text = PathText.Text.Substring(0, PathText.Text.LastIndexOf("\\") + 1);
                LoadFiles(PathText.Text);
            } else
            {
                MessageBox.Show("Can´t go further back");
            }
        }
    }
}
