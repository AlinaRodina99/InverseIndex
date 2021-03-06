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
            input = input.Replace("AND", "&").Replace("&&", "&");
            input = input.Replace("OR", "|").Replace("||", "|");
            input = input.Replace("NOT", "-");
            if (input.Trim().Contains(' ') && !input.Contains('&') && !input.Contains('|') && !input.Contains('-'))
            {
                throw new ArgumentException();
            }
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
            var amountOfWords = 0;
            var amountOfAndsAndOrs = 0;

            for (var i = 0; i < input.Length; ++i)
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
                            ++amountOfAndsAndOrs;
                            break;
                        }
                    case '|':
                        {
                            while (stack.Count > 0 && (stack.Peek() == "& " || stack.Peek() == "- " || stack.Peek() == "| "))
                            {
                                output += stack.Pop();
                            }
                            stack.Push("| ");
                            ++amountOfAndsAndOrs;
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
                                ++amountOfWords;
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

            if (amountOfWords != amountOfAndsAndOrs + 1)
            {
                throw new ArgumentException();
            }

            output = RemoveDoubleNot(output);

            return output.Trim();
        }
    }
}
