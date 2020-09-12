using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Compiler.HelperClasses;

namespace Compiler.Phases
{
    public class LexicalAnalysis
    {
        protected List<Token> Tokens = new List<Token>();

        public Token[] ProcessTextAndGenerateTokens(string text)
        {
            string word = "";
            char c;
            int lineNo = 1;
            //loop over the entire string
            for (int i = 0; i < text.Length; i++)
            {
                c = text[i];
                //If we got spaces, tab one word is complete generate token of it
                if (c == ' ' || c == '\t')
                {
                    GenerateToken(word, lineNo);
                    word = "";
                }
                //If we get ; its the terminator generate the token plus another token for the terminator
                else if (c == ';')
                {
                    GenerateToken(word, lineNo);
                    word = "";
                    GenerateToken(Constants.TERMINATOR, ";", Constants.TERMINATOR, lineNo);
                }
                //If we got a , generate token and also directrly generate token for it;
                else if (c == ',')
                {
                    GenerateToken(word, lineNo);
                    word = "";
                    GenerateToken(Constants.COMMA, ",", Constants.COMMA, lineNo);
                }
                else if (c == '.')
                {
                    //If its an int before . its means its a number simply pass
                    if (isIntConstant(word))
                    {
                        word += '.';
                    }
                    else
                    {
                        GenerateToken(word, lineNo);
                        i++;
                        GenerateToken(Constants.DOT, ".", Constants.DOT, lineNo);
                        word = "";
                    }
                }
                else if (c == '&')
                {
                    GenerateToken(word, lineNo);
                    word = "";

                    i++;
                    c = text[i];

                    if (c == '&')
                    {
                        i++;
                        GenerateToken(Constants.AND, "&&", Constants.AND, lineNo);
                    }
                    else
                    {
                        i--;
                        GenerateToken(Constants.LOGICAL_AND, lineNo);
                    }
                }
                else if (c == '|')
                {
                    GenerateToken(word, lineNo);
                    word = "";

                    i++;
                    c = text[i];

                    if (c == '|')
                    {
                        i++;
                        GenerateToken(Constants.OR, "||", Constants.OR, lineNo);
                    }
                    else
                    {
                        i--;
                        GenerateToken(Constants.LOGICAL_OR, lineNo);
                    }
                }
                //If we got a ! generate token and also directrly generate token for it;
                else if (c == '!')
                {
                    GenerateToken(word, lineNo);
                    word = "";

                    i++;
                    c = text[i];
                    if (c == '=')
                    {
                        GenerateToken(Constants.RELATIONAL_OPERATION, "!=", Constants.NOT_EQU_TO, lineNo);
                    }
                    else
                    {
                        i--;
                        GenerateToken(Constants.NOT, "!", Constants.NOT, lineNo);
                    }
                }
                else if (c == '=')
                {
                    GenerateToken(word, lineNo);
                    word = "";

                    i++;
                    c = text[i];
                    if (c == '=')
                    {
                        GenerateToken(Constants.RELATIONAL_OPERATION, "==", Constants.EQU_TO_EQU_TO, lineNo);
                    }
                    else
                    {
                        i--;
                        GenerateToken(Constants.EQUAL, "=", Constants.EQUAL, lineNo);
                    }
                }
                //If we get { } ( ) etc generate token and also directly generate token for them
                else if (c == '{' || c == '}' || c == '(' || c == ')' || c == '*')
                {
                    GenerateToken(word, lineNo);
                    word = "";
                    GenerateToken(c.ToString(), lineNo);
                }
                //If its a + we check for next char to confirm if its a ++ or += or not
                else if (c == '+')
                {
                    i++;
                    c = text[i];
                    if (c == '+')
                    {
                        GenerateToken(word, lineNo);
                        word = "";

                        //If the next char is also + generate token for ++
                        GenerateToken("++", lineNo);
                    }
                    else if (c == '=')
                    {
                        GenerateToken(word, lineNo);
                        word = "";

                        //If the next char is = generate token for +=
                        GenerateToken("+=", lineNo);
                    }
                    else
                    {
                        word = word + "+";
                        i--;//reset identification char
                    }
                }
                //If its a + we check for next char to confirm if its a ++ or += or not
                else if (c == '-')
                {
                    i++;
                    c = text[i];
                    if (c == '-')
                    {
                        GenerateToken(word, lineNo);
                        word = "";

                        //If the next char is also + generate token for ++
                        GenerateToken("--", lineNo);
                    }
                    else if (c == '=')
                    {
                        GenerateToken(word, lineNo);
                        word = "";

                        //If the next char is = generate token for +=
                        GenerateToken("-=", lineNo);
                    }
                    else
                    {
                        word = word + "-";
                        i--;//reset identification char
                    }
                }
                //If its a new line
                else if (c == '\r' || c == '\n')
                {
                    GenerateToken(word, lineNo);
                    word = "";
                    //If it is a line break we must skip other character i.e EOL char
                    i++;
                    //Increment line number
                    lineNo++;
                }
                else if (c == '"')
                {
                    GenerateToken(word, lineNo);
                    //If it is an inveted comma, its means its a string we must keep on reading till the next "
                    word = "";
                    do
                    {
                        word += c.ToString();
                        i++;
                        c = text[i];
                        //In case it is a special character in sring like, "Hello \"Ebad\" !";
                        if (c == '\\')
                        {
                            word += c.ToString();       //Put \ in string
                            i++;
                            c = text[i];                //Put special char in string like "
                            word += c.ToString();
                            i++;
                            c = text[i];                //Move to next char
                        }
                    } while (c != '"');
                    word += c.ToString();
                    GenerateToken(Constants.STRING_CONSTANT, word, Constants.STRING_CONSTANT, lineNo);
                    word = "";
                }
                else if (c == '\'')
                {
                    GenerateToken(word, lineNo);

                    word = "'";

                    //if it is a ' it means we are expecting a char we just need to read the next char if its \ its means its a special character

                    i++;
                    c = text[i];
                    word += c.ToString();

                    if (c == '\\') //Its a specioal character
                    {
                        i++;
                        c = text[i];
                        word += c.ToString();

                        i++;
                        c = text[i];
                        word += c.ToString();

                        GenerateToken(Constants.CHAR_CONSTANT, word, Constants.CHAR_CONSTANT, lineNo);
                    }
                    else
                    {
                        i++;
                        c = text[i];
                        word += c.ToString();

                        GenerateToken(Constants.CHAR_CONSTANT, word, Constants.CHAR_CONSTANT, lineNo);
                    }
                    word = "";
                }
                //If we got / it could be divide or a comment 
                else if (c == '/')
                {
                    //Imidiately read the next char
                    i++;
                    c = text[i];
                    //If the next char we read is also a / its means there are two / i.e // its an inline comment skip code till end line
                    if (c == '/')
                    {
                        i++;
                        c = text[i];
                        while (c != '\n' && c != '\r')
                        {
                            i++;
                            c = text[i];
                        }
                        i++;//skip End Of Line char
                    }
                    //If the next char we read is also a * its means we got /* i.e // its a multiline comment skip code till we get */                    else if (c == '*')
                    else if (c == '*')
                    {
                        i++;
                        c = text[i];
                        while (true)
                        {
                            if (c == '*')
                            {
                                i++;
                                c = text[i];
                                if (c == '/')          //We got * and / i.e comment is end
                                {
                                    i++;
                                    break;
                                }
                            }
                            i++;
                            c = text[i];
                        }
                    }
                    //If the next char is not / we generate the token for first / and i-- so that the char we read for identification will continue its execution from the top
                    else
                    {
                        GenerateToken("/", lineNo);//first /
                        i--;//reset identification char
                    }
                }
                else //Keep appending the chars to form word
                {
                    word = word + c;
                }
            }
            GenerateToken(word, lineNo);

            //The generate token funtion add the token to Tokens list we return that list (just making it an array)
            return this.Tokens.ToArray();
        }

