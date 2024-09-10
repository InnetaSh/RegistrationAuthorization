using Microsoft.EntityFrameworkCore;
using RegistrationAuthorization;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;


Account CurrentAccount = new Account();


Menu();


void Menu()
{
    Console.WriteLine("Выберите:");
    int action;


    Console.WriteLine("1 - Войти");
    Console.WriteLine("2 - У вас ещё нет аккаунта? Зарегистрироваться.");

    Console.WriteLine("3 - выход");

    Console.Write("действие - ");
    while (!Int32.TryParse(Console.ReadLine(), out action) || action < 1 || action > 3)
    {
        Console.WriteLine("Не верный ввод.Введите число:");
        Console.Write("действие - ");
    }

    string login = "";
    string password = "";

    switch (action)
    {
        case 1:
            VvodLoginPassword(out login, out password);
            Vhod(login, password);

            Console.WriteLine("Нажмите любую клавишу для выхода в меню...");
            Console.ReadKey();
            Console.Clear();
            Menu();

            break;
        case 2:
            VvodLoginPassword(out login, out password);
            CurrentAccount = Registration(login, password);
            SubMenu(CurrentAccount);

            Thread.Sleep(2000);
            Console.Clear();
            Menu();

            break;
        case 3:
            break;
    }
}



void VvodLoginPassword(out string login, out string password)
{
    Console.WriteLine("Введите логин");
   login = Console.ReadLine();

    Console.WriteLine("Введите пароль");
    password = Console.ReadLine();

}

void Vhod(string login, string password)
{
    string hashedPassword = HashPassword(password);

    using (ApplicationContext db = new ApplicationContext())
    {
        Account? account = db.Accounts.FirstOrDefault(ac => ac.Login == login && ac.Password == hashedPassword);

        if (account != null)
        {

            Console.WriteLine($"пользователь {login} войшел в свою учетную запись .");
        }
        else
        {
            Console.WriteLine("У вас ещё нет аккаунта... Зарегистрироваться.(Y/N)");
            string flag = Console.ReadLine().ToUpper();
            if (flag == "Y")
            {
                VvodLoginPassword(out login, out password);
                CurrentAccount = Registration(login, password);
                SubMenu(CurrentAccount);
            }
        }
    }
}


Account Registration(string login, string password)
{
   

    string hashedPassword = HashPassword(password);
    Account ac1;

    using (ApplicationContext db = new ApplicationContext())
    {
        ac1 = new Account { Login = login, Password = hashedPassword };

        db.Accounts.Add(ac1);
        db.SaveChanges();
    }
    Console.WriteLine($"пользователь {login} успешно зарегистрирован.");

    return ac1;
}


static string HashPassword(string password)
{
    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        
        byte[] hashBytes = sha256.ComputeHash(passwordBytes);

      
        StringBuilder hashStringBuilder = new StringBuilder();
        foreach (byte b in hashBytes)
        {
            hashStringBuilder.Append(b.ToString("x2"));
        }

        return hashStringBuilder.ToString();
    }
}



void SubMenu(Account CurrentAccount)
{
    Console.WriteLine("-------------------------------------");
    Console.WriteLine("Выберите действие с учетной записью :");
    int action;


    Console.WriteLine("1 - посмотреть данные");
    Console.WriteLine("2 - добавить данные");
    Console.WriteLine("3 - удалить данные");
    Console.WriteLine("4 - изменить данные");
    Console.WriteLine("5 - вернуться в меню");

    Console.Write("действие - ");
    while (!Int32.TryParse(Console.ReadLine(), out action) || action < 1 || action > 5)
    {
        Console.WriteLine("Не верный ввод.Введите число:");
        Console.Write("действие - ");
    }
    Console.WriteLine("-------------------------------------");
    string name = "";
    int age = 0;
    int phone = 0;
    switch (action)
    {
        case 1:
            Console.WriteLine("------------------------------------------");
            ShowDB(CurrentAccount);
            Console.WriteLine("------------------------------------------");

            SubMenu(CurrentAccount);
            break;
        case 2:
                GetUserInfo(out name, out age, out phone);
                AddUserInfo(CurrentAccount,name, age,phone);
                Console.WriteLine($"пользователь {name} добавлен в базу данных.");
            
            Thread.Sleep(1000);
            Console.Clear();
            SubMenu(CurrentAccount);
            break;
        case 3:
            DeleteUser(CurrentAccount);
            Thread.Sleep(1000);
            Console.Clear();
            SubMenu(CurrentAccount);
            break;
        case 4:
            while (true)
            {
                SubMenuUpdate(CurrentAccount);


                Console.WriteLine("изменить еще данные о пользователе? (Y/N)");
                string flag = Console.ReadLine().ToUpper();
                if (flag == "N")
                    break;
            }
            Thread.Sleep(1000);
            Console.Clear();
            SubMenu(CurrentAccount);
            break;
        case 5:
              Menu();
            break;

    }
}



