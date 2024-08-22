using Microsoft.EntityFrameworkCore;
namespace UrlShortner.Services;


public class UrlShorteningService
{
    public const int NumberOfCharsInShortLink = 7;
    private const string Alpahabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnoprstuvwxyz0123456789";

    private readonly Random _random = new ();
    private readonly ApplicationDbContext _dbContext;

    public UrlShorteningService (ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<string> GenerateUniqueCode()
    {
        var codeChars = new char [NumberOfCharsInShortLink];

        while (true)
        {
            // very low posabillity to generate duplicate unique code
            for (var i = 0; i < NumberOfCharsInShortLink; i++)
            {
                var randomIndex = _random.Next(Alpahabet.Length - 1);

                codeChars[i] = Alpahabet[randomIndex];
            }
            var code = new string (codeChars);

            if(!await _dbContext.ShortenedUrls.AnyAsync(s => s.Code == code))
            {
                return code;
            }
        }
    }
}