using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ATM
{
    class User
    {
        public string Name { get; set; }
        public string Pin { get; set; }
        public double Balance { get; set; }
        public bool IsAdmin { get; set; }
        public User(string[] data)
        {
            Name = data[0];
            Pin = data[1];
            Balance = double.Parse(data[2]);
            IsAdmin = data[3] == "True";
        }
    }

    internal class Program
    {
        static List<User> users = new();
        static void Main(string[] args)
        {
            // Styling
            Console.Title = "ATM";
            Console.ForegroundColor = ConsoleColor.Yellow;

            // Getting the users
            using (var sr = new StreamReader("users.txt"))
                while (!sr.EndOfStream) users.Add(new User(sr.ReadLine().Split(";")));

            // Greeting
            Console.WriteLine("Welcome to the ATM! Please enter your pin. If you don't have one yet enter -1!");
            string input = Console.ReadLine();
            if (input == "-1") input = CreateUser();

            while (!(users.Where(a => a.Pin == input).Count() == 1))
            {
                Console.WriteLine("Welcome to the ATM! Please enter your pin. If you don't have one yet enter -1!");
                input = Console.ReadLine();
                if (input == "-1") input = CreateUser();
            }
            User user = users.FirstOrDefault(a => a.Pin == input);

            // Operating
            Console.WriteLine($"Hello {user.Name}! What do you want to do?\n[1] Deposit money to your account\n[2] Withdraw money from your account\n[3] Check your balance\n[4]Delete account");
            if (user.IsAdmin) Console.WriteLine($"[5] Check all users\n[6] Delete a user");
            Console.WriteLine("\n[exit] Exit from the ATM");
            string option = Console.ReadLine();
            while (option.ToLower() != "exit")
            {
                switch (option)
                {
                    case "1":
                        {
                            Console.WriteLine("How much your want to deposit?");
                            int amount = int.TryParse(Console.ReadLine(), out amount) ? amount : 0;
                            while (amount == 0)
                            {
                                Console.WriteLine("Please enter a valid amount!");
                                amount = int.TryParse(Console.ReadLine(), out amount) ? amount : 0;
                            }
                            user.Balance += amount;
                            Save();
                            Console.WriteLine($"Money deposited successfully. Your current Balance is {user.Balance}");
                            break;
                        }
                    case "2":
                        {
                            Console.WriteLine("How much your want to withdraw?");
                            int amount = int.TryParse(Console.ReadLine(), out amount) ? amount : 0;
                            while (amount == 0 || user.Balance < amount)
                            {
                                Console.WriteLine($"Please enter a valid amount! (Current balance is: {user.Balance})");
                                amount = int.TryParse(Console.ReadLine(), out amount) ? amount : 0;
                            }
                            user.Balance -= amount;
                            Save();
                            Console.WriteLine($"Your current Balance is {user.Balance}");
                            break;
                        }
                    case "3":
                        {
                            Console.WriteLine($"Your current Balance is {user.Balance}");
                            break;
                        }
                    case "4":
                        {
                            Console.Write("Are you sure you want to delete your account? (y/n) ");
                            string res = Console.ReadLine();
                            if (res =="y")
                            {
                                users.Remove(user);
                                Save();
                                Console.WriteLine("Account deleted successfully");
                                Environment.Exit(0); 
                            }
                            else
                            {
                                Console.WriteLine("Account deletion stopped!");
                            }
                            break;
                        }
                    case "5":
                        {
                            if (!user.IsAdmin) break;

                            foreach (var u in users)
                            {
                                Console.WriteLine($"{u.Name}    {u.Pin}    {user.Balance.ToString("c")}");
                            }
                            break;
                        }
                    case "6":
                        {
                            if (!user.IsAdmin) break;
                            Console.WriteLine("Who you want to delete?");
                            int i=0;
                            foreach (var u in users)
                            {
                                Console.WriteLine($"[{i++}]{u.Name}    {u.Pin}    {user.Balance.ToString("c")}");
                            }
                            int res = int.TryParse(Console.ReadLine(), out res)? res: -1;
                            while (res == -1)
                            {
                                Console.WriteLine("Please enter a valid ID");
                                res = int.TryParse(Console.ReadLine(), out res)? res: -1;
                            }
                            try
                            {
                                users.RemoveAt(res);
                                Save();
                                Console.WriteLine("Account Deleted Successfully");
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Could not delete the requested account");
                            }
                            break;
                        }
                    default:
                        break;
                }
                Console.WriteLine($"What do you want to do next?\n[1] Deposit money to your account\n[2] Withdraw money from your account\n[3] Check your balance\n[4]Delete account");
                if (user.IsAdmin) Console.WriteLine($"[5] Check all users\n[6] Delete a user");
                Console.WriteLine("\n[exit] Exit from the ATM");
                option = Console.ReadLine();
            }
        }

        static void Save()
        {
            using (var sw = new StreamWriter("users.txt"))
                foreach (var item in users) sw.WriteLine($"{item.Name};{item.Pin};{item.Balance};{item.IsAdmin}");
        }

        private static string CreateUser()
        {
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine();

            Console.WriteLine("Enter your pin (4 digits): ");
            string pin = Console.ReadLine();
            while (pin.Length != 4 || !int.TryParse(pin, out int number))
            {
                Console.WriteLine("Please enter a valid pin!");
                pin = Console.ReadLine();
            }
            while (users.Where(a => a.Pin == pin).Count() != 0)
            {
                Console.WriteLine("This pin is already in use!");
                pin = Console.ReadLine();
            }

            using (var sw = File.AppendText("users.txt"))
                sw.WriteLine($"{name};{pin};0;False");

            users.Add(new User(new string[] { name, pin, "0", "False" }));
            return pin;
        }
    }
}