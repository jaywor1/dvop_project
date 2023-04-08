using System.Net.Http.Headers;
using System.Text.Json;

namespace console_client
{
   // public delegate Task de();

    internal class Program
    {
        public const ConsoleColor HIGHLIGHT_COLOR = ConsoleColor.White;
        public const ConsoleColor DEFAULT_COLOR = ConsoleColor.Gray;
        public static string token = "414e1f8735fc1b861a890dc790ede63ee357fd9845439a235a195191e79626d7";
        static void Main(string[] args)
        {

            const string admin_token = "414e1f8735fc1b861a890dc790ede63ee357fd9845439a235a195191e79626d7";

            int checkKeyVal = -2;
            int highlighted = 0;

            del delAtm = Atm;

            Menu mainMenu = new Menu("Main Menu", HIGHLIGHT_COLOR, DEFAULT_COLOR, new string[] { "ATM" }, delAtm);

            while (true)
            {
                mainMenu.Show();
                Console.Clear();
            }

        }


        static async Task Atm()
        {
            del delGetAtm = GetAtm;
            del delPutAtm = PutAtm;

            Menu menu = new Menu("ATM", HIGHLIGHT_COLOR, DEFAULT_COLOR, new string[] { "List ATMs", "Create ATM" }, delGetAtm, delPutAtm);
            menu.Show();
            
            
        }

 

        static async Task GetAtm()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.WriteLine("GET");
                HttpResponseMessage response = await client.GetAsync("atm?api_key=" + token);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    ATM[] atm = JsonSerializer.Deserialize<ATM[]>(json);

                    Console.WriteLine("RESULTS\n");

                    for (int i = 0; i < atm.Length; i++)
                    {
                        Console.WriteLine($"{atm[i].atm_id} | {atm[i].stock}  | {atm[i].error}");
                    }
                    Console.WriteLine("\nPress ENTER to continue...");
                    Console.ReadLine();

                }
            }
        }




        static async Task PutAtm()
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.Write("Enter stock: ");
                int stock = int.Parse(Console.ReadLine());
                Console.Write("Enter address: ");
                string address = Console.ReadLine();

                ATM atm = new ATM(0, stock, false, address);

                HttpResponseMessage res = await client.PutAsJsonAsync("atm?api_key="+ token, atm);

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("Created " + atm.ToString() + "\nPress ENTER to continue...");
                    Console.ReadLine();
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