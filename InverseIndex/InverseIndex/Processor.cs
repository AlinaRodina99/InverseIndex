using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InverseIndex
{
    /// <summary>
    /// Query's processor
    /// </summary>
    public class Processor
    {
        private readonly string pathToIndex;
        private readonly int[] docsIds;

        /// <summary>
        /// Query's processor constructor
        /// </summary>
        /// <param name="pathToIndex">Path to built index</param>
        public Processor(string pathToIndex, int[] docsIds)
        {
            this.pathToIndex = pathToIndex;
            this.docsIds = docsIds;
        }

        /// <summary>
        /// Finds the line containing given term in a file with index
        /// </summary>
        /// <param name="term">Term to find</param>
        /// <returns>Line containing given term, empty string if there is no line containing given term</returns>
        private string FindLineWithTerm(string term)
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

            var docsWithTerm1 = element1.Any(char.IsLetter) ? string.Join(' ', FindLineWithTerm(element1).Split(' ').Skip(2)) : element1;
            var docsWithTerm2 = element2.Any(char.IsLetter) ? string.Join(' ', FindLineWithTerm(element2).Split(' ').Skip(2)) : element2;
            if (docsWithTerm1 != "" && docsWithTerm2 != "")
            {
                return (docsWithTerm1, docsWithTerm2, 1);
            }

            return docsWithTerm1 == "" ? (docsWithTerm2, docsWithTerm1, 2) : (docsWithTerm1, docsWithTerm2, 2);
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
                        var docsIds1 = parse.Item1.Split(' ');
                        var docsIds2 = parse.Item2.Split(' ');
                        var i = 0;
                        var j = 0;
                        var andlist = "";
                        while (i < docsIds1.Length && j < docsIds2.Length)
                        {
                            if (int.Parse(docsIds1[i]) < int.Parse(docsIds2[j]))
                            {
                                ++i;
                            }
                            else if (int.Parse(docsIds1[i]) > int.Parse(docsIds2[j]))
                            {
                                ++j;
                            }
                            else
                            {
                                andlist += docsIds1[i] + " ";
                                ++i;
                                ++j;
                            }
                        }
                        return andlist.Trim();
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
                        var docsIds1 = parse.Item1.Split(' ');
                        var docsIds2 = parse.Item2.Split(' ');
                        var i = 0;
                        var j = 0;
                        var orList = "";
                        while (i < docsIds1.Length && j < docsIds2.Length)
                        {
                            if (int.Parse(docsIds1[i]) < int.Parse(docsIds2[j]))
                            {
                                orList += docsIds1[i] + " ";
                                ++i;
                            }
                            else if (int.Parse(docsIds1[i]) > int.Parse(docsIds2[j]))
                            {
                                orList += docsIds2[j] + " ";
                                ++j;
                            }
                            else
                            {
                                orList += docsIds1[i] + " ";
                                ++i;
                                ++j;
                            }
                        }
                        while (i < docsIds1.Length)
                        {
                            orList += docsIds1[i] + " ";
                            ++i;
                        }
                        while (j < docsIds2.Length)
                        {
                            orList += docsIds2[j] + " ";
                            ++j;
                        }
                        return orList.Trim();
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

        /// <summary>
        /// Executes NOT operation on element
        /// </summary>
        /// <param name="element">Element</param>
        /// <returns>Result of NOT operation</returns>
        private string NotOperation(string element)
        {
            if (element == "")
            {
                return string.Join(' ', docsIds);
            }

            var docsWithTerm = element.Any(char.IsLetter) ? FindLineWithTerm(element).Split(' ').Skip(2).ToArray() : element.Split(' ');
            var notList = "";
            var i = 0;
            var j = 0;
            while (i < docsWithTerm.Length)
            {
                if (docsIds[j] < int.Parse(docsWithTerm[i]))
                {
                    notList += docsIds[j] + " ";
                    ++j;
                }
                else
                {
                    ++j;
                    ++i;
                }
            }
            while (j < docsIds.Length)
            {
                notList += docsIds[j] + " ";
                ++j;
            }
            return notList.Trim();
        }

        /// <summary>
        /// Processes given query
        /// </summary>
        /// <param name="query">Given query</param>
        /// <returns>String with docs' ids according to query</returns>
        public string Process(string query)
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
                            stack.Push(AndOperation(element1, element2));
                            break;
                        }
                    case "|":
                        {
                            var element1 = stack.Pop();
                            var element2 = stack.Pop();
                            stack.Push(OrOperation(element1, element2));
                            break;
                        }
                    case "-":
                        {
                            var element = stack.Pop();
                            stack.Push(NotOperation(element));
                            break;
                        }
                    default:
                        {
                            stack.Push(terms[i]);
                            break;
                        }
                }
            }
            if (stack.Count == 1)
            {
                try
                {
                    var result = stack.Pop();
                    return result.Any(char.IsLetter) ? string.Join(' ', FindLineWithTerm(result).Split(' ').Skip(2)) : result;
                }
                catch (NullReferenceException)
                {
                    return "";
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
