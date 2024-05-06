using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DraftBook
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<FileInformation>? _files;

        public ObservableCollection<FileInformation> Files
        {
            get { return _files; }
            set
            {
                _files = value;
                OnPropertyChanged("Files");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Files = new ObservableCollection<FileInformation>();
            DisplayFilesInfo();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = FileNameTextBox.Text.Trim();
            string fileContent = FileContentTextBox.Text;

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Please enter a file name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (fileName.Length > 10)
            {
                MessageBox.Show("File name should not exceed 10 characters.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(projectFolder, fileName + ".txt");

            try
            {
                // Перевіряємо, чи існує файл з таким ім'ям
                if (File.Exists(filePath))
                {
                    MessageBox.Show("File already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Створюємо новий файл і записуємо вміст
                File.WriteAllText(filePath, fileContent);

                // Додаємо інформацію про файл до колекції для відображення у DataGrid
                Files.Add(new FileInformation(fileName, fileContent.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length, DateTime.Now));

                MessageBox.Show("File created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HidePanel_Click(object sender, RoutedEventArgs e)
        {
            RectanglePanel.Visibility = Visibility.Collapsed;
            ContentPanel.Visibility = Visibility.Collapsed;
            HidePanelButton.Visibility = Visibility.Collapsed;
            ContentPanelBorder.Visibility = Visibility.Collapsed;
        }

        private void CreateFileButton_Click(object sender, RoutedEventArgs e)
        {
            RectanglePanel.Visibility = Visibility.Visible;
            HidePanelButton.Visibility = Visibility.Visible;
            ContentPanelBorder.Visibility = Visibility.Visible;
            ContentPanel.Visibility = Visibility.Visible;
            FileNameTextBox.Visibility = Visibility.Visible;
            FileContentTextBox.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Visible;
        }

        public class FileInformation
        {
            public string Theme { get; set; }
            public int Words { get; set; }
            public DateTime Date { get; set; }

            public FileInformation(string theme, int words, DateTime date)
            {
                Theme = theme;
                Words = words;
                Date = date;
            }
        }

        private void DisplayFilesInfo()
        {
            string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
            string[] files = Directory.GetFiles(projectFolder, "*.txt");

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string fileContent = File.ReadAllText(file);
                Files.Add(new FileInformation(fileName, fileContent.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length, File.GetCreationTime(file)));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void dbFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CreateReadPanel(FileInformation selectedFile)
        {
            Grid readPanel = new Grid();
            readPanel.Background = Brushes.Transparent;

            TextBox fileContentTextBox = new TextBox();
            fileContentTextBox.Text = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, selectedFile.Theme + ".txt"));
            fileContentTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            fileContentTextBox.AcceptsReturn = true;
            fileContentTextBox.Background = Brushes.Transparent;
            fileContentTextBox.IsReadOnly = true;
            Grid.SetRow(fileContentTextBox, 0);

            Button closeButton = new Button();
            closeButton.Content = "Close";
            closeButton.Click += (s, e) => { ContentGrid.Children.Remove(readPanel); };
            Grid.SetRow(closeButton, 1);

            readPanel.Children.Add(fileContentTextBox);
            readPanel.Children.Add(closeButton);

            Grid.SetRow(readPanel, 1);
            ContentGrid.Children.Add(readPanel);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFile = dbFileList.SelectedItem as FileInformation;

            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, selectedFile.Theme + ".txt");
                File.Delete(filePath);
                Files.Remove(selectedFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFile = (sender as Button)?.DataContext as FileInformation;

            if (selectedFile != null)
            {
                ReadFileNameTextBox.Text = selectedFile.Theme;
                ReadFileContentTextBox.Text = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, selectedFile.Theme + ".txt"));
                ReadPanel.Visibility = Visibility.Visible;

                if (SaveButton != null)
                {
                    SaveButton.Click -= SaveButton_Click;
                    SaveButton.Click += SaveButtonReadPanel_Click;
                }

                if (CloseReadPanelButton != null)
                {
                    CloseReadPanelButton.Click -= CloseReadPanelButton_Click;
                    CloseReadPanelButton.Click += CloseReadPanelButton_Click;
                }
            }
        }

        private void SaveButtonReadPanel_Click(object sender, RoutedEventArgs e)
        {
            string fileName = ReadFileNameTextBox.Text.Trim();
            string fileContent = ReadFileContentTextBox.Text;

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Please enter a file name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (fileName.Length > 10)
            {
                MessageBox.Show("File name should not exceed 10 characters.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName + ".txt");

            try
            {
                // Перевіряємо, чи існує файл
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("File not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Зберігаємо зміни у файлі
                File.WriteAllText(filePath, fileContent);
                MessageBox.Show("File saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Оновлюємо дані у DataGrid
                UpdateDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateDataGrid()
        {
            // Очищаємо дані у DataGrid
            Files.Clear();

            // Оновлюємо дані у DataGrid
            DisplayFilesInfo();
        }




        private void CloseReadPanel_Click(object sender, RoutedEventArgs e)
        {
            ReadPanel.Visibility = Visibility.Collapsed;
        }

        private void CloseReadPanelButton_Click(object sender, RoutedEventArgs e)
        {
            ReadPanel.Visibility = Visibility.Collapsed;
        }
    }
}
