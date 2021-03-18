﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerDirectory
{
    public class Manager : Objects
    {
	    private string _empty;
	    private string _defaultPath = "c:\\";

	    public void Run()
	    {
		    _empty = _input.Input(_defaultPath);

			ToDistribute();
	    }

	    private void ToDistribute()
	    {
		    try
		    {
			    string command = _empty.Split(" ")[0];
			    string path = string.Empty;
			    string newPath = string.Empty;

			    switch (command)
			    {
				    case "ls":
					    path = _empty.Length == 2 ? _defaultPath : _empty.Split(" ")[1];
						CallOutput(path);
					    break;
				    case "cp":
						path = _empty.Split(" ")[1];
						newPath = _empty.Split(" ")[2];
						CallCopying(path, newPath);
					    break;
					case "clear":
						Console.Clear();
						break;
					case "cd":
						Move();
						break;
					case "info":
						break;
			    }

				if(command != "exit")
					Run();
		    }
		    catch
		    {
			    Run();
		    }
	    }

		private void CallOutput(string path)
		{
			_output.OutputTree(path);
		}

		private void CallCopying(string path, string newPath)
		{
			_copying.Copy(path, newPath);
		}

		private void Move()
		{
			_moving.Move();
		}
    }
}