using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
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
            del delGetAtmRefil = PostAtmRefil;
            del delDeleteAtm = DeleteAtm;

            Menu menu = new Menu("ATM", HIGHLIGHT_COLOR, DEFAULT_COLOR, new string[] { "List ATMs", "Create ATM", "List ATMs that need refil", "Delete ATM" }, delGetAtm, delPutAtm, delGetAtmRefil, delDeleteAtm);
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
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.Write("Enter stock: ");
                int stock = int.Parse(Console.ReadLine());
                Console.Write("Enter address: ");
                string address = Console.ReadLine();

                ATM atm = new ATM(0, stock, address, false);

                HttpResponseMessage res = await client.PutAsJsonAsync("atm?api_key=" + token, atm);

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("Created " + atm.ToString() + "\nPress ENTER to continue...");
                    Console.ReadLine();
                }

            }
        }

        static async Task DeleteAtm()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.Write("Enter ATM id: ");
                string atm_id = Console.ReadLine();

                HttpResponseMessage res = await client.DeleteAsync($"atm/{atm_id}?api_key={token}");

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("Deleted " + atm_id + "\nPress ENTER to continue...");
                    Console.ReadLine();
                }

            }
        }

        static async Task PostAtmRefil()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.Write("Limit: ");
                string limit = Console.ReadLine();

                Console.WriteLine("POST");

                var content = new StringContent(JsonSerializer.Serialize(new { limit = limit }), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("atm/refil?api_key=" + token, content);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    ATM[] atm = JsonSerializer.Deserialize<ATM[]>(json);

                    Console.WriteLine("RESULTS\n");

                    for (int i = 0; i < atm.Length; i++)
                    {
                        Console.WriteLine($"{atm[i].atm_id} | {atm[i].stock}  | {atm[i].error} | {atm[i].address}");
                    }
                    Console.WriteLine("\nPress ENTER to continue...");
                    Console.ReadLine();

                }
                else
                {
                    Console.WriteLine("ERROR");
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

        /*
        public static void CreateTable(object[] obj)
        {
            PropertyInfo[] props = obj[0].GetType().GetProperties();


            for (int i = 0; i < obj.Length; i++)
            {
                for (int y = 0; y < props.Length; y++)
                {
                    string 
                }
            }

        }
        */
    }
}