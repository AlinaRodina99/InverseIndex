using System;
using System.IO;

namespace InverseIndex
{
    public static class Program
    {
        public static void Main()
        {
            var path1 = Directory.GetCurrentDirectory() + "/../../../TestFiles";
            var path2 = Directory.GetCurrentDirectory() + "/../../../Terms";
            var stemmer = new Stemmer(path1, path2);
            stemmer.GetLemmas();
            var spimi = new Spimi(path1, $"{Directory.GetCurrentDirectory()} + /../../../Index/index_file.txt");
            spimi.BuildIndex();
        }
    }
}
