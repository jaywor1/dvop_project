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
        public static int g_limit = 20000;
        public static string token = "414e1f8735fc1b861a890dc790ede63ee357fd9845439a235a195191e79626d7";
        static void Main(string[] args)
        {
            if (LoadData(SAVE_FILE))
            {
                const string admin_token = "414e1f8735fc1b861a890dc790ede63ee357fd9845439a235a195191e79626d7";

                int checkKeyVal = -2;
                int highlighted = 0;

                

                del[] menuFuncs = new del[] { ATMRefil, ManageATM, ManageEmpl, ManageBranches, CreateATMMenu, CreateEmplMenu, CreateBranchMenu, Settings };

                Menu mainMenu = new Menu("Main Menu", HIGHLIGHT_COLOR, DEFAULT_COLOR, new string[] { "List ATMs to refil", "Manage ATMs", "Manage Employees", "Manage Branches", "Create ATM", "Create employee", "Create branch", "Settings" }, menuFuncs);

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

        static async Task Settings()
        {
            BasicMenu settingsMenu = new BasicMenu("Settings", HIGHLIGHT_COLOR, DEFAULT_COLOR, $"Set branch id (current branch_id: {g_branch_id})", $"Set limit (current limit: {g_limit})", "Set token", "Back to Main menu");

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
                    Console.Write("Enter new limit: ");
                    bool parse = int.TryParse(Console.ReadLine(), out g_limit);
                    if (!parse)
                    {
                        Console.WriteLine("[ ERROR ]: Failed to parse quiting ...");
                        Console.WriteLine("Press any key to continue ...");
                        Console.ReadKey();
                        break;
                    }
                    SaveData(SAVE_PATH, SAVE_FILE);
                    break;
                case 2:
                    Console.Write("Enter your token: ");
                    token = Console.ReadLine();
                    SaveData(SAVE_PATH, SAVE_FILE);
                    break;
                case 3:
                    break;

            }

        }
        static async Task ATMRefil()
        {
            ATM[] atms = await GetATM();
            if (atms.Length == 0)
            {
                Console.WriteLine("This branch doesn't have any ATMs");
                Console.WriteLine("Press any key to continue ...");
                Console.ReadLine();
                return;
            }

            ATM[] atmsRefil = new ATM[atms.Length];
            int index = 0;
            for (int i = 0; i < atms.Length; i++)
            {
                if (atms[i].stock <= g_limit)
                {
                    atmsRefil[index] = atms[i];
                    index++;
                }
            }

            if (atmsRefil[0] == null)
            {
                Console.WriteLine("This branch doesn't have any ATMs to refil");
                Console.WriteLine("Press any key to continue ...");
                Console.ReadLine();
                return;
            }


            BasicMenu listATM = new BasicMenu("ATMs", HIGHLIGHT_COLOR, DEFAULT_COLOR, ATMToString(atmsRefil));

            ATM selectedATM = atmsRefil[listATM.ShowInt()];

            BasicMenu atmOptions = new BasicMenu("Branch options", HIGHLIGHT_COLOR, DEFAULT_COLOR, "Refil ATM", "Back to menu");

            switch (atmOptions.ShowInt())
            {
                default:
                    break;
                case 0:
                    int stock = GetIntInput("Enter new stock: ", selectedATM.stock);
                    await UpdateATM(new ATM(selectedATM.atm_id, g_branch_id, stock, selectedATM.address));
                    break;
                case 1:
                    break;
        


            }
        }

        static async Task CreateBranchMenu()
        {
            Console.Write("Address: ");
            string address = Console.ReadLine();
            Console.Write("Open hours (format HH:MM:SS): ");
            string openHours = Console.ReadLine();
            Console.Write("Close hours (format HH:MM:SS): ");
            string closeHours = Console.ReadLine();

            Branch branch = new Branch(0, openHours, closeHours, address);

            await PutBranch(branch);
            
        }

        static async Task CreateATMMenu()
        {
            Console.Write("Stock: ");
            int stock;
            bool parse = int.TryParse(Console.ReadLine(), out stock);
            if (!parse)
            {
                Console.WriteLine("[ ERROR ]: Parse error quiting");
                Console.WriteLine("Press any key to continue ...");
                Console.ReadLine();
            }

            Console.Write("Address: ");
            string address = Console.ReadLine();

            ATM atm = new ATM(0, g_branch_id, stock, address);

            await PutAtm(atm);

        }


        static async Task CreateEmplMenu()
        {
            Console.Write("Full name: ");
            string name = Console.ReadLine();
            Console.Write("Position: ");
            string position = Console.ReadLine();

            Employe empl = new Employe(0, g_branch_id, name, position, true);

            await PutEmpl(empl);
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
                    sw.WriteLine($"limit:{g_limit}");
                    sw.WriteLine($"token:{token}");
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

            if(lines.Length != 3)
            {
                SaveData(SAVE_PATH, SAVE_FILE);
                return true;
            }

            // Parsing branch_id
            bool parse = int.TryParse(lines[0].Substring("branch_id:".Length), out g_branch_id);

            if (!parse)
                return false;

            parse = int.TryParse(lines[1].Substring("limit:".Length), out g_limit);

            if (!parse)
                return false;

            token = lines[2].Substring("token:".Length);

            return true;

        }

        static async Task ManageEmpl()
        {
            Employe[] employes = await GetEmployees();


            string[] names = new string[employes.Length];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = employes[i].name + ", " + employes[i].position;
            }

            BasicMenu listEmpl = new BasicMenu("Employees", HIGHLIGHT_COLOR, DEFAULT_COLOR, names);

            Employe selectedEmpl = employes[listEmpl.ShowInt()];

            BasicMenu emplOptions = new BasicMenu("Employee options", HIGHLIGHT_COLOR, DEFAULT_COLOR, "Modify employee", "Delete employee", "Back to menu");

            switch (emplOptions.ShowInt())
            {
                default:
                    break;
                case 0:
                    Console.WriteLine("YOU CAN PRESS ENTER FOR DEFAULT");
                    Employe empl = CreateEmpl(employes, selectedEmpl.employe_id);
                    await UpdateEmpl(empl);
                    break;
                case 1:
                    Console.WriteLine($"Do you want to delete {selectedEmpl.name}? (type 'yes' to confirm)");
                    string s = Console.ReadLine();
                    if(s == "yes")
                    {
                        DeleteEmpl(selectedEmpl);
                    }
                    else
                    {
                        Console.WriteLine("Employee wasn't deleted\nPress any key to continue ...");
                        Console.ReadKey();
                    }
                    break;
                case 2:
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

        static async Task ManageATM()
        {
            ATM[] atms = await GetATM();

            if(atms.Length == 0)
            {
                Console.WriteLine("This branch doesn't have any ATMs");
                Console.WriteLine("Press any key to continue ...");
                Console.ReadLine();
                return;
            }

            BasicMenu listATM = new BasicMenu("ATMs", HIGHLIGHT_COLOR, DEFAULT_COLOR, ATMToString(atms));

            ATM selectedATM = atms[listATM.ShowInt()];

            BasicMenu atmOptions = new BasicMenu("Branch options", HIGHLIGHT_COLOR, DEFAULT_COLOR, "Modify ATM", "Delete ATM", "Refil ATM", "Back to menu");

            switch (atmOptions.ShowInt())
            {
                default:
                    break;
                case 0:
                    Console.WriteLine("YOU CAN PRESS ENTER FOR DEFAULT");
                    ATM atm = CreateATM(atms, selectedATM.atm_id);
                    await UpdateATM(atm);
                    break;
                case 1:
                    await DeleteAtm(selectedATM);
                    break;
                case 2:
                    int stock = GetIntInput("Enter new stock: ", selectedATM.stock);
                    await UpdateATM(new ATM(selectedATM.atm_id, g_branch_id, stock, selectedATM.address));
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
        public static string[] ATMToString(ATM[] atms)
        {
            string[] arr = new string[atms.Length];

            int lId = 0;
            int lAddress = 0;
            int lStock = 0;

            foreach (var atm in atms)
            {
                if (atm == null)
                    break;
                if (atm.atm_id.ToString().Length > lId)
                    lId = atm.atm_id.ToString().Length;
                if (atm.address.Length > lAddress)
                    lAddress = atm.address.Length;
                if (atm.stock.ToString().Length > lStock)
                    lStock = atm.stock.ToString().Length;
            }


            for (int i = 0; i < atms.Length; i++)
            {
                if (atms[i] == null)
                    break;
                arr[i] = String.Format($"| {{0,{lId}}} | {{1,{lAddress}}} | {{2,{lStock}}} |", atms[i].atm_id, atms[i].address, atms[i].stock);
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

        public static string GetStringInput(string dialog, string defaultParameter)
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
        public static int GetIntInput(string dialog, int defaultParameter)
        {
            Console.Write(dialog);
            int val;
            string iS = Console.ReadLine();
            bool parse = int.TryParse(iS, out val);
            if (iS == "")
                val = defaultParameter;
            else if (!parse)
            {
                Console.WriteLine("[ ERROR ]: Error in parsing using default");
                val = defaultParameter;
            }
                
            return val;
        }

        public static Branch CreateBranch(Branch[] branches, int id)
        {

            Branch selectedBranch = branches.SingleOrDefault(x => x.branch_id == id);

            string address = GetStringInput("address: ", selectedBranch.address);
            string open_hours = GetStringInput("format (HH:MM:SS)\nopen_hours: ", selectedBranch.open_hours);
            string close_hours = GetStringInput("format (HH:MM:SS)\nclose_hours: ", selectedBranch.close_hours);

            

            return new Branch(selectedBranch.branch_id, open_hours, close_hours, address);

        }
        public static ATM CreateATM(ATM[] atms, int id)
        {

            ATM selectedATM = atms.SingleOrDefault(x => x.atm_id == id);

            int stock = GetIntInput("stock: ", selectedATM.stock);
            string address = GetStringInput("address: ", selectedATM.address);

            return new ATM(selectedATM.atm_id, g_branch_id, stock, address);

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

        static async Task UpdateATM(ATM atm)

        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage res = await client.PutAsJsonAsync($"atm/{atm.atm_id}?api_key={token}", atm);

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("PUT " + atm.address + "\nPress ENTER to continue...");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("FAILED TO CHANGE " + atm.address + "\nPress ENTER to continue...");
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


                HttpResponseMessage res = await client.PutAsJsonAsync("employe?api_key=" + token, employe);
               
                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("PUT " + employe.name + "\nPress ENTER to continue...");
                    Console.ReadLine();
                }

            }
        }

        static async Task PutBranch(Branch branch)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage res = await client.PutAsJsonAsync("branch?api_key=" + token, branch);

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("PUT " + branch.address + "\nPress ENTER to continue...");
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


                HttpResponseMessage res = await client.PutAsJsonAsync($"employe/{id}?api_key={token}", employe);

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("PUT " + employe.name + "\nPress ENTER to continue...");
                    Console.ReadLine();
                }

            }
        }

        static async Task DeleteEmpl(Employe employe)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string id = employe.employe_id.ToString();


                HttpResponseMessage res = await client.DeleteAsync($"employe/{id}?api_key={token}");

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("Deleted " + employe.name + "\nPress any key to continue...");
                    Console.ReadKey();

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


        public static async Task<ATM[]> GetATM()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.WriteLine("GET");
                HttpResponseMessage response = await client.GetAsync($"atm/{g_branch_id}?api_key={token}");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    ATM[] atms = JsonSerializer.Deserialize<ATM[]>(json);

                    return atms;

                }
            }
            return null;
        }




        static async Task PutAtm(ATM atm)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage res = await client.PutAsJsonAsync("atm?api_key=" + token, atm);

                if (res.IsSuccessStatusCode)
                {
                    Console.WriteLine("Created " + atm.ToString() + "\nPress ENTER to continue...");
                    Console.ReadLine();
                }

            }
        }

        static async Task DeleteAtm(ATM atm)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:3000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Console.WriteLine($"Type in 'yes' to confirm deletion of ATM at {atm.address}");
                if(Console.ReadLine() == "yes")
                {

                    HttpResponseMessage res = await client.DeleteAsync($"atm/{atm.atm_id}?api_key={token}");

                    if (res.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Deleted " + atm.address);
                    }
                }
                else
                {
                    Console.WriteLine($"ATM at {atm.address} wasn't deleted");
                }
                Console.WriteLine("Press any key to continue ...");
                Console.ReadLine();


            }
        }

    }
}