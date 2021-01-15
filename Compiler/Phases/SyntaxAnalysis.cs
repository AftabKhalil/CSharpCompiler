using System;
using System.Collections.Generic;

using Compiler.HelperClasses;

namespace Compiler.Phases
{
    public class SyntaxAnalysis
    {
        protected Token[] Tokens;
        protected int i;
        public SyntaxAnalysis(Token[] Tokens)
        {
            this.Tokens = Tokens;
            i = 0;
        }

        public SyntaxAnalysisResult Parse()
        {
            SyntaxAnalysisResult result = new SyntaxAnalysisResult();
            try
            {
                result.SyntaxParsed = ParseSyntax();
            }
            catch (Exception ex)
            {
                result.SyntaxParsed = false;
                result.Exception = ex;
            }

            return result;
        }

        private bool ParseSyntax()
        {
            if (Tokens[i].Type == Constants.ACCESS_MODIFIER)
            {
                i++;
                if (Tokens[i].Type == Constants.STATIC)
                {
                    i++;
                }
                if (Tokens[i].Type == Constants.CLASS)
                {
                    i++;
                    if (Tokens[i].Type == Constants.IDENTIFIER)
                    {
                        i++;
                        if (Tokens[i].Type == Constants.CUR_BRACKET_LEFT)
                        {
                            i++;
                            if (S0())
                            {
                                if (Tokens[i].Type == Constants.CUR_BRACKET_RIGHT)
                                {
                                    i++;
                                    return true;
                                }
                                return ThrowSyntaxError(Constants.CUR_BRACKET_RIGHT);
                            }
                        }
                        return ThrowSyntaxError(Constants.CUR_BRACKET_LEFT);
                    }
                    return ThrowSyntaxError(Constants.IDENTIFIER);
                }
                return ThrowSyntaxError(Constants.CLASS);
            }
            return ThrowSyntaxError(Constants.ACCESS_MODIFIER);
        }

        private bool S0()
        {
            if (Tokens[i].Type == Constants.ACCESS_MODIFIER)
            {
                i++;
                if (S1())
                {
                    return true;
                }
            }
            else if (Tokens[i].Type == Constants.CUR_BRACKET_RIGHT)
            {
                return true;
            }
            return ThrowSyntaxError(Constants.CUR_BRACKET_RIGHT);
        }

        private bool S1()
        {
            if (Tokens[i].Type == Constants.STATIC)
            {
                i++;
            }

            if (Tokens[i].Type == Constants.VOID)
            {
                i++;
                if (Tokens[i].Type == Constants.IDENTIFIER)
                {
                    i++;
                    if (METHOD_DECLARATION())
                    {
                        if (S0())
                        {
                            return true;
                        }
                    }
                    return ThrowSyntaxError(Constants.SMALL_BRACKET_LEFT);
                }
            }

            if (Tokens[i].Type == Constants.DATA_TYPE)
            {
                i++;
                {
                    if (Tokens[i].Type == Constants.IDENTIFIER)
                    {
                        i++;
                        if (S2())
                        {
                            if (S0())
                            {
                                return true;
                            }
                        }
                    }
                    else if (Tokens[i].Type == Constants.SQUARE_BRACKET_LEFT)
                    {
                        if (ARRAY_DECLARATION())
                        {
                            if (S0())
                            {
                                return true;
                            }
                        }
                    }
                    return ThrowSyntaxError(Constants.IDENTIFIER);
                }
            }
            return ThrowSyntaxError(Constants.DATA_TYPE);
        }

        private bool S2()
        {
            if (Tokens[i].Type == Constants.TERMINATOR)
            {
                i++;
                return true;
            }
            if (Tokens[i].Type == Constants.SMALL_BRACKET_LEFT)
            {
                if (METHOD_DECLARATION())
                {
                    return true;
                }
            }
            else if (Tokens[i].Type == Constants.EQUAL)
            {
                i++;
                if (E())
                {
                    if (Tokens[i].Type == Constants.COMMA)
                    {
                        if (VARIABLE_DECLARATION_1())
                        {
                            return true;
                        }
                    }
                    else if (Tokens[i].Type == Constants.TERMINATOR)
                    {
                        i++;
                        return true;
                    }
                    return ThrowSyntaxError(Constants.TERMINATOR);
                }
            }

            return false;
        }

