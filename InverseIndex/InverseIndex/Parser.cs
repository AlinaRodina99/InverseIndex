using System;
using System.Collections.Generic;
using System.Linq;

namespace InverseIndex
{
    /// <summary>
    /// Query's parser
    /// </summary>
    public class Parser
    {
        private string input;

        /// <summary>
        /// Query's parser constructor
        /// </summary>
        /// <param name="input">Input string</param>
        public Parser(string input)
        {
            input = input.Replace("AND", "&");
            input = input.Replace("OR", "|");
            input = input.Replace("NOT", "-");
            this.input = input;
        }

        /// <summary>
        /// Removes all spaces in given string
        /// </summary>
        /// <param name="input">Given string</param>
        /// <returns>String without spaces</returns>
        private string RemoveSpaces(string input) => string.Concat(input.Where(c => !char.IsWhiteSpace(c)));

        /// <summary>
        /// Removes double NOT in given string
        /// </summary>
        /// <param name="input">Given string</param>
        /// <returns>String without double NOT's</returns>
        private string RemoveDoubleNot(string input) => input.Replace("- - ", "");

        /// <summary>
        /// Parses input to reversed polish notation
        /// </summary>
        /// <returns>Input in reversed polish notation</returns>
        public string Parse()
        {
            var output = "";
            var stack = new Stack<string>();
            input = RemoveSpaces(input);
            var word = "";

            for (var i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '(':
                        {
                            stack.Push("(");
                            break;
                        }
                    case ')':
                        {
                            var pop = stack.Pop();
                            while (pop != "(")
                            {
                                output += pop;
                                pop = stack.Pop();
                            }
                            break;
                        }
                    case '&':
                        {
                            while (stack.Count > 0 && (stack.Peek() == "- " || stack.Peek() == "& "))
                            {
                                output += stack.Pop();
                            }
                            stack.Push("& ");
                            break;
                        }
                    case '|':
                        {
                            while (stack.Count > 0 && (stack.Peek() == "& " || stack.Peek() == "- " || stack.Peek() == "| "))
                            {
                                output += stack.Pop();
                            }
                            stack.Push("| ");
                            break;
                        }
                    case '-':
                        {
                            stack.Push("- ");
                            break;
                        }
                    default:
                        {
                            word += input[i];
                            if (i == input.Length - 1 || input[i + 1] == '(' || input[i + 1] == ')' || input[i + 1] == '-' || input[i + 1] == '&' ||
                                input[i + 1] == '|')
                            {
                                output += word + " ";
                                word = "";
                            }
                            break;
                        }
                }
            }
            while (stack.Count > 0)
            {
                var pop = stack.Pop();
                if (pop == "(")
                {
                    throw new ArgumentException();
                }
                output += pop;
            }
            output = RemoveDoubleNot(output);

            return output.Trim();
        }
    }
}
