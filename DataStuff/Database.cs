using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;




public class WebUser
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string? Phonenumber { get; set; }
    public string Password { get; set; }

    public List<Session> Sessions { get; set; } = [];

    static Dictionary<string, SessionUser> IdDict { get; set; } = [];

    public List<Transaction> Transactions { get; set; } = [];

    private static string filePath = Path.Combine(Directory.GetCurrentDirectory(), "UserData.json");

    private static List<WebUser> userList = new List<WebUser>();

    public WebUser() { Email = ""; Name = ""; Phonenumber = ""; Password = ""; userList.Add(this); }
    public WebUser(string email, string name, string? phonenumber, string password)
    { Email = email; Name = name; Phonenumber = phonenumber; Password = password; userList.Add(this); }

    public static void loadJson()
    {
        if (System.IO.File.Exists(filePath))
        {
            // Read the existing JSON data from the file
            string existingJson = System.IO.File.ReadAllText(filePath);

            // Try to deserialize the existing JSON data into a list of User objects
            userList = JsonSerializer.Deserialize<List<WebUser>>(existingJson) ?? new List<WebUser>();
            foreach (var user in userList)
            {
                foreach (var session in user.Sessions)
                {
                    IdDict.Add(session.Id, new SessionUser(session, user));
                }
            }
        }
    }

    public static void saveJson()
    {

        foreach (var user in userList)
        {
            List<Session> SessionsToDelete = [];
            foreach (var session in user.Sessions)
            {
                if (session.ExpireDate < DateTime.Now)
                {
                    SessionsToDelete.Add(session);
                }
            }
            foreach (var sessiontodelete in SessionsToDelete)
            {
                user.Sessions.Remove(sessiontodelete);
            }
        }
        try
        {
            // Add the new data to the list
            // Serialize the updated list to a JSON string 
            string updatedJson = JsonSerializer.Serialize(userList, new JsonSerializerOptions { WriteIndented = true });

            // Write the updated JSON string to the specified file
            System.IO.File.WriteAllText(filePath, updatedJson);

            Console.WriteLine($"Data successfully appended to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
        }
    }

    public static WebUser? getUserByEmail(string email)
    {
        Console.WriteLine($"Email--: {email}");
        foreach (var user in userList)
        {
            Console.WriteLine($"Checking user: {user.Email}");
            if (user.Email == email)
            {
                Console.WriteLine(user.Email, user.Name, user.Password);
                return user;
            }
        }
        return null;
    }


    public static bool EmailExists(string Email)
    {
        Console.WriteLine("EmailExists Email: ", Email);
        if (userList.Any(user => user.Email == Email))
        {
            return true;
        }
        return false;
    }


    public string CreateSession(DateTime expireDate)
    {
        string base64String = Stuff.GenerateRandomBase64String(32);
        Session session = new Session(base64String, expireDate);
        Sessions.Add(session);
        IdDict.Add(base64String, new SessionUser(session, this));
        return base64String;
    }

    public static WebUser? GetUserBySession(string session)
    {
        if(!IdDict.ContainsKey(session)) return null;
        SessionUser sessionuser = IdDict[session];
        if (sessionuser.Session.ExpireDate < DateTime.Now) return null;
        return sessionuser.User;
    }


    public static void DeleteSession(string session)
    {
        if(!IdDict.ContainsKey(session)) return;
        SessionUser sessionuser = IdDict[session];
        sessionuser.User.Sessions.Remove(sessionuser.Session);
        IdDict.Remove(session);
    }




}


public class Session(string id, DateTime expiredate)
{
    public string Id { get; set; } = id;
    public DateTime ExpireDate { get; set; } = expiredate;

}


public class SessionUser(Session session, WebUser user)
{
    public WebUser User { get; set; } = user;
    public Session Session { get; set; } = session;

}


public class Transaction
{
    public Transaction() 
    {
        Type = "EMPTY";
        Category = "EMPTY";
        UseCase = "EMPTY";
        Amount = 0;
        Origin = "EMPTY";
        Destination = "EMPTY";
        Date = DateTime.Now;
    }

    public Transaction(string type, string category, string useCase, int amount, string origin, string destination, DateTime date)
    {
        Type = type;
        Category = category;
        UseCase = useCase;
        Amount = amount;
        Origin = origin;
        Destination = destination;
        Date = date;
    }

    public string? Type { get; set; }
    public DateTime? Date { get; set; }
    public double? Amount { get; set; }
    
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public string? UseCase { get; set; }
    public string? Category { get; set; }

    
}

