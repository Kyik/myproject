using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using System.IO;
//using Microsoft.AspNetCore.Mvc.Rendering;

namespace JobPostingSystem;

public class Helper
{
    private readonly IWebHostEnvironment en;
    private readonly IHttpContextAccessor ct;
    private readonly IConfiguration cf; //inject new configurations from appsettings.json

    public Helper(IWebHostEnvironment en, IHttpContextAccessor ct, IConfiguration cf)
    {
        this.en = en;
        this.ct = ct;
        this.cf = cf;
    }

    // ------------------------------------------------------------------------
    // Photo Upload
    // ------------------------------------------------------------------------

    public string ValidatePhoto(IFormFile f)
    {
        var reType = new Regex(@"^image\/(jpeg|png)$", RegexOptions.IgnoreCase);
        var reName = new Regex(@"^.+\.(jpeg|jpg|png)$", RegexOptions.IgnoreCase);

        if (!reType.IsMatch(f.ContentType) || !reName.IsMatch(f.FileName))
        {
            return "Only JPG and PNG photo is allowed.";
        }
        else if (f.Length > 5 * 1024 * 1024)
        {
            return "Photo size cannot more than 5MB.";
        }

        return "";
    }

    public string ValidateVideo(IFormFile f)
    {
        var reType = new Regex(@"^image\/(mp4)$", RegexOptions.IgnoreCase);
        var reName = new Regex(@"^.+\.(mp4)$", RegexOptions.IgnoreCase);

        if (!reType.IsMatch(f.ContentType) || !reName.IsMatch(f.FileName))
        {
            return "Only mp4 videos are allowed.";
        }
        else if (f.Length > 10 * 1024 * 1024)
        {
            return "Video size cannot more than 10MB.";
        }

        return "";
    }

    public string SavePhoto(IFormFile f, string folder)
    {
        var file = Guid.NewGuid().ToString("n") + ".jpg";
        var path = Path.Combine(en.WebRootPath, folder, file);

        var options = new ResizeOptions
        {
            Size = new(200, 200),
            Mode = ResizeMode.Crop,
        };

        using var stream = f.OpenReadStream();
        using var img = Image.Load(stream);
        img.Mutate(x => x.Resize(options));
        img.Save(path);

        return file;
    }
    public string SaveVideo(IFormFile f, string folder)
    {
        var file = Guid.NewGuid().ToString("n") + ".mp4";
        var path = Path.Combine(en.WebRootPath, folder, file);

        using var stream = System.IO.File.Create(path);
        f.CopyTo(stream);
        
        return file;
    }


    public void DeletePhoto(string file, string folder)
    {
        file = Path.GetFileName(file);
        var path = Path.Combine(en.WebRootPath, folder, file);
        File.Delete(path);
    }



    // ------------------------------------------------------------------------
    // Security Helper Functions
    // ------------------------------------------------------------------------

    private readonly PasswordHasher<object> ph = new();

    public string HashPassword(string password)
    {
        return ph.HashPassword(0, password);
    }

    public bool VerifyPassword(string hash, string password)
    {
        return ph.VerifyHashedPassword(0, hash, password) == PasswordVerificationResult.Success;
    }

    public void SignIn(string email, string role, bool rememberMe)
    {
        // (1) Claim, identity and principal
        // TODO
        List<Claim> claims = [
            new(ClaimTypes.Name,email),
            new(ClaimTypes.Role,role),
            ];

        // TODO
        ClaimsIdentity identity = new(claims, "Cookies");

        // TODO
        ClaimsPrincipal principal = new(identity);

        // (2) Remember me (authentication properties)
        // TODO
        AuthenticationProperties properties = new()
        {
            IsPersistent = rememberMe,
        };

        // (3) Sign in
        ct.HttpContext!.SignInAsync(principal, properties);
    }

    public void SignOut()
    {
        // Sign out
        ct.HttpContext!.SignOutAsync();
    }

    public string RandomPassword()
    {
        string s = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string password = "";

        Random r = new();
        for (int i = 1; i <= 10; i++)
        {
            password += s[r.Next(s.Length)];
        }

        return password;
    }




    //------ Send Email -----------------------
    public void SendEmail(MailMessage mail)
    {
        // TODO
        string user = cf["Smtp:User"] ?? "";
        string pass = cf["Smtp:Pass"] ?? "";
        string name = cf["Smtp:Name"] ?? "";
        string host = cf["Smtp:Host"] ?? "";
        int port = cf.GetValue<int>("Smtp:Port");

        // TODO
        mail.From = new MailAddress(user, name);

        using var smtp = new SmtpClient
        {
            Host = host,
            Port = port,
            EnableSsl = true,
            Credentials = new NetworkCredential(user, pass),

        };

        smtp.Send(mail);
    }
}


