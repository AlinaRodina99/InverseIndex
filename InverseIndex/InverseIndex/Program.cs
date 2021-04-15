using System;
using System.Text;

namespace InverseIndex
{
    public class Program
    {
        private static string RemoveSpecialCharacters(string str)
        {
            var newString = new StringBuilder();
            foreach (char character in str)
            {
                if ((character >= 'A' && character <= 'Z') || (character >= 'a' && character <= 'z'))
                {
                    newString.Append(character);
                }
            }
            return newString.ToString();
        }

        public static void Main()
        {

            var indexer = new Indexer();
            indexer.Start();
            //Console.WriteLine('æ' <= 'z' && 'æ' >= 'a');
            //Console.WriteLine($"{Convert.ToUInt16('æ')}");
            //Console.WriteLine($"{Convert.ToUInt16('a')}");
            //Console.WriteLine($"{Convert.ToUInt16('z')}");
            //Console.WriteLine($"{RemoveSpecialCharacters("æææææ")}");
        }
    }
}