void ShowDB(Account account)
{
    using (ApplicationContext db = new ApplicationContext())
    {
        Account? ac = db.Accounts
            .Include(a => a.User)
            .FirstOrDefault(a => a.Login == account.Login && a.Password == account.Password);
        if (ac != null)
        {
            var user = ac.User;
            if (user == null)
            {
                Console.WriteLine("Данные не заполнены.");
            }
            else
            {
                if (string.IsNullOrEmpty(user.Name))
                {
                    Console.WriteLine("Данные об имени не заполнены.");
                }
                else
                {
                    Console.WriteLine($"  {user.Id}.{user.Name,-20} - {user.Age} - {user.Phone}");
                }
            }
        }
        else
        {
            Console.WriteLine("Аккаунт не найден.");
        }
        
    }
}
void GetUserInfo(out string name, out int age, out int phone)
{

    Console.WriteLine("Введите имя: ");
    Console.Write("Имя - ");
    name = Console.ReadLine();

    Console.WriteLine("Введите возраст: ");
    Console.Write("возраст - ");
    while (!Int32.TryParse(Console.ReadLine(), out age) || age < 1 || age > 100)
    {
        Console.WriteLine("Не верный ввод.Введите число:");
        Console.Write("возраст - ");
    }

    Console.WriteLine("Введите телефон: ");
    Console.Write("номер телефона - ");
    while (!Int32.TryParse(Console.ReadLine(), out phone) || phone.ToString().Length > 13)
    {
        Console.WriteLine("Не верный ввод.Введите номер телефона:");
        Console.Write("номер телефона - ");
    }
}

void AddUserInfo(Account account,string name, int age, int phone)
{
    using (ApplicationContext db = new ApplicationContext())
    {
        Account? ac = db.Accounts.FirstOrDefault(a => a.Login == account.Login && a.Password == account.Password);
        if (ac != null)
        {
            User us1 = new User { Name = name, Age = age, Phone = phone };
            db.Users.Add(us1);
            ac.User = us1;
            db.SaveChanges();
        }
        else
        {
            Console.WriteLine("Аккаунт не найден.");
        }

    }

}

void DeleteUser(Account account)
{
    using (ApplicationContext db = new ApplicationContext())
    {
        Account? ac = db.Accounts.FirstOrDefault(a => a.Login == account.Login && a.Password == account.Password);

        if (ac != null)
        {
            ac.User = new User { Name = "", Age = 0, Phone = 0 };
            
            db.SaveChanges();
            Console.WriteLine($"пользователь {account.Login} удален из базы данных.");
        }
        else
            Console.WriteLine("Аккаунт не найден.");
    }
}
void SubMenuUpdate(Account account)
{
    using (ApplicationContext db = new ApplicationContext())
    {
        Account? ac = db.Accounts.FirstOrDefault(a => a.Login == account.Login && a.Password == account.Password);

        if (ac != null)
        {
            Console.WriteLine("Выберите какие данные изменить:");

            Console.WriteLine("1 -  все данные о пользователе");
            Console.WriteLine("2 -  имя пользователя");
            Console.WriteLine("3 -  возраст пользователя");
            Console.WriteLine("4 -  номер телефона пользователя");
            Console.WriteLine("5 - выход в меню");

            int numdate;
            Console.Write("изменить данные о  ");
            while (!Int32.TryParse(Console.ReadLine(), out numdate) || numdate < 1 || numdate > 5)
            {
                Console.WriteLine("Не верный ввод.Введите число:");
                Console.Write("изменить данные о  ");
            }

            switch (numdate)
            {
                case 1:
                    UpdateUserName(account);
                    UpdateUserAge(account);
                    UpdateUserPhone(account);
                    break;
                case 2:
                    UpdateUserName(account);
                    break;
                case 3:
                    UpdateUserAge(account);
                    break;
                case 4:
                    UpdateUserPhone(account);
                    break;
                case 5:
                    break;
            }
        }
        else
            Console.WriteLine($"данные о пользователе {account.User.Name} не найдены");
    }
}
void UpdateUserName(Account account)
{

    using (ApplicationContext db = new ApplicationContext())
    {
        Account? ac = db.Accounts
            .Include(a => a.User)
            .FirstOrDefault(a => a.Login == account.Login && a.Password == account.Password);

        if (ac != null)
        {
            var user = ac.User;
            if (user == null)
            {
                Console.WriteLine("Пользователь не найден.");
                return;
            }

            Console.WriteLine("Введите новое имя: ");
            Console.Write("Новое имя - ");
            string newName = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(newName))
            {
                user.Name = newName;
                db.SaveChanges();
                Console.WriteLine($"Данные пользователя изменены на {newName}.");
            }
            else
            {
                Console.WriteLine("Имя не может быть пустым.");
            }

            Console.WriteLine($"данные пользователя {newName} изменены .");
        }
        else
        {
            Console.WriteLine("Аккаунт не найден.");
        }
    }
}
void UpdateUserAge(Account account)
{

    using (ApplicationContext db = new ApplicationContext())
    {
        Account? ac = db.Accounts.FirstOrDefault(a => a.Login == account.Login && a.Password == account.Password);

        if (ac != null)
        {
            Console.WriteLine("Введите новый возраст: ");
            Console.Write("Новый возраст - ");
            int NewAge;
            while (!Int32.TryParse(Console.ReadLine(), out NewAge) || NewAge < 1 || NewAge > 100)
            {
                Console.WriteLine("Не верный ввод.Введите число:");
                Console.Write("Новый возраст -  ");
            }

            account.User.Age = NewAge;
            db.SaveChanges();

            Console.WriteLine($"данные пользователя {account.User.Name} изменены .");
        }

    }
}

void UpdateUserPhone(Account account)
{

    using (ApplicationContext db = new ApplicationContext())
    {
        Account? ac = db.Accounts.FirstOrDefault(a => a.Login == account.Login && a.Password == account.Password);

        if (ac != null)
        {
            Console.WriteLine("Введите новый номер телефона: ");
            Console.Write("Новый номер - ");
            int NewPhone;
            while (!Int32.TryParse(Console.ReadLine(), out NewPhone) || NewPhone.ToString().Length > 13)
            {
                Console.WriteLine("Не верный ввод.Введите номер:");
                Console.Write("Новый номер телефона -  ");
            }

            account.User.Phone = NewPhone;
            db.SaveChanges();

            Console.WriteLine($"данные пользователя {account.User.Name} изменены .");
        }

    }
}

