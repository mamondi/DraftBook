using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace DraftBook
{
    public static class FileOperations
    {
        public static void DisplayFilesInfo(ObservableCollection<FileInformation> Files)
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

        public static void UpdateDataGrid(ObservableCollection<FileInformation> Files)
        {
            Files.Clear();
            DisplayFilesInfo(Files);
        }
    }
}
