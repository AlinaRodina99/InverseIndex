using System;
using System.Collections.Generic;
using System.IO;
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
        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private string line;

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
        /// <returns>Line containing given term</returns>
        private async Task<string> FindLineWithTerm(string term)
        {
            var streamReader = new StreamReader(pathToIndex);
            await Task.WhenAny(new[] { Cancel(), ReadLineFromFileAsync(streamReader, term), ReadLineFromFileAsync(streamReader, term), 
                ReadLineFromFileAsync(streamReader, term), ReadLineFromFileAsync(streamReader, term) });
            return line;
        }

        /// <summary>
        /// Cancels cancellation token
        /// </summary>
        /// <returns></returns>
        private Task Cancel() => Task.Run(() => cancellationToken.Cancel());

        private async Task ReadLineFromFileAsync(StreamReader streamReader, string term)
        {
            try
            {
                using (streamReader)
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var currentLine = await streamReader.ReadLineAsync();
                        if (currentLine.Contains(term))
                        {
                            var terms = currentLine.Split(' ');
                            if (terms[0] == term)
                            {
                                line = currentLine;
                                cancellationToken.Cancel();
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private void AndOperation(string term1, string term2)
        {

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
                            var term1 = stack.Pop();
                            var term2 = stack.Pop();

                            stack.Push();
                            break;
                        }
                    case "|":
                        {
                            var term1 = stack.Pop();
                            var term2 = stack.Pop();
                            break;
                        }
                    case "-":
                        {
                            var term = stack.Pop();
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
