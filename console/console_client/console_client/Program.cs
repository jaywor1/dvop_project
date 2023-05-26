using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;

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

        public static int g_branch_id = 1;
        public static string token = "414e1f8735fc1b861a890dc790ede63ee357fd9845439a235a195191e79626d7";
        static void Main(string[] args)
        {
            if (LoadData(SAVE_FILE))
            {
                const string admin_token = "414e1f8735fc1b861a890dc790ede63ee357fd9845439a235a195191e79626d7";

                int checkKeyVal = -2;
                int highlighted = 0;


                del[] menuFuncs = new del[] { Atm, ManageEmpl, ManageBranches, Settings };

                Menu mainMenu = new Menu("Main Menu", HIGHLIGHT_COLOR, DEFAULT_COLOR, new string[] { "ATM", "Manage Employees", "Manage Branches", "Settings" }, menuFuncs);

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
            del[] empl_funcs = new del[] { GetEmpl, Empl, DeleteEmpl, BackToMenu };

            Menu menu = new Menu("Employee", HIGHLIGHT_COLOR, DEFAULT_COLOR, new string[] { "List employes", "Create employe", "Delete employe", "Back to Main menu" }, empl_funcs);
            menu.Show();
        }

        static async Task Settings()
        {
            BasicMenu settingsMenu = new BasicMenu("Settings", HIGHLIGHT_COLOR, DEFAULT_COLOR, $"Set branch id (current branch_id: {g_branch_id})", "Back to Main menu");

            int selected = settingsMenu.ShowInt();

            switch (selected)
            {
                default:
                    Console.WriteLine("[ ERROR ]: Unexpected error");
                    break;
                case 0:
                    g_branch_id = GetBranch("Enter branch id: ");
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
                    sw.WriteLine($"branch_id:{g_branch_id}");
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
            bool parse = int.TryParse(lines[0].Substring("branch_id:".Length), out g_branch_id);

            if (parse)
                return true;
            else
                return false;

        }

        static async Task ManageEmpl()
        {
            Employe[] employes = await GetEmployees();


            string[] names = new string[employes.Length];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = employes[i].name;
            }

            BasicMenu listEmpl = new BasicMenu("Employees", HIGHLIGHT_COLOR, DEFAULT_COLOR, names);

            int selectedId = employes[listEmpl.ShowInt()].employe_id;

            BasicMenu emplOptions = new BasicMenu("Employee options", HIGHLIGHT_COLOR, DEFAULT_COLOR, "Modify employee", "Delete employee", "Back to menu");

            switch (emplOptions.ShowInt())
            {
                default:
                    break;
                case 0:
                    Console.WriteLine("YOU CAN PRESS ENTER FOR DEFAULT");
                    Employe empl = CreateEmpl(employes, selectedId);
                    await UpdateEmpl(empl);
                    break;


            }
        }

        static async Task ManageBranches()
        {
            Branch[] branches = await GetBranches();



            BasicMenu listBranch = new BasicMenu("Branches", HIGHLIGHT_COLOR, DEFAULT_COLOR, BranchToString(branches));

            Branch selectedBranch = branches[listBranch.ShowInt()];

            BasicMenu branchOptions = new BasicMenu("Branch options", HIGHLIGHT_COLOR, DEFAULT_COLOR, "Modify branch", "Delete branch", "Back to menu");

            switch (branchOptions.ShowInt())
            {
                default:
                    break;
                case 0:
                    Console.WriteLine("YOU CAN PRESS ENTER FOR DEFAULT");
                    Branch branch = CreateBranch(branches, selectedBranch.branch_id);
                    await UpdateBranch(branch);
                    break;
                case 1:
                    Console.WriteLine($"Do you want to delete branch {selectedBranch.address}? (Type 'yes' to confirm)");
                    string ans = Console.ReadLine();
                    if (ans == "yes")
                    {
                        DeleteBranch(selectedBranch.branch_id);
                        Console.WriteLine($"Branch {selectedBranch.address} was deleted\nPress any key to continue...");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("Branch wasn't deleted");
                    }
                    break;
                case 2:
                    break;


            }
        }

        public static string[] BranchToString(Branch[] branches)
        {
            string[] arr = new string[branches.Length];

            int lId = branches.Max(x => x.branch_id.ToString().Length);
            int lOpenHours = branches.Max(x => x.open_hours.Length);
            int lCloseHours = branches.Max(x => x.close_hours.Length);
            int lAddress = branches.Max(x => x.address.Length);


            for (int i = 0; i < branches.Length; i++)
            {
                arr[i] = String.Format($"| {{0,{lId}}} | {{1,{lAddress}}} | {{2,{lOpenHours}}} | {{3,{lCloseHours}}} |", branches[i].branch_id, branches[i].address, branches[i].open_hours, branches[i].close_hours);
            }
            return arr;
        }

        public static Employe CreateEmpl(Employe[] employes, int id)
        {
            Console.Write("branch_id:");

            int branch_id;
            string iBranch = Console.ReadLine();
            if (iBranch == "")
                branch_id = employes.SingleOrDefault(x => x.employe_id == id).branch_id;
            else
            {
                bool parse = int.TryParse(iBranch, out branch_id);
                if (!parse)
                {
                    Console.WriteLine("[ ERROR ]: Parsing error, expecting integer");
                    CreateEmpl(employes, id);
                }

            }

            Console.Write("name:");
            string name = "";
            string iName = Console.ReadLine();
            if (iName == "")
                name = employes.SingleOrDefault(x => x.employe_id == id).name;
            else
                name = iName;

            Console.Write("position:");
            string position = "";
            string iPosition = Console.ReadLine();
            if (iPosition == "")
                position = employes.SingleOrDefault(x => x.employe_id == id).position;
            else
                position = iPosition;

            Console.Write("present (y/n): ");
            bool present = true;
            string iPresent = Console.ReadLine();
            if (iPresent == "")
                present = employes.SingleOrDefault(x => x.employe_id == id).present;
            else
            {
                if(iPresent == "y")
                {
                    present = true;
                }
                else if(iPresent == "n")
                {
                    present = false;
                }
                else
                {
                    Console.WriteLine("[ ERROR ]: User is dumbass");
                    Employe empl = CreateEmpl(employes, id);
                }
               
            }

            return new Employe(employes.SingleOrDefault(x => x.employe_id == id).employe_id, branch_id, name, position, present);

        }

        public static string GetStringBranchInput(string dialog, string defaultParameter)
        {
            Console.Write(dialog);
            string s = "";
            string iS = Console.ReadLine();
            if (iS == "")
                s = defaultParameter;
            else
                s = iS;

            return s;
        }

        public static Branch CreateBranch(Branch[] branches, int id)
        {

            Branch selectedBranch = branches.SingleOrDefault(x => x.branch_id == id);

            string address = GetStringBranchInput("address: ", selectedBranch.address);
            string open_hours = GetStringBranchInput("format (HH:MM:SS)\nopen_hours: ", selectedBranch.open_hours);
            string close_hours = GetStringBranchInput("format (HH:MM:SS)\nclose_hours: ", selectedBranch.close_hours);

            

            return new Branch(selectedBranch.branch_id, open_hours, close_hours, address);

        }
        public static async Task<Employe[]> GetEmployees()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.WriteLine("GET");
                HttpResponseMessage response = await client.GetAsync($"employe/{g_branch_id}?api_key={token}");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    Employe[] employes = JsonSerializer.Deserialize<Employe[]>(json);

                    return employes;

                }
            }
            return null;
        }

        public static async Task<Branch[]> GetBranches()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.WriteLine("GET");
                HttpResponseMessage response = await client.GetAsync($"branch/?api_key={token}");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    Branch[] branches = JsonSerializer.Deserialize<Branch[]>(json);

                    return branches;

                }
            }
            return null;
        }

        static async Task UpdateBranch(Branch branch)
        
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage res = await client.PutAsJsonAsync($"branch/{branch.branch_id}?api_key={token}", branch);

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("PUT " + branch.address + "\nPress ENTER to continue...");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("FAILED TO CHANGE " + branch.address + "\nPress ENTER to continue...");
                    Console.ReadLine();
                }

            }
        }
        static async Task GetEmpl()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.WriteLine("GET");
                HttpResponseMessage response = await client.GetAsync($"employe/{g_branch_id}?api_key={token}");

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
        static async Task PutEmpl(Employe employe)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Console.Write("Enter name: ");
                //string name = Console.ReadLine();
                //Console.Write("Enter branch ID: ");
                //string branch_id = Console.ReadLine();
                //Console.Write("Enter position: ");
                //string position = Console.ReadLine();

                //Employe employe = new Employe(int.Parse(branch_id), name, position);


                HttpResponseMessage res = await client.PutAsJsonAsync("employe?api_key=" + token, employe);
               
                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("PUT " + employe.name + "\nPress ENTER to continue...");
                    Console.ReadLine();
                }

            }
        }

        static async Task UpdateEmpl(Employe employe)
        {
            int id = employe.employe_id;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Console.Write("Enter name: ");
                //string name = Console.ReadLine();
                //Console.Write("Enter branch ID: ");
                //string branch_id = Console.ReadLine();
                //Console.Write("Enter position: ");
                //string position = Console.ReadLine();

                //Employe employe = new Employe(int.Parse(branch_id), name, position);


                HttpResponseMessage res = await client.PutAsJsonAsync($"employe/{id}?api_key={token}", employe);

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("PUT " + employe.name + "\nPress ENTER to continue...");
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

        static async Task DeleteBranch(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage res = await client.DeleteAsync($"branch/{id}?api_key={token}");

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