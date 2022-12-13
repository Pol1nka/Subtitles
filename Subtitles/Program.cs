using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Subtitles
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // string inp = "privet kak dela sessia sdana";
            // SubCreator.GetPhrase(inp);
            string[] initialStrings = File.ReadAllLines("subs.txt");
            SubtitlesLoader[] subtitles = new SubtitlesLoader[initialStrings.Length];
            for (int i = 0; i < initialStrings.Length; i++)
            {
                subtitles[i] = SubCreator.CreateSubtitle(initialStrings[i]);
            }

            SubtitleOutputer shower = new SubtitleOutputer(subtitles);
            shower.BeignWork();

            Console.ReadLine();
            
        }
    }

    public class SubtitlesLoader
    {
        public int TimeStart { get; }
        public int TimeEnd { get; }
        public string Position { get; }
        public string Phrase { get; }
        public ConsoleColor Color { get; }

        public SubtitlesLoader(int timeStart, int timeEnd, string position, string phrase, ConsoleColor color)
        {
            TimeStart = timeStart;
            TimeEnd = timeEnd;
            Position = position;
            Phrase = phrase;
            Color = color;
        }
    }

    class SubCreator
    {
        public static SubtitlesLoader CreateSubtitle(string initialString)
        {
            int startTime = GetTimeStart(initialString);
            int endTime = GetTimeEnd(initialString);
            string position = GetPosition(initialString);
            string phrase = GetPhrase(initialString);
            ConsoleColor color = GetColor(initialString);

            return new SubtitlesLoader(startTime, endTime, position, phrase, color);
        }

         public static int GetTimeStart(string initialString)
         {
             int startTime = int.Parse(initialString.Split('-')[0].Split(' ')[0].Split(':')[1]);
             return startTime;
         }

         public static int GetTimeEnd(string initialString)
         {
             int endTime = int.Parse(initialString.Split('-')[1].Split(' ')[1].Split(':')[1]);
             return endTime;
         }

        public static string GetPosition(string initialString)
        {
            string position = "";
            if (initialString.Contains("["))
                position = initialString.Split('[')[1].Split(',')[0];
            else
            {
                position = "Bottom";
            }

            return position;
        }

        public static ConsoleColor GetColor(string initialString)
        {
            ConsoleColor color;
            string colors = "";
            if (initialString.Contains("]"))
                colors = initialString.Split(']')[0].Split(',')[1];
            else
            {
                color = ConsoleColor.White;
            }

            switch (colors)
            {
                case "Red":
                    color = ConsoleColor.Red;
                    break;
                case "Green":
                    color = ConsoleColor.Green;
                    break;
                case "Blue":
                    color = ConsoleColor.Blue;
                    break;
                default:
                    color = ConsoleColor.White;
                    break;
            }

            return color;
        }

        public static string GetPhrase(string initialString)
        {
            string text = "";
            if (initialString.Contains("[") || initialString.Contains("]"))
                text = initialString.Split(']')[1];
            else
            {
                string[] phrases = initialString.Split(' ');

                List<string> words = phrases.ToList();
                words.RemoveAt(0);
                words.RemoveAt(0);
                words.RemoveAt(0);

                text = string.Join(" ", words);
            }

            return text;
        }
    }

    public class SubtitleOutputer
    {
        private static int runTime;
        private SubtitlesLoader[] subtitles;
        public SubtitleOutputer(SubtitlesLoader[] subtitles)
        {
            this.subtitles = subtitles;
        }

        public void BeignWork()
        {
            TimerCallback timerCallback = new TimerCallback(Test);
            Timer timer = new Timer(timerCallback, subtitles, 0, 1000);
        }

        private static void Test(object obj)
        {
            SubtitlesLoader[] input = (SubtitlesLoader[]) obj;  
            foreach(SubtitlesLoader subtit in input)
            {
                if (subtit.TimeStart == runTime) 
                    SubtitleConsole(subtit);
                else if (subtit.TimeEnd == runTime) 
                    RemoveSubtitle(subtit);
            }

            runTime++;
        }

        private static void SubtitleConsole(SubtitlesLoader subtit)
        {
            SetPosition(subtit);
            Console.ForegroundColor = subtit.Color;
            Console.Write(subtit.Phrase);
        }

        private static void RemoveSubtitle(SubtitlesLoader subtit)
        {
            SetPosition(subtit);
            for (int i = 0; i < subtit.Phrase.Length; i++)
                Console.Write(" ");
        }

        private static void SetPosition(SubtitlesLoader subtit)
        {
            switch (subtit.Position)
            {
                case "Top":
                    Console.SetCursorPosition((Console.WindowWidth - subtit.Phrase.Length) / 2, 1);
                    break;
                case "Right":
                    Console.SetCursorPosition(Console.WindowWidth - subtit.Phrase.Length, (Console.WindowHeight - 1) / 2);
                    break;
                case "Bottom":
                    Console.SetCursorPosition((Console.WindowWidth - subtit.Phrase.Length) / 2, Console.WindowHeight);
                    break;
                case "Left":
                    Console.SetCursorPosition(0, (Console.WindowHeight - 1) / 2);
                    break;
                default:
                    break;
            }

        }
    }
}

