using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Pie;

namespace Praktos666
{
    internal class Program
    {
        public static int minPosition = 0;
        public static int maxPosition = 0;
        public static int currentPosition = 0;
        public static int currentCharPosition = 0;
        public static string[] lines;
        public static string filePath;

        static void Main(string[] args)
        {
            Console.WriteLine("Введите путь до файла:");
            filePath = Console.ReadLine();
            lines = File.ReadAllLines(filePath);
            Console.Clear();
            Console.WriteLine("F1 для сохранения вашего файла");
            minPosition = Console.CursorTop;

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }

            maxPosition = Console.CursorTop;
            Console.SetCursorPosition(0, minPosition);
            Keys();
            Console.ReadLine();
        }

        static void Keys()
        {
            var key = Console.ReadKey();
            while (key.Key != ConsoleKey.F1)
            {
                while (key.Key != ConsoleKey.Backspace || !Char.IsLetterOrDigit(key.KeyChar) || key.Key != ConsoleKey.Spacebar)
                {
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            currentPosition--;
                            currentCharPosition = 0;
                            if (currentPosition < 0) currentPosition = maxPosition;
                            break;

                        case ConsoleKey.DownArrow:
                            currentPosition++;
                            currentCharPosition = 0;
                            if (currentPosition > maxPosition) currentPosition = 0;
                            break;

                        case ConsoleKey.LeftArrow:
                            if (currentCharPosition != 0) currentCharPosition--;
                            break;

                        case ConsoleKey.RightArrow:
                            if (lines[currentPosition].Length != currentCharPosition) currentCharPosition++;
                            break;

                        case ConsoleKey.Backspace:
                            UpdateLine(currentPosition, currentCharPosition - 1, key);
                            if (currentCharPosition != 0) currentCharPosition--;
                            break;

                        case ConsoleKey.F1:
                            Save();
                            break;

                        default:
                            UpdateLine(currentPosition, currentCharPosition, key);
                            currentCharPosition++;
                            break;
                    }
                    Console.SetCursorPosition(currentCharPosition, currentPosition + minPosition);
                    key = Console.ReadKey();
                }
            }
        }

        static void Save()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Введите формат, в котором вы хотите сохранить свой файл :3 :");
            string formatPath = Console.ReadLine();
            string[] formatParts = formatPath.Split('.');
            string extension = formatParts[1];

            if (extension == "json")
            {
                List<Cake> objectList = MakeListFromLines();
                using StreamWriter sw = File.CreateText(formatPath);
                sw.WriteLine(JsonConvert.SerializeObject(objectList));
            }
            else if (extension == "xml")
            {
                List<Cake> objectList = MakeListFromLines();
                XmlSerializer xml = new XmlSerializer(objectList.GetType());
                using FileStream fs = new FileStream(formatPath, FileMode.OpenOrCreate);
                xml.Serialize(fs, objectList);
                fs.Dispose();
            }
        }

        static List<Cake> MakeListFromLines()
        {
            List<Cake> objectList = new List<Cake>();
            Cake obj = new Cake();
            foreach (var line in lines)
            {
                if (!int.TryParse(line, out int number)) obj.Name = line;
                else
                {
                    obj.Price = Convert.ToInt32(line);
                    objectList.Add(obj);
                    obj = new Cake();
                }
            }
            return objectList;
        }

        static void UpdateLine(int a, int b, ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Backspace) lines[a] = lines[a].Remove(b, 1);
            else lines[a] = lines[a].Insert(b, Convert.ToString(key.KeyChar));
            Console.SetCursorPosition(0, a + minPosition);
            Console.Write(string.Format("{{0, -{0}}}", Console.BufferWidth), lines[a]);
        }
    }
}