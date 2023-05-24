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

        // Few global variables, so what?

        public static string SAVE_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\data";
        public static string SAVE_FILE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\data\\settings.txt";

        public static int branch_id = 1;
        public static string token = "414e1f8735fc1b861a890dc790ede63ee357fd9845439a235a195191e79626d7";
        static void Main(string[] args)
        {
            if (LoadData(SAVE_FILE))
            {
                const string admin_token = "414e1f8735fc1b861a890dc790ede63ee357fd9845439a235a195191e79626d7";

                int checkKeyVal = -2;
                int highlighted = 0;


                del[] menuFuncs = new del[] { Atm, Empl, Settings };

                Menu mainMenu = new Menu("Main Menu", HIGHLIGHT_COLOR, DEFAULT_COLOR, new string[] { "ATM", "Manage Employees", "Settings" }, menuFuncs);

                while (true)
                {
                    mainMenu.Show();
                    Console.Clear();
                }
            }
            else
            {
                Console.WriteLine("[ ERROR ]: Failed loading data");
                Console.ReadLine();
            }

           

        }


        static async Task Atm()
        {
            del[] atm_funcs = new del[] { GetAtm, PutAtm, PostAtmRefil, DeleteAtm, PatchAtmRefil, PostAtmError, BackToMenu };


            Menu menu = new Menu("ATM", HIGHLIGHT_COLOR, DEFAULT_COLOR, new string[] { "List ATMs", "Create ATM", "List ATMs that need refil", "Delete ATM", "Change ATM stock", "List ATMs with errors", "Back to Main menu" }, atm_funcs);
            menu.Show();


        }

        static async Task Empl()
        {
            del[] empl_funcs = new del[] { GetEmpl, PutEmpl, DeleteEmpl, BackToMenu };

            Menu menu = new Menu("Employee", HIGHLIGHT_COLOR, DEFAULT_COLOR, new string[] { "List employes", "Create employe", "Delete employe", "Back to Main menu" }, empl_funcs);
            menu.Show();
        }

        static async Task Settings()
        {
            BasicMenu settingsMenu = new BasicMenu("Settings", HIGHLIGHT_COLOR, DEFAULT_COLOR, $"Set branch id (current branch_id: {branch_id})", "Back to Main menu");

            int selected = settingsMenu.ShowInt();

            switch (selected)
            {
                default:
                    Console.WriteLine("[ ERROR ]: Unexpected error");
                    break;
                case 0:
                    branch_id = GetBranch("Enter branch id: ");
                    bool save = SaveData(SAVE_PATH, SAVE_FILE);
                    if (save)
                    {
                        Console.WriteLine("[ OK ]: Save successful");
                        Console.WriteLine("Press key to continue ...");
                        Console.ReadKey();
                    }
                        
                    break;
                case 1:
                    break;

            }

        }

        public static int GetBranch(string dialog)
        {
            Console.Write(dialog);

            int branch_id;
            bool parse = int.TryParse(Console.ReadLine(), out branch_id);

            if (!parse)
                return -1;

            return branch_id;
        }
       
        public static bool SaveData(string dirPath, string savePath)
        {
            try
            {
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                

                using (StreamWriter sw = new StreamWriter(savePath, false))
                {
                    sw.WriteLine($"branch_id:{branch_id}");
                    sw.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static bool LoadData(string saveFile)
        {
            string[] lines = File.ReadAllLines(saveFile);

            // Parsing branch_id
            bool parse = int.TryParse(lines[0].Substring("branch_id:".Length), out branch_id);

            if (parse)
                return true;
            else
                return false;

        }
        

        static async Task GetEmpl()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.WriteLine("GET");
                HttpResponseMessage response = await client.GetAsync($"employe/{branch_id}?api_key={token}");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    Employe[] employes = JsonSerializer.Deserialize<Employe[]>(json);

                    Console.WriteLine("RESULTS\n");

                    int lId = employes.Max(x => x.employe_id.ToString().Length);
                    int lName = employes.Max(x => x.name.Length);
                    int lPos = employes.Max(x => x.position.Length);
                    int lBranch = employes.Max(x => x.branch_id.ToString().Length);


                    ConsoleColor defaultCol = Console.ForegroundColor;
                    for (int i = 0; i < employes.Length; i++)
                    {
                        if (employes[i].present)
                            Console.ForegroundColor = ConsoleColor.Green;
                        else
                            Console.ForegroundColor = ConsoleColor.Red;
                        String s = String.Format($"| {{0,{lId}}} | {{1,{lName}}} | {{2,{lPos}}} | {{3,{lBranch}}} |", employes[i].employe_id, employes[i].name, employes[i].position, employes[i].branch_id);
                        Console.WriteLine(s);
                    }
                    Console.ForegroundColor = defaultCol;

                    Console.WriteLine("\nPress ENTER to continue...");
                    Console.ReadLine();

                }
            }
        }
        static async Task PutEmpl()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.Write("Enter name: ");
                string name = Console.ReadLine();
                Console.Write("Enter branch ID: ");
                string branch_id = Console.ReadLine();
                Console.Write("Enter position: ");
                string position = Console.ReadLine();

                Employe employe = new Employe(int.Parse(branch_id), name, position);


                HttpResponseMessage res = await client.PutAsJsonAsync("employe?api_key=" + token, employe);

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("Created " + employe.name + "\nPress ENTER to continue...");
                    Console.ReadLine();
                }

            }
        }

        static async Task DeleteEmpl()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.Write("Enter employe ID: ");
                string id = Console.ReadLine();


                HttpResponseMessage res = await client.DeleteAsync($"employe/{id}?api_key={token}");

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("Deleted " + id + "\nPress ENTER to continue...");
                    Console.ReadLine();

                }

            }
        }

        static async Task MoveEmpl()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.Write("Enter employe ID: ");
                string id = Console.ReadLine();

                HttpResponseMessage res = await client.DeleteAsync($"employe/{id}?api_key={token}");

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("Deleted " + id + "\nPress ENTER to continue...");
                    Console.ReadLine();
                }

            }
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
            Console.WriteLine("Listing ATMs");
            await GetAtm();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.Write("Enter ATM id to delete: ");
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

        static async Task PostAtmError()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.Write("Error: ");
                string error = Console.ReadLine();

                Console.WriteLine("POST");

                var content = new StringContent(JsonSerializer.Serialize(new { error = error }), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("atm/error?api_key=" + token, content);

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

        static async Task PatchAtmRefil()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.Write("ATM ID: ");
                string atm_id = Console.ReadLine();
                Console.Write("New stock: ");
                string stock = Console.ReadLine();


                Console.WriteLine("PATCH");

                var content = new StringContent(JsonSerializer.Serialize(new { atm_id = atm_id, stock = stock }), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PatchAsync("atm/refil?api_key=" + token, content);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();


                    Console.WriteLine("RESULTS\n");

                    Console.WriteLine(json);

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




        static async Task BackToMenu()
        {

        }
    }
}