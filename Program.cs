using System;
using System.Collections;
using System.IO;

namespace RecSum
{
    class Program
    {
        static void Main(string[] args)
        {
            
            ValidCommand command;
            //Create Help Message.
            string[] msg = CreateMsg();

            //Judge command value and execute related operation.
            if (Enum.TryParse<ValidCommand>(args[0], out command))
            {
                switch (command)
                {
                    case ValidCommand.record:
                        //If all input is double type, record values into text file.
                        if (isNums(args)) {
                            RecordNums(args);
                        } else {
                            PrintHelp(msg);
                        }
                        break;
                    case ValidCommand.summary:
                        //If file exists, calculate and print summary.
                        if (File.Exists(args[1])){
                            SumNums(args[1]);
                        } else {
                            PrintHelp(msg);
                        }
                        break;
                    case ValidCommand.help:
                        PrintHelp(msg);
                        break;
                }
            } else {
                // If command is invalid, print help message.
                PrintHelp(msg);
            }
        }

        static string[] CreateMsg()
        {
            ArrayList msg = new ArrayList();
            msg.Add("");
            msg.Add("This program is a console application based on .Net.");
            msg.Add("");
            msg.Add("To run this program:");
            msg.Add(@"1. Navigate to the program directory and run 'dotnet restore'.");
            msg.Add(@"2. Run 'dotnet run [command] [filepath] [numbers]'");
            msg.Add("");
            msg.Add("Command options: \n\t\trecord \n\t\tsummary \n\t\thelp");
            msg.Add("Note: \n\t\tNumbers should be double type.");
            msg.Add("\t\tIf any invalid command or options is typed, help message will be printed.");

            return msg.ToArray(typeof(string)) as string[];
        }

        /*
         * Record input numbers.
         * If filepath does not exist, create a new text file and save input numbers.
         * If filepath exists, append input numbers in the text file.
         */
        static void RecordNums(string[] args)
        {
            bool isFileExists = File.Exists(args[1]);
            string path = args[1];

            if (!isFileExists) {
                using (StreamWriter sw = File.CreateText(path))
                {
                    for (int i = 2; i < args.Length; i++)
                    {
                        sw.WriteLine(args[i]);
                    }
                }
            } else {
                using (StreamWriter sw = File.AppendText(path))
                {
                    for (int i = 2; i < args.Length; i++)
                    {
                        sw.WriteLine(args[i]);
                    }
                }
            }
        }
        /*
         * Judge whether all input value is number.
         * If any input value is invalid, print help message.
         */
        static bool isNums(string[] args)
        {
            for (int i = 2; i < args.Length; i++)
            {
                if (Double.TryParse(args[i], out double num)){
                    continue;
                } else {
                    return false;
                }
            }
            return true;
        }

        /*
         * Read numbers from the text file and calculate summary (#, min, max, avg).
         */
        static void SumNums(string path)
        {
            string line;
            double sum = 0;
            int size = 0;
            double min = Double.MaxValue;
            double max = Double.MinValue;

            using (StreamReader sr = File.OpenText(path))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (Double.TryParse(line, out double num))
                    {
                        sum += num;
                        size++;
                        min = min > num ? num : min;
                        max = max < num ? num : max;
                    }
                }
            }
            PrintSummary(size, max, min, sum / size);
        }

        /*
         * Print Help Message.
         */
        static void PrintHelp(string[] msg)
        {
            foreach (string s in msg)
            {
                Console.WriteLine(s);
            }
        }

        /*
         * Print summary information in requested form.
         */
        static void PrintSummary(int size, double max, double min, double avg)
        {
            //Get maximum length from summary and calculate the padding length.
            int maxWidth = Math.Max(size.ToString().Length, Math.Max(max.ToString().Length,
                           Math.Max(min.ToString().Length, avg.ToString().Length)));
            int sizePad = maxWidth - size.ToString().Length;
            int maxPad = maxWidth - max.ToString().Length;
            int minPad = maxWidth - min.ToString().Length;
            int avgPad = maxWidth - avg.ToString().Length;

            string[] summary = new string[6];
            summary[0] = "+--------------+-" + Pad(maxWidth, '-', '+');
            summary[1] = $"| # of Entries | {size}" + Pad(sizePad, ' ', '|');
            summary[2] = $"| Min. value   | {min}" + Pad(minPad, ' ', '|');
            summary[3] = $"| Max. value   | {max}" + Pad(maxPad, ' ', '|');
            summary[4] = $"| Avg. value   | {avg}" + Pad(avgPad, ' ', '|');
            summary[5] = summary[0];

            foreach (string s in summary)
            {
                Console.WriteLine(s);
            }
        }

        /*
         * Adjust each line to same length by adding right padding.
         */
        static string Pad(int num, char pad, char endChar)
        {
            string str = "";
            for (int i = 0; i < num; i++)
            {
                str += pad.ToString();
            }
            str += endChar;
            return str;
        }

        enum ValidCommand
        {
            record,
            summary,
            help
        }
    }
}