        private bool METHOD_DECLARATION()
        {
            if (Tokens[i].Type == Constants.SMALL_BRACKET_LEFT)
            {
                i++;
                METHOD_DECLARATION_ARGS();
                if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
                {
                    i++;
                    if (Tokens[i].Type == Constants.CUR_BRACKET_LEFT)
                    {
                        if (MST())
                        {
                            return true;
                        }
                    }
                    return ThrowSyntaxError(Constants.CUR_BRACKET_LEFT);
                }
                return ThrowSyntaxError(Constants.SMALL_BRACKET_RIGHT);
            }
            return ThrowSyntaxError(Constants.SMALL_BRACKET_LEFT);
        }

        private bool METHOD_DECLARATION_ARGS()
        {
            if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
            {
                return true;
            }

            if (Tokens[i].Type == Constants.DATA_TYPE)
            {
                i++;
                if (Tokens[i].Type == Constants.IDENTIFIER)
                {
                    i++;
                    if (Tokens[i].Type == Constants.COMMA)
                    {
                        i++;
                        if (Tokens[i].Type == Constants.DATA_TYPE)
                        {
                            if (METHOD_DECLARATION_ARGS())
                            {
                                return true;
                            }
                        }
                        return ThrowSyntaxError(Constants.DATA_TYPE);
                    }
                    if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
                    {
                        return true;
                    }
                    return ThrowSyntaxError(Constants.SMALL_BRACKET_RIGHT);
                }
                return ThrowSyntaxError(Constants.IDENTIFIER);
            }
            return ThrowSyntaxError(Constants.DATA_TYPE);
        }

        private bool MST()
        {
            if (Tokens[i].Type == Constants.CUR_BRACKET_LEFT)
            {
                i++;
                while (SSTS())
                {
                    SST();
                }
                if (Tokens[i].Type == Constants.CUR_BRACKET_RIGHT)
                {
                    i++;
                    return true;
                }
                return ThrowSyntaxError(Constants.CUR_BRACKET_RIGHT);
            }

            else if (SST())
            {
                return true;
            }
            return false;
        }

