using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceTracker.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public required string ErrorMessage { get; set; }

    public required string ErrorText { get; set; }



    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        ErrorMessage = GetRandomErrorMessage();
        ErrorText = GetRandomErrorText();
    }

    private string GetRandomErrorMessage()
    {
        var messages = new List<string>
        {
            "An error occurred while processing your request. Please try again later.",
            "Oops! Something went wrong. Our team is working on it.",
            "Yikes! An error happened while processing your request. Hang tight.",
            "Error 404: Unicorn not found. Just kidding, something went wrong.",
            "Oh no! A wild error appeared.",
            "A STOP: 0x0000007B INACCESSIBLE_BOOT_DEVICE error has occurred.",
            "The system has panicked due to a critical error. Please restart your computer.",
            "SERVICE ERROR 305: MESSAGE DELIVERY FAILED. FURTHER MESSAGES WILL BE CHARGED TO YOUR ACCOUNT.",
            "Thank you for texting the Texas Attorney General's fraudulent SMS detection hotline. Please reply with your address for service of process in Texas. Failure to comply is punishable by a fine not to exceed $250,000.",
            "Oops! It looks like we've misplaced your data. Don't worry, we'll find it... eventually.",
            "Error 500: Internal Server Error. Our servers need a coffee break.",
            "Houston, we have a problem. Error code: 1234. Abort mission?",
            "Error 418: I'm a teapot. Seriously, I'm just a teapot.",
            "Something went wrong, but at least it's not your fault!",
            "Error 401: Unauthorized. Even this error message doesn't have permission.",
            "Error: The internet is full. Please try again later.",
            "Our system is on vacation. Please check back after it returns.",
            "Error 202: System taking a nap. Please try again after a coffee break.",
            "Warning: The system is feeling a bit under the weather today.",
            "Error 403: Forbidden. You shall not pass!",
            "Error 007: License to crash. Please contact Q for assistance.",
            "System error: Something went wrong, but we're not sure what. Try turning it off and on again.",
            "Error 512: The system is experiencing an existential crisis.",
            "Oops! Our server tripped over a cable. We'll get it plugged back in soon.",
            "Error 999: Cat on keyboard. Please remove the cat and try again.",
            "This is not the webpage you're looking for. Move along.",
            "Error 12345: The computer is tired. Please let it rest for a moment.",
            "Error 789: A wormhole opened in our server room. Please hold while we investigate.",
            "Error 451: Sorry, the server is on vacation at the moment. Please try again later."

        };

        var random = new Random();
        int index = random.Next(messages.Count);
        return messages[index];
    }

    private string GetRandomErrorText()
    {
        var text = new List<string>
        {
            "In the mystical land of Development, unicorns prance upon rainbows, and error messages transform into whimsical haikus. If you wish to access this fantastical realm, simply sprinkle some pixie dust on your keyboard and chant 'debug-a-lot' thrice. While riding a unicycle, of course. Beware of the mischievous Code Gremlins who feast on stray semicolons and misplaced curly braces, turning your code into abstract art when no one is watching. But as the moon wanes, the Development environment slowly reveals its practical side once more. The detailed error information becomes a beacon of hope for resolving issues. Remember to disable Development mode for deployed applications to protect sensitive information. For local debugging, ensure your ASPNETCORE_ENVIRONMENT variable is set to Development and restart your application.",
            ":)",
            "AHAHAHAHAHAHAHAHAHAHHAHA",
            "Well.. well.. well.. Look who finally came back....",
            "Ok nahhh now fr what did you do",
            "Gahhhh damnnnnn",
            ":(",
            "._.",
            ":[",
            "D:",
            "FUEHHHH",
            "In short terms: ",
            "In short terms: Banana Error!",
            "Some statistics about the mansion: 24,000 square feet, 10,000 windows, 2,000 doors, 52 skylights, 47 fireplaces, 40 bedrooms, 40 staircases, 13 bathrooms, six kitchens, three elevators, two basements and one shower.",
            "Platelets are cells that clump together and help to form blood clots. Aspirin keeps platelets from clumping together, thus helping to prevent or reduce blood clots. During a heart attack. Blood clots form in an already-narrowed artery and block the flow of oxygen-rich blood to the heart muscle.",
            "Supposedly, this pizza was first created by the baker Raffaele Esposito in Naples. His creation was immediately a favorite, and Esposito was called to make a pizza for the visit of King Umberto and Queen Margherita of Italy in 1889.",
            "Forecasters begin using names in 1950. In that year and in 1951, names were from the international phonetic alphabet in use at the time - Able, Baker, Charlie, etc. Female, English-language names were used beginning in 1953. Alternating male and female names were first used to name Atlantic Basin hurricanes in 1979.",
            "The Statue of Liberty (Liberty Enlightening the World; French: La Liberté éclairant le monde) is a colossal neoclassical sculpture on Liberty Island in the middle of New York Harbor, in Manhattan, New York City.",
            "The solubility of gases decreases when the temperature is raised, and that why the dissolved air bubbles out from the water. Then, as the boiling point of water is reached (100 degrees Celsius or 212 degrees Fahrenheit), water vapor starts to form inside the liquid in the form of bubbles.",
            "The word “cookie” comes from the Dutch “koekjes” which came from the Dutch “koek”, meaning “cake”; cookie was introduced to English in the very early 18th century. It is thought this term caught on more in the United States due to the strong Dutch heritage in early America.",

        };

        var random = new Random();
        int index = random.Next(text.Count);
        return text[index];
    }
}

