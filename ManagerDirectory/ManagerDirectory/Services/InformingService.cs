﻿using System.IO;
using System.Linq;
using ManagerDirectory.Enums;

namespace ManagerDirectory.Services
{
    public class InformingService
    {
        private string _fullPathFile;
        public string FullPathFile
        {
            get => _fullPathFile;
            set => _fullPathFile = value;
        }

        private string _fullPathDirectory;
        public string FullPathDirectory
        {
            get => _fullPathDirectory;
            set => _fullPathDirectory = value;
        }

        public override string ToString()
        {
	        if (!string.IsNullOrEmpty(_fullPathDirectory) && Path.GetExtension(_fullPathDirectory) == string.Empty)
            {
                var directoryInfo = new DirectoryInfo(_fullPathDirectory);
                var countDirectory = directoryInfo.EnumerateDirectories("*", SearchOption.AllDirectories).Count();
                var countFiles = directoryInfo.EnumerateFiles("*.*", SearchOption.AllDirectories).Count();
                long size = 0;

                var files = directoryInfo.EnumerateFiles("*.*", SearchOption.AllDirectories)
                    .ToList();

                foreach (var file in files) 
	                size += file.Length;

                return $"Количество папок: {countDirectory}\n" +
                       $"Количество файлов: {countFiles}\n" +
                       $"Размер: {ConvertAsync(size)}";
            }

	        var fileInfo = new FileInfo(_fullPathFile);

	        return $"Имя: {Path.GetFileNameWithoutExtension(_fullPathFile)}\n" +
	               $"Расширение: {fileInfo.Extension}\n" +
	               $"Размер: {ConvertAsync(fileInfo.Length)}";
        }

        private string ConvertAsync(long size)
        {
            return size switch
            {
                < 1024 => $"{size.ToString()} {Value.B.ToString()}",
                > 1024 and < 1_048_576 => $"{(double)size / 1024:F} {Value.KB.ToString()}",
                > 1_048_576 and < 1_073_741_824 => $"{(double)size / 1_048_576:F} {Value.MB.ToString()}",
                > 1_073_741_824 => $"{(double)size / 1_073_741_824:F} {Value.GB.ToString()}",
                _ => "0"
            };
        }
    }
}
