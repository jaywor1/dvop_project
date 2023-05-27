using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console_client
{
    // <not_found> is this delegate
    public delegate Task del();

    // <not_found> this class fr better burn it when the time comes 🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥🔥
    public class Menu
    {
        public string name;
        public ConsoleColor highlightCol;
        public ConsoleColor defaultCol;
        public string[] options;
        public del[] tasks;

        public Menu(string name, ConsoleColor highlightCol, ConsoleColor defaultCol, string[] options, del[] tasks)
        {
            this.name = name;
            this.highlightCol = highlightCol;
            this.defaultCol = defaultCol;
            this.options = options;
            this.tasks = tasks;
        }

        public void Show()
        {
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

            for (int i = 0; i < tasks.Length; i++)
            {
                if (i == highlighted)
                {
                    Fix(tasks[i].Invoke());
                    break;
                }
            }

        }

        public int ShowInt()
        {
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

            for (int i = 0; i < tasks.Length; i++)
            {
                if (i == highlighted)
                {
                    return i;
                    break;
                }
            }

            return -1;
        }

        private async Task Fix(Task t)
        {
            t.Wait();
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
