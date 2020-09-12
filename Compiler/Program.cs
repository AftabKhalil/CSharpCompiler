using System;
using System.Linq;

using Compiler.HelperClasses;
using Compiler.Phases;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            //Read file
            MyFileReader myFileReader = new MyFileReader("C://Samples//First.txt");
            string text = myFileReader.ReadFile();

            //Make tokens
            Token[] tokens = new LexicalAnalysis().ProcessTextAndGenerateTokens(text);
            tokens.ToList().ForEach(t => Console.WriteLine(t.ToString()));
            Console.WriteLine("--------------------------------------------------------------------\n\n");

            //Parse syntax analysis phase
            SyntaxAnalysis syntaxAnalysis = new SyntaxAnalysis(tokens);
            SyntaxAnalysisResult syntaxAnalysisResult = syntaxAnalysis.Parse();
            if (!syntaxAnalysisResult.SyntaxParsed)
            {
                Console.WriteLine(syntaxAnalysisResult.Exception.Message);
            }



            Console.Read();
        }
    }
}