﻿using System.IO;
using System.Threading.Tasks;
using ManagerDirectory.Actions;

namespace ManagerDirectory.Validation
{
    public sealed class CustomValidation
    {
        private readonly Commands _commands = new();

		internal async Task<bool> CheckForInputAsync(string nameCommand)
        {
            return await Task.Run(() =>
            {
                foreach (var command in _commands.ArrayCommands)
                {
                    if ((command == nameCommand.Split(" ")[0] && nameCommand != string.Empty) || nameCommand.Contains(':'))
                        return true;
                }

                return false;
			});
        }

		internal async Task<string> CheckEnteredPathAsync(string path, string defaultPath)
        {
            return await Task.Run(() =>
            {
                if (Directory.Exists(defaultPath + path))
                    return defaultPath + path;

                if ((Directory.Exists(path) || File.Exists(path)) && path.Contains('\\'))
                    return path;

                return defaultPath;
			});
        }
	}
}
