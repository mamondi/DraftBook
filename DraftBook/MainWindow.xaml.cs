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
            FileOperations.DisplayFilesInfo(Files);
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
                // Перевіряємо, чи вказано ім'я файлу
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("No file selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
