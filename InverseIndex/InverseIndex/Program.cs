using System;
using System.IO;
using Porter2Stemmer;

namespace InverseIndex
{
    public static class Program
    {
        public static void Main()
        {
            var path1 = "C:\\Users\\HP\\inverse_index\\InverseIndex\\InverseIndex\\TestFiles";
            var path2 = "C:\\Users\\HP\\inverse_index\\InverseIndex\\InverseIndex\\Terms";
            var path3 = "C:\\Users\\HP\\inverse_index\\InverseIndex\\InverseIndex\\Index\\index.txt";
            var stemmer = new Stemmer(path1, path2);
            stemmer.GetLemmas();
            var spimi = new Spimi(path2, path3);
            spimi.BuildIndex();
        }
    }
}