        private void GenerateToken(string token, int lineNo)
        {
            //If for any reson we came in this methos with an empty or white space string we simply return
            if (String.IsNullOrWhiteSpace(token))
                return;

            Tuple<string, string> temp = null;
            token = token.Trim();

            //Now we will check the string one by one
            if ((temp = isKeyWord(token)) != null)
            {
                GenerateToken(temp.Item1, token, temp.Item2, lineNo);
            }
            //First check if it is a datatype
            else if ((temp = isDataType(token)) != null)
            {
                GenerateToken(temp.Item1, token, temp.Item2, lineNo);
            }
            //In temp we got the value from isSymbol
            //If its not null its a symbol
            else if ((temp = isSymbol(token)) != null)
            {
                //Our token is a symbol and temp has the value which token it is
                GenerateToken(temp.Item1, token, temp.Item2, lineNo);
            }
            else if (isIntConstant(token))
            {
                //Our token is a int constant
                GenerateToken(Constants.INT_CONSTANT, token, Constants.INT_CONSTANT, lineNo);

            }
            else if (isFloatConstant(token))
            {
                //Our token is a float constant
                GenerateToken(Constants.FLOAT_CONSTANT, token, Constants.FLOAT_CONSTANT, lineNo);

            }
            else if (isBoolConstant(token))
            {
                //Our token is a float constant
                GenerateToken(Constants.BOOL_CONSTANT, token, Constants.BOOL_CONSTANT, lineNo);

            }
            else if (isIdentifier(token))
            {
                //Lets make a rule that we have only
                //small and capital letters in our variable names
                //int abc = 10; is legal
                //int abc2 = 10; is illegal
                GenerateToken(Constants.IDENTIFIER, token, Constants.IDENTIFIER, lineNo);
            }
            else
            {
                Console.WriteLine("TOKEN NOT generated FOR " + token);
            }
        }

        private void GenerateToken(string type, string value, string valueString, int lineNo)
        {
            //Console.WriteLine(tokenType + " ," + value + " ," + lineNo);
            this.Tokens.Add(new Token(type, value, valueString, lineNo));
        }

