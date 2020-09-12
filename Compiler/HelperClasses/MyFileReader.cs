using System;
using System.IO;

namespace Compiler.HelperClasses
{
    //Class to read a test file
    public class MyFileReader
    {
        private string FileName;

        //Constructor
        public MyFileReader(string FileName)
        {
            this.FileName = FileName;
        }

        //Function to read and return the entire content of the file in a single string
        public string ReadFile()
        {
            string text = File.ReadAllText("..//..//..//Samples//" + this.FileName);
            return text;
        }
    }
}