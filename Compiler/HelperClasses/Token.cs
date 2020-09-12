using System;

namespace Compiler.HelperClasses
{
    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string ValueString { get; set; }
        public int LineNumber { get; set; }

        public Token(string Type, string Value, string ValueString, int LineNumber)
        {
            this.Type = Type;
            this.Value = Value;
            this.ValueString = ValueString;
            this.LineNumber = LineNumber;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", this.Type, this.Value, this.ValueString, this.LineNumber);
        }
    }
}