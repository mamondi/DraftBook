using System;

namespace DraftBook
{
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
}
