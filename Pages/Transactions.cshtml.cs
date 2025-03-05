// using FinanceUser;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;

// namespace FinanceTracker.Pages;

// public class TransactionsModel : PageModel
// {

//     [BindProperty]
//     public required string Type { get; set; }


//     [BindProperty]
//     public required DateTime Date { get; set; }


//     [BindProperty]
//     public required decimal Amount { get; set; }


//     [BindProperty]
//     public required string Origin { get; set; }


//     [BindProperty]
//     public required string Destination { get; set; }


//     [BindProperty]
//     public string? Description { get; set; }


//     [BindProperty]
//     public required string UseCase { get; set; }



//     [BindProperty]
//     public required string Category { get; set; }


//     [BindProperty]
//     public string? SenderName { get; set; }


//     [BindProperty]
//     public string? SenderAccount { get; set; }


//     [BindProperty]
//     public bool IsIncoming { get; set; }


//     public required string ID { get; set; }

//     [BindProperty]
//     public required string Trans_ID { get; set; }




//     public WebUser? WebUser { get; set; }
//     public IActionResult OnGet()
//     {


//         WebUser = null;
//         if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
//         if (sessionId == null) return RedirectToPage("/Index");
//         WebUser = WebUser.GetUserBySession(sessionId);
//         if (WebUser == null) return RedirectToPage("/Index");
//         if(!WebUser.HasFinancialAccounts()) return RedirectToPage("/AddFinAccDash");
//         Console.WriteLine($"User: {WebUser.Name} Email: {WebUser.Email} Loaded Page: /Transactions");
//         return Page();
//     }

//     public IActionResult OnPostAddTransaction()
//     {

//         try
//         {
//             WebUser = null;
//             if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
//             if (sessionId == null) return RedirectToPage("/Index");
//             WebUser = WebUser.GetUserBySession(sessionId);
//             if (WebUser == null) return RedirectToPage("/Index");


//             // if (IsIncoming)
//             // {
//             //     var newIncomingTransaction = new Transaction
//             //     (
//             //         Type,
//             //         Date,
//             //         Amount,
//             //         Origin,
//             //         Destination,
//             //         Description,
//             //         UseCase,
//             //         Category,
//             //         SenderName,
//             //         SenderAccount,
//             //         IsIncoming,
//             //         ID = System.Guid.NewGuid().ToString()
//             //     );

//             //     WebUser.Transactions.Add(newIncomingTransaction);
//             // }
//             // else
//             // {
//             //     var newOutgoingTransaction = new Transaction
//             //     (
//             //     Type,
//             //         Date,
//             //         Amount,
//             //         Origin,
//             //         Destination,
//             //         Description,
//             //         UseCase,
//             //         Category,
//             //         SenderName,
//             //         SenderAccount,
//             //         IsIncoming,
//             //         ID = System.Guid.NewGuid().ToString()
//             //     );
//             //     WebUser.Transactions.Add(newOutgoingTransaction);

//             // }




//             // Assuming WebUser is initialized somewhere and has a Transactions property



//             Console.WriteLine("User: " + WebUser.Name + "   Email: " + WebUser.Email + " Add Transaction Post");
//             // Return JSON response of the transactions
//             return RedirectToPage("Transactions");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error adding transaction: {ex.Message}");
//             return RedirectToPage("/Error"); // Handle error appropriately
//         }
//     }

//     public IActionResult OnPostEditTransaction(string Trans_ID)
//     {
//         WebUser = null;
//         if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
//         if (sessionId == null) return RedirectToPage("/Index");
//         WebUser = WebUser.GetUserBySession(sessionId);
//         if (WebUser == null) return RedirectToPage("/Index");

//         // if (Trans_ID != null)
//         // {

//         //     var currentTransaction = WebUser.GetTransactionByID(Trans_ID);

//         //     currentTransaction.Type = Type;
//         //     currentTransaction.Category = Category;
//         //     currentTransaction.UseCase = UseCase;
//         //     currentTransaction.Amount = Amount;
//         //     currentTransaction.Origin = Origin;
//         //     currentTransaction.Destination = Destination;
//         //     currentTransaction.Date = Date;
//         //     currentTransaction.ID = Trans_ID;

//         //     OnPostDeleteTransaction(Trans_ID);
//         //     WebUser.Transactions.Add(currentTransaction);
//         // }
//         return Page();
//     }


//     public IActionResult OnPostDeleteTransaction(string Trans_ID)
//     {
//         try
//         {
//             WebUser = null;
//             if (!Request.Cookies.TryGetValue("SessionCookie", out string? sessionId)) return RedirectToPage("/Index");
//             if (sessionId == null) return RedirectToPage("/Index");
//             WebUser = WebUser.GetUserBySession(sessionId);
//             if (WebUser == null) return RedirectToPage("/Index");

//             if (Trans_ID == null) Console.WriteLine("OnPostDeleteTransaction -> Trans_ID is null!");
//             else
//             {
//                 var transactionToDelete = WebUser.GetTransactionByID(Trans_ID);
//                 if(transactionToDelete == null) Console.WriteLine("OnPostDeleteTransaction -> transactionToDelete is null!");
//                 else
//                     WebUser.Transactions.Remove(transactionToDelete);
//                     Console.WriteLine($"Transaction with ID: {Trans_ID} deleted!");
//             }
//             return Page();
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error deleting transaction: {ex.Message}");
//             return RedirectToPage("/Error");
//         }
//     }

// }

