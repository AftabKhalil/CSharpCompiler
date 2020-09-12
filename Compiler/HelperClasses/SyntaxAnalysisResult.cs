using System;

namespace Compiler.HelperClasses
{
    public class SyntaxAnalysisResult
    {
        public bool SyntaxParsed { get; set; }
        public Exception Exception { get; set; }
    }
}