        //Method to check if token is a keyword
        private Tuple<string, string> isKeyWord(string token)
        {
            switch (token)
            {
                case "class": return new Tuple<string, string>(Constants.CLASS, Constants.CLASS);

                case "public": return new Tuple<string, string>(Constants.ACCESS_MODIFIER, Constants.PUBLIC);
                case "private": return new Tuple<string, string>(Constants.ACCESS_MODIFIER, Constants.PRIVATE);
                case "protected": return new Tuple<string, string>(Constants.ACCESS_MODIFIER, Constants.PROTECTED);

                case "static": return new Tuple<string, string>(Constants.STATIC, Constants.STATIC);

                case "if": return new Tuple<string, string>(Constants.IF, Constants.IF);
                case "else": return new Tuple<string, string>(Constants.ELSE, Constants.ELSE);

                case "do": return new Tuple<string, string>(Constants.DO, Constants.DO);
                case "while": return new Tuple<string, string>(Constants.WHILE, Constants.WHILE);
                case "for": return new Tuple<string, string>(Constants.FOR, Constants.FOR);

                case "void": return new Tuple<string, string>(Constants.VOID, Constants.VOID);

                default: return null;
            }
        }

        //Method to check if token is a datatype
        private Tuple<string, string> isDataType(string token)
        {
            switch (token)
            {
                case "int": return new Tuple<string, string>(Constants.DATA_TYPE, Constants.INT);
                case "string": return new Tuple<string, string>(Constants.DATA_TYPE, Constants.STRING);
                case "float": return new Tuple<string, string>(Constants.DATA_TYPE, Constants.FLOAT);
                case "char": return new Tuple<string, string>(Constants.DATA_TYPE, Constants.CHAR);
                default: return null;
            }
        }

        //Method to check symbols
        private Tuple<string, string> isSymbol(string token)
        {
            switch (token)
            {
                case "=": return new Tuple<string, string>(Constants.EQUAL, Constants.EQUAL);
                case "+": return new Tuple<string, string>(Constants.ADDSUB, Constants.PLUS);
                case "-": return new Tuple<string, string>(Constants.ADDSUB, Constants.MINUS);
                case "*": return new Tuple<string, string>(Constants.DIVMUL, Constants.MULTIPLY);
                case "/": return new Tuple<string, string>(Constants.DIVMUL, Constants.DIVIDE);
                //We will add new symbols when needed

                case "{": return new Tuple<string, string>(Constants.CUR_BRACKET_LEFT, Constants.CUR_BRACKET_LEFT);
                case "}": return new Tuple<string, string>(Constants.CUR_BRACKET_RIGHT, Constants.CUR_BRACKET_RIGHT);
                case "(": return new Tuple<string, string>(Constants.SMALL_BRACKET_LEFT, Constants.SMALL_BRACKET_LEFT);
                case ")": return new Tuple<string, string>(Constants.SMALL_BRACKET_RIGHT, Constants.SMALL_BRACKET_RIGHT);

                case "<": return new Tuple<string, string>(Constants.RELATIONAL_OPERATION, Constants.LESS_THAN);
                case "<=": return new Tuple<string, string>(Constants.RELATIONAL_OPERATION, Constants.LESS_THAN_EQU);
                case ">": return new Tuple<string, string>(Constants.RELATIONAL_OPERATION, Constants.GREATER_THAN);
                case ">=": return new Tuple<string, string>(Constants.RELATIONAL_OPERATION, Constants.GREATER_THAN_EQU);
                case "==": return new Tuple<string, string>(Constants.RELATIONAL_OPERATION, Constants.EQU_TO_EQU_TO);
                case "!=": return new Tuple<string, string>(Constants.RELATIONAL_OPERATION, Constants.NOT_EQU_TO);


                case "++": return new Tuple<string, string>(Constants.INC_DEC, Constants.INC);
                case "--": return new Tuple<string, string>(Constants.INC_DEC, Constants.DEC);

                case "+=": return new Tuple<string, string>(Constants.ASSIGNMENT_OPERATOR, Constants.INC_EQU);
                case "-=": return new Tuple<string, string>(Constants.ASSIGNMENT_OPERATOR, Constants.DEC_EQU);

                //return null if its not a symbol
                default: return null;
            }
        }

        //Method to check if we got an interger constant like 10
        private bool isIntConstant(string token)
        {
            //isInt will be true only if the Parsing was successfull
            //We ignore the converted "value"
            bool isInt = Int32.TryParse(token, out int value);
            return isInt;

        }

        private bool isFloatConstant(string token)
        {
            bool isfloat = float.TryParse(token, out float value);
            return isfloat;
        }

        private bool isBoolConstant(string token)
        {
            bool isBool = bool.TryParse(token, out bool value);
            return isBool;
        }

        private bool isIdentifier(string token)
        {
            //we will see if token has only a-z and A-Z
            //https://stackoverflow.com/questions/1181419/verifying-that-a-string-contains-only-letters-in-c-sharp/1181426
            string identifierRegex = @"^[a-zA-Z]+$";
            bool isIdentifier = Regex.IsMatch(token, identifierRegex);
            return isIdentifier;
        }
    }
}