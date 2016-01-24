// Skeleton written by Joe Zachary for CS 3500, January 2015
// Revised by Joe Zachary, January 2016
// JLZ Repaired pair of mistakes, January 23, 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public class Formula
    {
        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>

        string[] formulaArray;

        public Formula(string formula)
        {
            int leftParen = 0;
            int rightParen = 0;

            if(formula.Length == 0)
            {
                throw new FormulaFormatException("No tokens detected"); 
            }

            char[] formChar = formula.ToCharArray();
            
            if(!char.IsLetterOrDigit(formChar[0]) || formChar[0] != '(')
            {
                throw new FormulaFormatException("Starting token must be a: number, variable, or opening Parenthese");
            }

            if (!char.IsLetterOrDigit(formChar[formChar.Count() -1]) || formChar[0] != ')')
            {
                throw new FormulaFormatException("Ending token must be a: number, variable, or opening Parenthese");
            }

            for(int i = 0; i < formChar.Length; i++)
            {

            }


        }
        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            Stack<double> valueStack = new Stack<double>();
            Stack<string> operatorStack = new Stack<string>();
            double result = 0;
            foreach(string s in formulaArray)
            {
                result = 0;
                if(double.TryParse(s, out result))
                {
                    if(operatorStack.Peek() == "*")
                    {
                        operatorStack.Pop();
                        result *= valueStack.Pop();
                    }
                    else if(operatorStack.Peek() == "/")
                    {
                        if (valueStack.Peek() != 0)
                        {
                            operatorStack.Pop();
                            result /= valueStack.Pop();
                        }
                        else
                        {
                            throw new FormulaEvaluationException("There is a division by zero in your formula.");
                        }
                    }

                    else
                    {
                        valueStack.Push(result);
                    }
                }
                else if(s == "+" || s == "-")
                {
                    if(operatorStack.Peek() == "+")
                    {
                        operatorStack.Pop();
                        result = valueStack.Pop() + valueStack.Pop();
                        valueStack.Push(result);
                    }
                    else
                    {
                        operatorStack.Pop();
                        result = valueStack.Pop() - valueStack.Pop();
                        valueStack.Push(result);
                    }
                    operatorStack.Push(s);
                }

                else if(s == "*" || s == "/" || s == "(")
                {
                    operatorStack.Push(s);
                }

                else if(s == ")")
                {
                    if (operatorStack.Peek() == "+" || s == "-")
                    {
                        if (operatorStack.Peek() == "+")
                        {
                            operatorStack.Pop();
                            result = valueStack.Pop() + valueStack.Pop();
                            valueStack.Push(result);
                        }
                        else
                        {
                            operatorStack.Pop();
                            result = valueStack.Pop() - valueStack.Pop();
                            valueStack.Push(result);
                        }
                        operatorStack.Push(s);
                    }

                    operatorStack.Pop();

                    if(operatorStack.Peek() == "*" || operatorStack.Peek() == "/")
                    {
                        if(operatorStack.Peek() == "*")
                        {
                            operatorStack.Pop();
                            result = valueStack.Pop() * valueStack.Pop();
                            valueStack.Push(result);
                        }
                        else
                        {
                            if (valueStack.Peek() != 0)
                            {
                                operatorStack.Pop();
                                result /= valueStack.Pop();
                            }
                            else
                            {
                                throw new FormulaEvaluationException("There is a division by zero in your formula.");
                            }
                        }
                    }
                }
                else
                {
                    if (operatorStack.Peek() == "*")
                    {
                        operatorStack.Pop();
                        result = valueStack.Pop() * lookup(s);
                    }
                    else if (operatorStack.Peek() == "/")
                    {
                        if (valueStack.Peek() != 0)
                        {
                            operatorStack.Pop();
                            valueStack.Push(lookup(s) / valueStack.Pop());
                        }
                        else
                        {
                            throw new FormulaEvaluationException("There is a division by zero in your formula.");
                        }
                    }
                    else
                    {
                        valueStack.Push(lookup(s));
                    }
                                      
                }
            }
            if (operatorStack.Count != 0)
            {
                if(operatorStack.Peek() == "+")
                {
                    return valueStack.Pop() + valueStack.Pop();
                }   
                else if(operatorStack.Peek() == "-")
                {
                    return valueStack.Pop() - valueStack.Pop();
                } 
            }

            return valueStack.Pop();
            
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(string formula)
        {
            // Patterns for individual tokens
            string lpPattern = @"\(";
            string rpPattern = @"\)";
            string opPattern = @"[\+\-*/]";
            string varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            string spacePattern = @"\s+";

            // Overall pattern
            string pattern = string.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(string variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(string message) : base(message)
        {
        }
    }
}
