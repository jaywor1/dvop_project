using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console_client
{
    // Love this class compared to that <not_found> Menu
    internal class BasicMenu
    {
        public string name;
        public ConsoleColor highlightCol;
        public ConsoleColor defaultCol;
        public string[] options;

        public BasicMenu(string name, ConsoleColor highlightCol, ConsoleColor defaultCol, params string[] options)
        {
            this.name = name;
            this.highlightCol = highlightCol;
            this.defaultCol = defaultCol;
            this.options = options;
        }
        public int ShowInt()
        {
            // Simplicity I seek and I find in you <3
            int highlighted = 0;
            int checkKeyVal = -2;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"-------------- {name} --------------");
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == highlighted)
                    {
                        Console.ForegroundColor = highlightCol;
                        Console.WriteLine($"--> {options[i]}");
                        Console.ForegroundColor = defaultCol;
                    }
                    else
                    {
                        Console.WriteLine(options[i]);
                    }
                }

                checkKeyVal = checkKey(Console.ReadKey());

                if (checkKeyVal == 0)
                {
                    break;
                }
                else if (checkKeyVal == -2)
                {
                    continue;
                }
                else
                {
                    if (highlighted + checkKeyVal < 0 || highlighted + checkKeyVal >= options.Length)
                    {
                        continue;
                    }
                    highlighted += checkKeyVal;
                }
            }

            Console.Clear();

            for (int i = 0; i < options.Length; i++)
            {
                if (i == highlighted)
                {
                    return i;
                    break;
                }
            }

            return -1;
        }
        private int checkKey(ConsoleKeyInfo cki)
        {
            switch (cki.Key)
            {
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    return 1;
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    return -1;
                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    return 0;
                default:
                    return -2;
            }
        }
    }
}
