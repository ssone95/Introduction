using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingWithFiles
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //string combinedPath = Path.Combine(@"C:\Users\nedel\Documents", "Working with files", "test2 something.txt");
            //Console.WriteLine(combinedPath);

            ////File.WriteAllText(combinedPath, "Hello!\n");

            ////string[] words = new string[]
            ////{
            ////    "Hello2!", "Welcome to C#!", combinedPath
            ////};

            ////File.AppendAllLines(combinedPath, words);

            ////string[] readLines = File.ReadAllLines(combinedPath);

            ////foreach(var line in readLines)
            ////{
            ////    Console.WriteLine("Line: " + line);
            ////}


            ////Console.WriteLine();

            //string text = "test 123\n" + File.ReadAllText(combinedPath);

            //string[] lines2 = text.Split('\n').Where(line => !string.IsNullOrEmpty(line)).ToArray();

            //int totalLines = text.Split('\n').Where(line => !string.IsNullOrEmpty(line)).Count();

            //foreach(var line in lines2)
            //{
            //    //if (string.IsNullOrEmpty(line))
            //    //{
            //    //    continue;
            //    //}
            //    Console.WriteLine("Line: " + line);
            //}


            ////Console.WriteLine("Please provide a sentence to write to the file:");

            ////string sentence = Console.ReadLine();

            ////File.AppendAllText(combinedPath, sentence + "\n");

            ////File.AppendAllLines(combinedPath, lines2);
            ///

            string combinedPath = Path.Combine(@"C:\Users\nedel\Documents", "Working with files", "table.csv");
            Console.WriteLine(combinedPath);

            string[] lines = File.ReadAllLines(combinedPath);

            foreach(var line in lines)
            {
                var chars = line.Split(';');
                foreach(var c in chars)
                {
                    Console.Write(c + " ");
                }
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}