        private bool SST()
        {
            if (Tokens[i].Type == Constants.IDENTIFIER)
            {
                i++;
                if (ID_F())
                {
                    if (Tokens[i].Type == Constants.TERMINATOR)
                    {
                        i++;
                        if (SST())
                        {
                            return true;
                        }
                    }
                    else if (SSTF())
                    {
                        return true;
                    }
                    return ThrowSyntaxError(Constants.TERMINATOR);
                }
            }

            //Variable declaration
            else if (Tokens[i].Type == Constants.DATA_TYPE)
            {
                if (VARIABLE_DECLARATION())
                {
                    return true;
                }
            }

            //If statement
            else if (Tokens[i].Type == Constants.IF)
            {
                if (IF())
                {
                    if (MST())
                    {
                        return true;
                    }
                    if (Tokens[i].Type == Constants.ELSE)
                    {
                        i++;
                        if (MST())
                        {
                            return true;
                        }
                    }
                }
            }

            //For loop
            else if (Tokens[i].Type == Constants.FOR)
            {
                i++;
                if (Tokens[i].Type == Constants.SMALL_BRACKET_LEFT)
                {
                    i++;
                    if (Tokens[i].Type == Constants.DATA_TYPE)
                    {
                        if (VARIABLE_DECLARATION())
                        {
                            if (E())
                            {
                                if (Tokens[i].Type == Constants.TERMINATOR)
                                {
                                    i++;
                                    if (SST())
                                    {
                                        if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
                                        {
                                            i++;
                                            if (MST())
                                            {
                                                return true;
                                            }
                                        }
                                        return ThrowSyntaxError(Constants.SMALL_BRACKET_RIGHT);
                                    }
                                }
                                return ThrowSyntaxError(Constants.TERMINATOR);
                            }
                        }
                    }
                    if (Tokens[i].Type == Constants.IDENTIFIER)
                    {
                        if (VARIABLE_DECLARATION_1())
                        {
                            if (E())
                            {
                                if (Tokens[i].Type == Constants.TERMINATOR)
                                {
                                    i++;
                                    if (SST())
                                    {
                                        if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
                                        {
                                            i++;
                                            if (MST())
                                            {
                                                return true;
                                            }
                                        }
                                        return ThrowSyntaxError(Constants.SMALL_BRACKET_RIGHT);
                                    }
                                }
                                return ThrowSyntaxError(Constants.TERMINATOR);
                            }
                        }
                    }
                    if (Tokens[i].Type == Constants.TERMINATOR)
                    {
                        if (E())
                        {
                            if (Tokens[i].Type == Constants.TERMINATOR)
                            {
                                i++;
                                if (SST())
                                {
                                    if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
                                    {
                                        i++;
                                        if (MST())
                                        {
                                            return true;
                                        }
                                    }
                                    return ThrowSyntaxError(Constants.SMALL_BRACKET_RIGHT);
                                }
                            }
                            return ThrowSyntaxError(Constants.TERMINATOR);
                        }
                    }
                    return ThrowSyntaxError(Constants.TERMINATOR);
                }
                return ThrowSyntaxError(Constants.SMALL_BRACKET_LEFT);
            }

            //While loop
            else if (Tokens[i].Type == Constants.WHILE)
            {
                i++;
                {
                    if (Tokens[i].Type == Constants.SMALL_BRACKET_LEFT)
                    {
                        i++;
                        if (E())
                        {
                            if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
                            {
                                i++;
                                if (MST())
                                {
                                    return true;
                                }
                            }
                            return ThrowSyntaxError(Constants.SMALL_BRACKET_RIGHT);
                        }
                    }
                    return ThrowSyntaxError(Constants.SMALL_BRACKET_LEFT);
                }
            }

            //Do while loop
            if (Tokens[i].Type == Constants.DO)
            {
                i++;
                if (MST())
                {
                    if (Tokens[i].Type == Constants.WHILE)
                    {
                        i++;
                        if (Tokens[i].Type == Constants.SMALL_BRACKET_LEFT)
                        {
                            i++;
                            if (E())
                            {
                                if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
                                {
                                    i++;
                                    if (Tokens[i].Type == Constants.TERMINATOR)
                                    {
                                        i++;
                                        return true;
                                    }
                                    return ThrowSyntaxError(Constants.TERMINATOR);
                                }
                                return ThrowSyntaxError(Constants.SMALL_BRACKET_RIGHT);
                            }
                        }
                        return ThrowSyntaxError(Constants.SMALL_BRACKET_LEFT);
                    }
                    return ThrowSyntaxError(Constants.WHILE);
                }
            }

            //Print statement
            if (Tokens[i].Type == Constants.PRINT)
            {
                i++;
                if (Tokens[i].Type == Constants.SMALL_BRACKET_LEFT)
                {
                    i++;
                    if (E())
                    {
                        if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
                        {
                            i++;
                            if (Tokens[i].Type == Constants.TERMINATOR)
                            {
                                i++;
                                return true;
                            }
                            return ThrowSyntaxError(Constants.TERMINATOR);
                        }
                        return ThrowSyntaxError(Constants.SMALL_BRACKET_RIGHT);
                    }
                }
                return ThrowSyntaxError(Constants.SMALL_BRACKET_LEFT);
            }

            //Return statement
            if (Tokens[i].Type == Constants.RETURN)
            {
                i++;
                E();
                if (Tokens[i].Type == Constants.TERMINATOR)
                {
                    i++;
                    return true;
                }
                return ThrowSyntaxError(Constants.TERMINATOR);
            }

            if (Tokens[i].Type == Constants.BREAK)
            {
                i++;
                if (Tokens[i].Type == Constants.TERMINATOR)
                {
                    i++;
                    return true;
                }
                return ThrowSyntaxError(Constants.TERMINATOR);
            }

            else if (SSTF())
            {
                return true;
            }
            return false;
        }

