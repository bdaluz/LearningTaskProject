using Services.Models;

namespace ProjetoTasks.Menus
{
    internal class UserMenu(IUserService usersvc)
    {
        public async Task<int> Menu()
        {

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Menu:");
                Console.WriteLine("1 - Login");
                Console.WriteLine("2 - Create account");
                Console.WriteLine("3 - Reset password");

                Console.Write("\nSelect an option: ");
                try
                {
                    int choice = int.Parse(Console.ReadLine());
                    Console.WriteLine();
                    switch (choice)
                    {
                        case 1:
                            User user = await Login();
                            if (user != null)
                            {
                                Console.WriteLine("\nLogged in\n");
                                return user.Id;
                            }
                            break;
                        case 2:
                            User userlogged = await Signup();
                            if (userlogged != null)
                            {
                                Console.WriteLine("\nLogged in\n");
                                return userlogged.Id;
                            }
                            break;
                        case 3:
                            await ResetUserPassword();
                            break;
                        default:
                            Console.WriteLine("\nInvalid option.\n");
                            continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public async Task<User?> Login()
        {
            Console.WriteLine("Login");
            Console.Write("Username: ");
            string username = Console.ReadLine().Trim();
            Console.Write("Password: ");
            string password = Console.ReadLine().Trim();

            try
            {
                User user = await usersvc.ValidateLogin(username, password);
                return user;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<User?> Signup()
        {
            Console.WriteLine("Sign up");

            Console.Write("Username: ");
            string username = Console.ReadLine().Trim();

            if (await usersvc.ValidateUsername(username))
            {
                Console.WriteLine("\nUsername already exists.");
                return null;
            }

            Console.Write("Email: ");
            string email = Console.ReadLine().Trim();

            if (await usersvc.ValidateUserEmail(email))
            {
                Console.WriteLine("\nEmail already exists.");
                return null;
            }
            else
            {
                Console.Write("Password: ");
                string password = Console.ReadLine().Trim();

                await usersvc.AddUser(username, email, password);

                User user = await usersvc.ValidateLogin(username, password);
                return user;
            }
        }

        public async Task ResetUserPassword()
        {
            Console.WriteLine("Password reset");

            Console.Write("Enter your email ddress to reset your password: ");
            string email = Console.ReadLine();

            if (!await usersvc.ValidateUserEmail(email))
            {
                Console.WriteLine("\nNo account found with that email address.");
                return;
            }

            await usersvc.SendPasswordResetToken(email);

            Console.Write("Enter the token sent to your email: ");
            string token = Console.ReadLine();
            if (await usersvc.ValidateToken(email, token))
            {
                Console.Write("Enter new password: ");
                string newpass = Console.ReadLine();
                await usersvc.UpdatePassword(email, newpass);
            }
        }

    }
}
