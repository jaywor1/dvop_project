using System.Net.Http.Headers;
using System.Text.Json;

namespace console_client
{
    internal class Program
    {
        public const ConsoleColor HIGHLIGHT_COLOR = ConsoleColor.White;
        public const ConsoleColor DEFAULT_COLOR = ConsoleColor.Gray;
        static void Main(string[] args)
        {
            while (true)
            {

                int checkKeyVal = -2;
                int highlighted = 0;

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Bank Api Client");
                    string[] options = { "ATM", "Branch" };

                    Console.WriteLine("-------------- Menu --------------");
                    for (int i = 0; i < options.Length; i++)
                    {
                        if (i == highlighted)
                        {
                            Console.ForegroundColor = HIGHLIGHT_COLOR;
                            Console.WriteLine($"--> {options[i]}");
                            Console.ForegroundColor = DEFAULT_COLOR;
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

                switch (highlighted)
                {
                    case 0:
                        RunTask("atm").Wait();
                        break;
                    case 1:
                        RunTask("branch").Wait();
                        break;
                }

                Console.Clear();

                Console.WriteLine("\n REQUEST\n-------------------------------------");

                RunTask("atm").Wait();

                Console.WriteLine("\n-------------------------------------\nPress ENTER for new request");
                Console.ReadLine();
            }
        }

        static async Task RunTask(string endpoint)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.WriteLine("GET");
                HttpResponseMessage response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    ATM[] atm = JsonSerializer.Deserialize<ATM[]>(json);

                    for (int i = 0; i < atm.Length; i++)
                    {
                        Console.WriteLine($"{atm[i].atm_id} | {atm[i].stock} | {atm[i].withdraw_log} | {atm[i].error_log}");
                    }

                }
            }
        }

        public static int checkKey(ConsoleKeyInfo cki)
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