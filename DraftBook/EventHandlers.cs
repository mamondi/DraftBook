using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DraftBook
{
    public partial class MainWindow : Window
    {
        private void dbFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

                SaveButton.Click -= SaveButton_Click;
                CloseReadPanelButton.Click -= CloseReadPanelButton_Click;

                SaveButton.Click += SaveButtonReadPanel_Click;
                CloseReadPanelButton.Click += CloseReadPanelButton_Click;
            }
        }

        private void CloseReadPanelButton_Click(object sender, RoutedEventArgs e)
        {
            ReadPanel.Visibility = Visibility.Collapsed;
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

            var selectedFile = dbFileList.SelectedItem as FileInformation;

            if (selectedFile == null)
            {
                MessageBox.Show("No file selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string oldFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, selectedFile.Theme + ".txt");
            string newFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName + ".txt");

            try
            {
                if (fileName != selectedFile.Theme)
                {
                    File.Move(oldFilePath, newFilePath);
                    selectedFile.Theme = fileName; 
                }

                File.WriteAllText(newFilePath, fileContent);

                FileOperations.UpdateDataGrid(Files);

                dbFileList.SelectedItem = selectedFile;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
