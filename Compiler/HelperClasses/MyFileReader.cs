﻿using System;
using System.IO;

namespace Compiler.HelperClasses
{
    //Class to read a test file
    public class MyFileReader
    {
        private string path;

        //Constructor
        public MyFileReader(string path)
        {
            this.path = path;
        }

        //Function to read and return the entire content of the file in a single string
        public string ReadFile()
        {
            string text = File.ReadAllText(this.path);
            return text;
        }
    }
}