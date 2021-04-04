using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InverseIndex
{
    /// <summary>
    /// Query's processor
    /// </summary>
    public class Processor
    {
        private string pathToIndex;

        /// <summary>
        /// Query's processor constructor
        /// </summary>
        /// <param name="pathToIndex">Path to built index</param>
        public Processor(string pathToIndex)
        {
            this.pathToIndex = pathToIndex;
        }

        /// <summary>
        /// Finds the line containing given term in a file with index
        /// </summary>
        /// <param name="term">Term to find</param>
        /// <returns>Line containing given term, empty string if there is no line containing given term</returns>
        private string FindLineWithTerm(string term)
        {
            try
            {
                using (var fileStream = File.Open(pathToIndex, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var buferredStream = new BufferedStream(fileStream))
                using (var streamReader = new StreamReader(buferredStream))
                {
                    var line = "";
                    do
                    {
                        line = streamReader.ReadLine();
                    }
                    while (!line.StartsWith(term) && line.Split(' ')[0] != term);
                    return line;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            return "";
        }


        /// <summary>
        /// Finds lines with terms if necessary and parses it to two strings -- docs with terms and a number that indicates what combination of strings is passed
        /// </summary>
        /// <param name="element1">First element-term</param>
        /// <param name="element2">Second element-term</param>
        /// <returns>Two strings and a number: 1 for non-empty strings, 2 for one empty string (at the second place), 3 for two empty strings</returns>
        private (string, string, int) FindAndParse(string element1, string element2)
        {
            if (element1 == "" && element2 == "")
            {
                return (element1, element2, 3);
            }

            var docsWithTerm1 = element1.Any(char.IsLetter) ? string.Join(' ', FindLineWithTerm(element1).Skip(2)) : element1;
            var docsWithTerm2 = element2.Any(char.IsLetter) ? string.Join(' ', FindLineWithTerm(element2).Split(' ').Skip(2)) : element2;
            if (element1 != "" && element2 != "")
            {
                return (docsWithTerm1, docsWithTerm2, 1);
            }

            return element1 == "" ? (docsWithTerm2, docsWithTerm1, 2) : (docsWithTerm1, docsWithTerm2, 2);
        }

        /// <summary>
        /// Executes AND operation on elements
        /// </summary>
        /// <param name="element1">First element</param>
        /// <param name="element2">econd element</param>
        /// <returns>Result of AND operation</returns>
        private string AndOperation(string element1, string element2)
        {
            var parse = FindAndParse(element1, element2);
            switch (parse.Item3)
            {
                case 1:
                    {
                        return string.Join(' ', parse.Item1.Split(' ').Intersect(parse.Item2.Split(' ')));
                    }
                case 2:
                case 3:
                    {
                        return "";
                    }
                default:
                    {
                        throw new ArgumentException();
                    }
            }
        }

        /// <summary>
        /// Executes OR operation on elements
        /// </summary>
        /// <param name="element1">First element</param>
        /// <param name="element2">econd element</param>
        /// <returns>Result of OR operation</returns>
        private string OrOperation(string element1, string element2)
        {
            var parse = FindAndParse(element1, element2);
            switch (parse.Item3)
            {
                case 1:
                    {
                        return string.Join(' ', parse.Item1.Split(' ').Union(parse.Item2.Split(' ')));
                    }
                case 2:
                    {
                        return parse.Item1;
                    }
                case 3:
                    {
                        return "";
                    }
                default:
                    {
                        throw new ArgumentException();
                    }
            }
        }

        public void Process(string query)
        {
            var terms = query.Split(' ');

            var stack = new Stack<string>();
            for (var i = 0; i < terms.Length; ++i)
            {
                switch (terms[i])
                {
                    case "&":
                        {
                            var element1 = stack.Pop();
                            var element2 = stack.Pop();

                            stack.Push();
                            break;
                        }
                    case "|":
                        {
                            var element1 = stack.Pop();
                            var element2 = stack.Pop();
                            break;
                        }
                    case "-":
                        {
                            var element = stack.Pop();
                            break;
                        }
                    default:
                        {
                            stack.Push(terms[i]);
                            break;
                        }
                }
                if (terms[i] == "&" || terms[i] == '-' || terms[i] == '*' || terms[i] == '/')
                {
                    var value1 = stack.Pop();
                    var value2 = stack.Pop();
                    stack.Push(OparationResult(str[i], value1, value2));
                }
                else if (i < str.Length - 1 && Char.IsNumber(str[i]) && Char.IsNumber(str[i + 1]))
                {
                    number = number * 10 + (int)Char.GetNumericValue(str[i]);
                }
                else if (i > 0 && Char.IsNumber(str[i - 1]) && Char.IsNumber(str[i]))
                {
                    number = number * 10 + (int)Char.GetNumericValue(str[i]);
                    stack.Push(number);
                    number = 0;
                }
                else
                {
                    stack.Push((int)Char.GetNumericValue(str[i]));
                }
            }
            if (stack.Length() == 1)
            {
                return stack.Pop();
            }
            else
            {
                throw new InvalidOperationException("Invalid string");
            }
        }
    }
}