        private bool SSTS()
        {
            if (Tokens[i].Type == Constants.IDENTIFIER || Tokens[i].Type == Constants.RETURN || Tokens[i].Type == Constants.DATA_TYPE || Tokens[i].Type == Constants.IF || Tokens[i].Type == Constants.FOR || Tokens[i].Type == Constants.WHILE || Tokens[i].Type == Constants.DO || Tokens[i].Type == Constants.PRINT)
            {
                return true;
            }
            return false;
        }

        private bool SSTF()
        {
            if (Tokens[i].Type == Constants.CUR_BRACKET_RIGHT || Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT || SSTS())
            {
                return true;
            }
            return false;
        }

        private bool IF()
        {
            if (Tokens[i].Type == Constants.IF)
            {
                i++;
                if (Tokens[i].Type == Constants.SMALL_BRACKET_LEFT)
                {
                    i++;
                    if (E())
                    {
                        if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
                        {
                            i++;
                            if (MST())
                            {
                                return true;
                            }
                        }
                        return ThrowSyntaxError(Constants.SMALL_BRACKET_RIGHT);
                    }
                    return ThrowSyntaxError("Expression");
                }
                return ThrowSyntaxError(Constants.CUR_BRACKET_LEFT);
            }
            return false;
        }

        private bool VARIABLE_DECLARATION()
        {
            if (Tokens[i].Type == Constants.DATA_TYPE)
            {
                i++;
                if (Tokens[i].Type == Constants.IDENTIFIER)
                {
                    i++;
                    if (Tokens[i].Type == Constants.EQUAL)
                    {
                        i++;
                        if (E())
                        {
                            if (Tokens[i].Type == Constants.COMMA)
                            {
                                i++;
                                if (VARIABLE_DECLARATION_1())
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    if (Tokens[i].Type == Constants.COMMA)
                    {
                        i++;
                        if (VARIABLE_DECLARATION_1())
                        {
                            return true;
                        }
                    }
                    if (Tokens[i].Type == Constants.TERMINATOR)
                    {
                        i++;
                        return true;
                    }
                    return ThrowSyntaxError(Constants.TERMINATOR);
                }
                if (Tokens[i].Type == Constants.SQUARE_BRACKET_LEFT)
                {
                    if (ARRAY_DECLARATION())
                    {
                        return true;
                    }
                }
                return ThrowSyntaxError(Constants.IDENTIFIER);
            }
            return ThrowSyntaxError(Constants.DATA_TYPE);
        }

        private bool VARIABLE_DECLARATION_1()
        {
            if (Tokens[i].Type == Constants.IDENTIFIER)
            {
                i++;
                if (Tokens[i].Type == Constants.EQUAL)
                {
                    i++;
                    if (E())
                    {
                        if (Tokens[i].Type == Constants.COMMA)
                        {
                            i++;
                            if (VARIABLE_DECLARATION_1())
                            {
                                return true;
                            }
                        }
                    }
                }
                if (Tokens[i].Type == Constants.COMMA)
                {
                    i++;
                    if (VARIABLE_DECLARATION_1())
                    {
                        return true;
                    }
                }
                if (Tokens[i].Type == Constants.TERMINATOR)
                {
                    i++;
                    return true;
                }
                return ThrowSyntaxError(Constants.TERMINATOR);
            }
            return ThrowSyntaxError(Constants.IDENTIFIER);
        }

        private bool ARRAY_DECLARATION()
        {
            if (Tokens[i].Type == Constants.SQUARE_BRACKET_LEFT)
            {
                i++;
                if (Tokens[i].Type == Constants.SQUARE_BRACKET_RIGHT)
                {
                    i++;
                    if (Tokens[i].Type == Constants.IDENTIFIER)
                    {
                        i++;
                        if (Tokens[i].Type == Constants.EQUAL)
                        {
                            i++;

                            if (Tokens[i].Type == Constants.NEW)
                            {
                                i++;
                                if (Tokens[i].Type == Constants.DATA_TYPE)
                                {
                                    i++;
                                    if (Tokens[i].Type == Constants.SQUARE_BRACKET_LEFT)
                                    {
                                        i++;
                                        if (E())
                                        {
                                            if (Tokens[i].Type == Constants.SQUARE_BRACKET_RIGHT)
                                            {
                                                i++;
                                                if (Tokens[i].Type == Constants.TERMINATOR)
                                                {
                                                    i++;
                                                    return true;
                                                }
                                                return ThrowSyntaxError(Constants.TERMINATOR);
                                            }
                                            return ThrowSyntaxError(Constants.SQUARE_BRACKET_RIGHT);
                                        }
                                    }
                                    return ThrowSyntaxError(Constants.SQUARE_BRACKET_LEFT);
                                }
                                return ThrowSyntaxError(Constants.DATA_TYPE);
                            }
                            return ThrowSyntaxError(Constants.NEW);
                        }
                        if (Tokens[i].Type == Constants.COMMA)
                        {
                            i++;
                            if (ARRAY_DECLARATION_1())
                            {
                                return true;
                            }
                        }
                        if (Tokens[i].Type == Constants.TERMINATOR)
                        {
                            i++;
                            return true;
                        }
                        return ThrowSyntaxError(Constants.TERMINATOR);
                    }
                }
            }
            return false;
        }

        private bool ARRAY_DECLARATION_1()
        {
            if (Tokens[i].Type == Constants.IDENTIFIER)
            {
                i++;
                if (Tokens[i].Type == Constants.EQUAL)
                {
                    i++;
                    if (Tokens[i].Type == Constants.NEW)
                    {
                        i++;
                        if (Tokens[i].Type == Constants.DATA_TYPE)
                        {
                            i++;
                            if (Tokens[i].Type == Constants.SQUARE_BRACKET_LEFT)
                            {
                                i++;
                                if (E())
                                {
                                    if (Tokens[i].Type == Constants.SQUARE_BRACKET_RIGHT)
                                    {
                                        i++;
                                        if (Tokens[i].Type == Constants.TERMINATOR)
                                        {
                                            i++;
                                            return true;
                                        }
                                        return ThrowSyntaxError(Constants.TERMINATOR);
                                    }
                                    return ThrowSyntaxError(Constants.SQUARE_BRACKET_RIGHT);
                                }
                            }
                            return ThrowSyntaxError(Constants.SQUARE_BRACKET_LEFT);
                        }
                        return ThrowSyntaxError(Constants.DATA_TYPE);
                    }
                    return ThrowSyntaxError(Constants.NEW);
                }
                if (Tokens[i].Type == Constants.COMMA)
                {
                    i++;
                    if (VARIABLE_DECLARATION_1())
                    {
                        return true;
                    }
                }
                if (Tokens[i].Type == Constants.TERMINATOR)
                {
                    i++;
                    return true;
                }
                return ThrowSyntaxError(Constants.TERMINATOR);
            }
            return ThrowSyntaxError(Constants.IDENTIFIER);
        }

        private bool E()
        {
            if (E0())
            {
                if (F())
                {
                    if (E1())
                    {
                        return true;
                    }
                }
            }
            return ThrowSyntaxError("Expresion");
        }

        private bool E0()
        {
            if (Tokens[i].Type == Constants.IDENTIFIER || Tokens[i].Type == Constants.SMALL_BRACKET_LEFT || Tokens[i].Type == Constants.SQUARE_BRACKET_LEFT || Tokens[i].Type == Constants.INC || Tokens[i].Type == Constants.DEC || Tokens[i].Type == Constants.NOT || Tokens[i].Type == Constants.INT_CONSTANT || Tokens[i].Type == Constants.FLOAT_CONSTANT || Tokens[i].Type == Constants.BOOL_CONSTANT || Tokens[i].Type == Constants.STRING_CONSTANT || Tokens[i].Type == Constants.CHAR_CONSTANT)
            {
                return true;
            }
            return false;
        }

        private bool E1()
        {
            if (Tokens[i].Type == Constants.ASSIGNMENT_OPERATOR)
            {
                i++;
                if (F())
                {
                    if (E1())
                    {
                        return true;
                    }
                }
            }
            else if (EF())
            {
                return true;
            }
            return false;
        }

        private bool EF()
        {
            if (Tokens[i].Type == Constants.TERMINATOR || Tokens[i].Type == Constants.COMMA || Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT || Tokens[i].Type == Constants.SQUARE_BRACKET_RIGHT)
            {
                return true;
            }
            return false;
        }

        private bool F()
        {
            if (E0())
            {
                if (G())
                {
                    if (F1())
                    {
                        return true;
                    }

                }
            }
            else if (EF() || Tokens[i].Type == Constants.ASSIGNMENT_OPERATOR)
            {
                return true;
            }
            return false;
        }

        private bool F1()
        {
            if (Tokens[i].Type == Constants.OR)
            {
                i++;
                if (G())
                {
                    if (F1())
                    {
                        return true;
                    }
                }
            }
            else if (EF() || Tokens[i].Type == Constants.ASSIGNMENT_OPERATOR)
            {
                return true;
            }
            return false;
        }

        private bool G()
        {
            if (E0())
            {
                if (H())
                {
                    if (G1())
                    {
                        return true;
                    }
                }
            }
            else if (EF() || Tokens[i].Type == Constants.ASSIGNMENT_OPERATOR || Tokens[i].Type == Constants.OR)
            {
                return true;
            }
            return false;
        }

        private bool G1()
        {
            if (Tokens[i].Type == Constants.AND)
            {
                i++;
                if (H())
                {
                    if (G1())
                    {
                        return true;
                    }
                }
            }
            else if (EF() || Tokens[i].Type == Constants.ASSIGNMENT_OPERATOR || Tokens[i].Type == Constants.OR)
            {
                return true;
            }
            return false;
        }

        private bool H()
        {
            if (E0())
            {
                if (I())
                {
                    if (H1())
                    {
                        return true;
                    }
                }
            }
            else if (EF() || Tokens[i].Type == Constants.ASSIGNMENT_OPERATOR || Tokens[i].Type == Constants.OR || Tokens[i].Type == Constants.AND)
            {
                return true;
            }
            return false;
        }

        private bool H1()
        {
            if (Tokens[i].Type == Constants.RELATIONAL_OPERATION)
            {
                i++;
                if (I())
                {
                    if (H1())
                    {
                        return true;
                    }
                }
            }
            else if (EF() || Tokens[i].Type == Constants.ASSIGNMENT_OPERATOR || Tokens[i].Type == Constants.OR || Tokens[i].Type == Constants.AND)
            {
                return true;
            }
            return false;
        }

        private bool I()
        {
            if (E0())
            {
                if (J())
                {
                    if (I1())
                    {
                        return true;
                    }
                }
            }
            else if (EF() || Tokens[i].Type == Constants.ASSIGNMENT_OPERATOR || Tokens[i].Type == Constants.OR || Tokens[i].Type == Constants.AND || Tokens[i].Type == Constants.RELATIONAL_OPERATION)
            {
                return true;
            }
            return false;
        }

        private bool I1()
        {
            if (Tokens[i].Type == Constants.ADDSUB)
            {
                i++;
                if (J())
                {
                    if (I1())
                    {
                        return true;
                    }
                }
            }
            else if (EF() || Tokens[i].Type == Constants.ASSIGNMENT_OPERATOR || Tokens[i].Type == Constants.OR || Tokens[i].Type == Constants.AND || Tokens[i].Type == Constants.RELATIONAL_OPERATION)
            {
                return true;
            }
            return false;
        }

        private bool J()
        {
            if (E0())
            {
                if (K())
                {
                    if (J1())
                    {
                        return true;
                    }
                }
            }
            else if (EF() || Tokens[i].Type == Constants.ASSIGNMENT_OPERATOR || Tokens[i].Type == Constants.OR || Tokens[i].Type == Constants.AND || Tokens[i].Type == Constants.RELATIONAL_OPERATION || Tokens[i].Type == Constants.ADDSUB)
            {
                return true;
            }
            return false;
        }

        private bool J1()
        {
            if (Tokens[i].Type == Constants.DIVMUL)
            {
                i++;
                if (K())
                {
                    if (J1())
                    {
                        return true;
                    }
                }
            }
            else if (EF() || Tokens[i].Type == Constants.ASSIGNMENT_OPERATOR || Tokens[i].Type == Constants.OR || Tokens[i].Type == Constants.AND || Tokens[i].Type == Constants.RELATIONAL_OPERATION || Tokens[i].Type == Constants.ADDSUB)
            {
                return true;
            }
            return false;
        }

        private bool K()
        {
            if (Tokens[i].Type == Constants.IDENTIFIER)
            {
                i++;
                if (ID_1())
                {
                    return true;
                }
            }
            else if (CONST())
            {
                return true;
            }

            else if (Tokens[i].Type == Constants.SMALL_BRACKET_LEFT)
            {
                i++;
                if (E())
                {
                    if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
                    {
                        i++;
                        return true;
                    }
                    return ThrowSyntaxError(Constants.SMALL_BRACKET_RIGHT);
                }
                return ThrowSyntaxError("Expression");
            }

            else if (Tokens[i].Type == Constants.INC_DEC)
            {
                i++;
                if (Tokens[i].Type == Constants.IDENTIFIER)
                {
                    i++;
                    if (Tokens[i].Type == Constants.SMALL_BRACKET_LEFT)
                    {
                        if (METHOD_CALL())
                        {
                            return true;
                        }
                    }
                    return true;
                }
            }
            else if (Tokens[i].Type == Constants.NOT)
            {
                i++;
                if (E())
                {
                    return true;
                }
            }
            return false;
        }

        private bool KF()
        {
            if (EF() || Tokens[i].Type == Constants.DIVMUL || Tokens[i].Type == Constants.ADDSUB || Tokens[i].Type == Constants.RELATIONAL_OPERATION || Tokens[i].Type == Constants.AND || Tokens[i].Type == Constants.OR || Tokens[i].Type == Constants.ASSIGNMENT_OPERATOR)
            {
                return true;
            }
            return false;
        }

        private bool CONST()
        {
            if (Tokens[i].Type == Constants.INT_CONSTANT || Tokens[i].Type == Constants.BOOL_CONSTANT || Tokens[i].Type == Constants.FLOAT_CONSTANT || Tokens[i].Type == Constants.STRING_CONSTANT || Tokens[i].Type == Constants.CHAR_CONSTANT)
            {
                i++;
                return true;
            }
            return false;
        }

        private bool ID_1()
        {
            if (Tokens[i].Type == Constants.INC_DEC)
            {
                i++;
                return true;
            }
            else if (Tokens[i].Type == Constants.DOT)
            {
                i++;
                if (C_M())
                {
                    return true;
                }
            }
            else if (Tokens[i].Type == Constants.SMALL_BRACKET_LEFT)
            {
                if (METHOD_CALL())
                {
                    return true;
                }
            }
            else if (Tokens[i].Type == Constants.SQUARE_BRACKET_LEFT)
            {
                i++;
                if (E())
                {
                    if (Tokens[i].Type == Constants.SQUARE_BRACKET_RIGHT)
                    {
                        i++;
                        return true;
                    }
                }
            }
            else if (KF())
            {
                return true;
            }
            return false;
        }

        private bool ID_F()
        {
            if (Tokens[i].Type == Constants.EQUAL)
            {
                i++;
                if (E())
                {
                    return true;
                }
                return ThrowSyntaxError("Expression");
            }

            if (Tokens[i].Type == Constants.DOT)
            {
                i++;
                if (Tokens[i].Type == Constants.IDENTIFIER)
                {
                    i++;
                    if (Tokens[i].Type == Constants.EQUAL)
                    {
                        i++;
                        if (E())
                        {
                            return true;
                        }
                        return ThrowSyntaxError("Expression");
                    }
                    return ThrowSyntaxError(Constants.EQUAL);
                }
            }

            if (Tokens[i].Type == Constants.INC_DEC)
            {
                i++;
                return true;
            }

            if (Tokens[i].Type == Constants.ASSIGNMENT_OPERATOR)
            {
                i++;
                if (E())
                {
                    return true;
                }
                return ThrowSyntaxError("Expresion");
            }

            if (Tokens[i].Type == Constants.SQUARE_BRACKET_LEFT)
            {
                i++;
                if (E())
                {
                    if (Tokens[i].Type == Constants.SQUARE_BRACKET_RIGHT)
                    {
                        i++;
                        if (Tokens[i].Type == Constants.EQUAL)
                        {
                            i++;
                            if (E())
                            {
                                return true;
                            }
                        }
                        return ThrowSyntaxError(Constants.SQUARE_BRACKET_RIGHT);
                    }
                    return ThrowSyntaxError(Constants.SQUARE_BRACKET_RIGHT);
                }
            }

            return false;
        }

        private bool C_M()
        {
            if (Tokens[i].Type == Constants.INT_CONSTANT)
            {
                i++;
                return true;
            }

            if (Tokens[i].Type == Constants.IDENTIFIER)
            {
                i++;
                if (Tokens[i].Type == Constants.SMALL_BRACKET_LEFT)
                {
                    if (METHOD_CALL())
                    {
                        return true;
                    }
                }
                return true;
            }
            return false;
        }

        private bool METHOD_CALL()
        {
            if (Tokens[i].Type == Constants.SMALL_BRACKET_LEFT)
            {
                i++;
                if (E0())
                {
                    if (METHOD_CALL_PARM_LIST())
                    {
                        if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
                        {
                            i++;
                            return true;
                        }
                        return ThrowSyntaxError(Constants.SMALL_BRACKET_RIGHT);
                    }
                }
                else if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
                {
                    return true;
                }
                return ThrowSyntaxError(Constants.SMALL_BRACKET_RIGHT);
            }
            return ThrowSyntaxError(Constants.SMALL_BRACKET_LEFT);
        }

        private bool METHOD_CALL_PARM_LIST()
        {
            if (E())
            {
                if (METHOD_CALL_PARM_LIST_1())
                {
                    return true;
                }
            }
            return false;
        }

        private bool METHOD_CALL_PARM_LIST_1()
        {
            if (Tokens[i].Type == Constants.COMMA)
            {
                i++;
                if (E())
                {
                    if (METHOD_CALL_PARM_LIST_1())
                    {
                        return true;
                    }
                }
                return ThrowSyntaxError("Expression");
            }
            else if (Tokens[i].Type == Constants.SMALL_BRACKET_RIGHT)
            {
                return true;
            }
            return ThrowSyntaxError(Constants.SMALL_BRACKET_RIGHT);
        }

        private bool ThrowSyntaxError(string requiredToken)
        {
            string errorString = string.Format("Require token '{0}' at line {1} instead of token '{2}'", requiredToken, Tokens[i].LineNumber, Tokens[i].Value);
            if (i-- != -1)
                errorString += string.Format(", after token '{0}'", Tokens[i].Value);
            throw new Exception(errorString);
        }
    }
}
