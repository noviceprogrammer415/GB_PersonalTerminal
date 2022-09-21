﻿using System;
using System.IO;
using System.Threading.Tasks;
using ManagerDirectory.ConsoleView;
using ManagerDirectory.Infrastructure.Models;
using ManagerDirectory.Infrastructure.Repositories;
using ManagerDirectory.Properties;
using ManagerDirectory.Validation;

namespace ManagerDirectory.Services
{
    internal sealed class ManagerService
    {
        private (string command, Uri path) _entry;
        private Uri _defaultPath = new(Resources.DefaultPath);
        private readonly string _fileName = Resources.CurrentPath;
        private readonly string _fileLogErrors = Resources.FileLogErrors;

        private readonly Displaying _displaying = new();
        private readonly Repository _repository = new();
        private CurrentPath _currentPath = new();
        private readonly InformingService _informer = new();
        private readonly CustomValidation _validation = new();

        public async Task StartAsync()
        {
            await Task.Run(async () =>
            {
                if(File.Exists(_fileName))
                    _currentPath = await _repository.GetPath(_fileName, _currentPath, _defaultPath);
                else
                {
					File.Create(_fileName).Close();
                    _currentPath.Path = _defaultPath.OriginalString;
				}

                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (_currentPath.Path.Length > drive.Name.Length)
                    {
                        if (drive.Name.Equals(_currentPath.Path.Substring(0, 3)))
                            return;
                    }
                    else
                    {
                        if (drive.Name.Equals(_currentPath.Path.Substring(0, _currentPath.Path.Length)))
                            return;
                    }
                }

                _currentPath.Path = _defaultPath.OriginalString;
            });
        }

        public async Task RunAsync()
        {
            if (File.Exists(_fileName) && !string.IsNullOrEmpty(_currentPath.Path))
                _defaultPath = new Uri(_currentPath.Path);

            var receiver = new Receiver();
            _entry = await receiver.ReceiveAsync(_defaultPath, _validation);

            await SwitchCommandAsync();
        }

        private async Task SwitchCommandAsync()
        {
            try
            {
                if (_entry.command.Contains(':'))
                    _defaultPath = new Uri(Path.Combine(_entry.command, "\\"));

                var path = _entry.path;
                var newPath = string.Empty;

                switch (_entry.command)
                {
                    case "disk":
                        await CallOutputAsync();
                        break;
                    case "ls":
                        path = await _validation.CheckEnteredPathAsync(_entry.path, _defaultPath);
                        await CallOutputAsync(path, 10);
                        break;
                    case "lsAll":
                        path = await _validation.CheckEnteredPathAsync(_entry.path, _defaultPath);
                        await CallOutputAsync(path, Directory.GetDirectories(path.OriginalString).Length + Directory.GetFiles(path.OriginalString).Length);
                        break;
                    case "cp":
                        //path = await TransformAsync(_entry.Remove(0, _entry.command.Length + 1));
                        //path = path.TrimEnd();
                        //newPath = _entry.Remove(0, _entry.command.Length + path.Length + 2) + "\\";
                        //await CallCopyingAsync(path, newPath);
                        break;
                    case "clear": Console.Clear(); break;
                    case "cd":
                        path = new Uri(Path.Combine(_entry.path.OriginalString, "\\"));
                        _defaultPath = await _validation.CheckEnteredPathAsync(path, _defaultPath);
                        break;
                    case "cd..":
                        path = new Uri(_defaultPath.OriginalString.Remove(_defaultPath.OriginalString.Length - 1, 1));
                        _defaultPath = new Uri(Directory.GetParent(path.OriginalString)!.FullName);
                        break;
                    case "cd\\":
                        _defaultPath = new Uri(Directory.GetDirectoryRoot(_defaultPath.OriginalString));
                        break;
                    case "info":
                        await CallInformerAsync(_entry.command);
                        await _displaying.OutputInfoFilesAndDirectoryAsync(_informer);
                        break;
                    case "help":
                        Console.WriteLine(await _repository.GetHelp());
                        break;
                    case "rm": await CallDeletionAsync(); break;
                }

                if (_entry.command != "exit")
                {
                    _currentPath.Path = string.Empty;
                    await RunAsync();
                }
                else
                {
                    _currentPath.Path = _defaultPath.OriginalString;
                    await _repository.CreatePath(_currentPath, _fileName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await File.AppendAllTextAsync(_fileLogErrors, $"{DateTime.Now:G} {e.Message} {e.TargetSite}");
                await File.AppendAllTextAsync(_fileLogErrors, Environment.NewLine);
                await RunAsync();
            }
        }

        private async Task CallOutputAsync()
            => await _displaying.GetDisksAsync();

        private async Task CallOutputAsync(Uri path, int maxObjects)
            => await _displaying.ViewTreeAsync(path, maxObjects);

        private async Task CallCopyingAsync(string name, Uri newPath)
        {
            var copying = new CopyingService();
            await copying.CopyAsync(_defaultPath, name, newPath);
        }

        private async Task CallDeletionAsync()
        {
            var entry = await _validation.CheckEnteredPathAsync(_entry.path, _defaultPath);

            var deletion = new RemovingService();

            if (!string.IsNullOrEmpty(Path.GetExtension(entry.OriginalString)))
                deletion.FullPathFile = entry;
            else
                deletion.FullPathDirectory = entry;
        }

        private async Task CallInformerAsync(string command)
        {
            Uri path;

            if (_entry.command.Length == command.Length)
                path = _defaultPath;
            else
                path = await _validation.CheckEnteredPathAsync(_entry.path, _defaultPath);

            if (!string.IsNullOrEmpty(Path.GetExtension(path.OriginalString)))
            {
                _informer.FullPathFile = path;
                _informer.FullPathDirectory = null;
            }
            else
            {
                _informer.FullPathDirectory = path;
                _informer.FullPathFile = null;
            }
        }
    }
}