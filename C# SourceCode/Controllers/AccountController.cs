using JobPostingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace Demo.Controllers;

    public class AccountController : Controller
    {
        private readonly DB db;
        private readonly IWebHostEnvironment en;
        private readonly Helper hp;

        public AccountController(DB db, IWebHostEnvironment en, Helper hp)
        {
            this.db = db;
            this.en = en;
            this.hp = hp;
        }

    //------------ Job History---------------------------------------
    public bool CheckPId(string id) 
    { 
    return !db.CareerHistory.Any(ch => ch.Id == id);
    }

    private string NextPid()
    {
        string max = db.CareerHistory.Max(ch => ch.Id) ?? "P000";
        int n = int.Parse(max[1..]);
        return (n + 1).ToString("'P'000");
    }


    //------Get---------
    [Authorize(Roles = "Member")]
    public IActionResult SetUpProfile2()
    {
        var m = db.Members.Find(User.Identity!.Name);
        if (m == null) return RedirectToAction("Index", "Home");

        var vm = new CareerVM
        {
            Id = NextPid(),
        };

        return View(vm);
    }

    //---POST--------------------
    [HttpPost]
    [Authorize]
    public IActionResult SetUpProfile2(CareerVM vm)
    {
        var m = db.Members.Find(User.Identity!.Name);
        if (m == null) return RedirectToAction("Index", "Home");

      

        if (ModelState.IsValid)
        {
            db.CareerHistory.Add(new()
            {
                Id = vm.Id,
                Career = vm.Career,
                CompanyName = vm.CompanyName,
                StartDatec = vm.StartDatec ?? DateTime.MinValue, // Handle nullable DateTime
                EndDatec = vm.EndDatec ?? DateTime.MinValue,     // Handle nullable DateTime
                UserEmail = m.Email,
            });

            db.SaveChanges();

            TempData["Info"] = "Profile Setup Completed!";
            return RedirectToAction("Index", "Home");
        }

        return View(vm);
    }

    //--------- End of  Job History ---------------------------





    //--------- Education Level -----------------------

    public bool CheckId(string id)
    {
        return !db.Education.Any(p => p.Id == id);
    }

    private string NextId()
    {
        string max = db.Education.Max(p => p.Id) ?? "E000";
        int n = int.Parse(max[1..]);
        return (n + 1).ToString("'E'000");
    }

    [Authorize(Roles ="Member")]
    public IActionResult SetUpProfile1() {
        var m = db.Members.Find(User.Identity!.Name);
        if (m == null) return RedirectToAction("Index", "Home");

        var vm = new EducationVM
        {
            Id = NextId(),
        };

        return View(vm);
    }
    [HttpPost]
    [Authorize]
    public IActionResult SetUpProfile1(EducationVM vm)
    {
        var m = db.Members.Find(User.Identity!.Name);
        if (m == null) return RedirectToAction("Index", "Home");

        if (ModelState.IsValid)
        {
            db.Education.Add(new()
            {
                Id = vm.Id,
                Course = vm.Course,
                Institution = vm.Institution,
                StartDate = vm.StartDate ?? DateTime.MinValue, // Handle nullable DateTime
                EndDate = vm.EndDate ?? DateTime.MinValue,     // Handle nullable DateTime
                UserEmail = m.Email,
            });

            db.SaveChanges();

            TempData["Info"] = "Almost Done! Last Step To Finish";
            return RedirectToAction("SetUpProfile2", "Account");
        }

        return View(vm);
    }

    //Get: Account/AddEducation
    [Authorize]
    public IActionResult AddEducation()
    {
        var vm = new EducationVM
        {
            Id = NextId(),
            StartDate = DateTime.Now, // Optional: Set defaults
            EndDate = DateTime.Now
        };
        return View(vm);
    }



    [HttpPost]
    [Authorize]
    public IActionResult AddEducation(EducationVM vm)
    {
        var m = db.Members.Find(User.Identity!.Name);
        if (m == null) return RedirectToAction("Index", "Home");

        if (ModelState.IsValid)
        {
            db.Education.Add(new()
            {
                Id = vm.Id,
                Course = vm.Course,
                Institution = vm.Institution,
                StartDate = vm.StartDate ?? DateTime.MinValue, // Handle nullable DateTime
                EndDate = vm.EndDate ?? DateTime.MinValue,     // Handle nullable DateTime
                UserEmail = m.Email,
            });

            db.SaveChanges();

            TempData["Info"] = "Education Added.";
            return RedirectToAction("UpdateProfile", "Account");
        }

        return View(vm);


       
    } 

    //Get: Account/AddEducation
    [Authorize]
    public IActionResult AddCareer()
    {
        var vm = new CareerVM
        {
            Id = NextPid(),
            StartDatec = DateTime.Now, // Optional: Set defaults
            EndDatec = DateTime.Now
        };
        return View(vm);
    }



    [HttpPost]
    [Authorize]
    public IActionResult AddCareer(CareerVM vm)
    {
        var m = db.Members.Find(User.Identity!.Name);
        if (m == null) return RedirectToAction("Index", "Home");

        if (ModelState.IsValid)
        {
            db.CareerHistory.Add(new()
            {
                Id = vm.Id,
                Career = vm.Career,
                CompanyName = vm.CompanyName,
                StartDatec = vm.StartDatec ?? DateTime.MinValue, // Handle nullable DateTime
                EndDatec = vm.EndDatec ?? DateTime.MinValue,     // Handle nullable DateTime
                UserEmail = m.Email,
            });

            db.SaveChanges();

            TempData["Info"] = "Education Added.";
            return RedirectToAction("UpdateProfile", "Account");
        }

        return View(vm);
    }

    // GET: Account/UpdateEducation/{id}
    [Authorize(Roles = "Member")]
    public IActionResult UpdateEducation(string id)
    {
        var user = db.Users.Find(User.Identity!.Name);
        if (user == null) return RedirectToAction("Index", "Home");

        var edu = db.Education.FirstOrDefault(e => e.Id == id && e.UserEmail == user.Email);
        if (edu == null) return NotFound();

        var vm = new UpdateEducationVM
        {
            Id = edu.Id,
            Course = edu.Course,
            Institution = edu.Institution,
            StartDate = edu.StartDate,
            EndDate = edu.EndDate
        };

        return View(vm);
    }
    // POST: Account/UpdateEducation
    [HttpPost]
    [Authorize(Roles = "Member")]
    public IActionResult UpdateEducation(UpdateEducationVM vm)
    {
        var user = db.Users.Find(User.Identity!.Name);
        if (user == null) return RedirectToAction("Index", "Home");

        if (ModelState.IsValid)
        {
            var edu = db.Education.FirstOrDefault(e => e.Id == vm.Id && e.UserEmail == user.Email);
            if (edu == null) return NotFound();
            edu.Id = vm.Id; 
            edu.Course = vm.Course;
            edu.Institution = vm.Institution;
            edu.StartDate = vm.StartDate ?? DateTime.MinValue; // Handle nullable DateTime
            edu.EndDate = vm.EndDate ?? DateTime.MinValue;

            db.SaveChanges();

            TempData["Info"] = "Education updated successfully.";
            return RedirectToAction("UpdateProfile", "Account");
        }

        return View(vm);
    }

    // GET: Account/UpdateCareer/{id}
    [Authorize(Roles = "Member")]
    public IActionResult UpdateCareer(string id)
    {
        var user = db.Users.Find(User.Identity!.Name);
        if (user == null) return RedirectToAction("Index", "Home");

        var crh = db.CareerHistory.FirstOrDefault(e => e.Id == id && e.UserEmail == user.Email);
        if (crh == null) return NotFound();

        var vm = new UpdateCareerVM
        {
            Id = crh.Id,
            Career = crh.Career,
            CompanyName = crh.CompanyName,
            StartDatec = crh.StartDatec,
            EndDatec = crh.EndDatec
        };

        return View(vm);
    }
    // POST: Account/UpdateEducation
    [HttpPost]
    [Authorize(Roles = "Member")]
    public IActionResult UpdateCareer(UpdateCareerVM vm)
    {
        var user = db.Users.Find(User.Identity!.Name);
        if (user == null) return RedirectToAction("Index", "Home");

        if (ModelState.IsValid)
        {
            var crh = db.CareerHistory.FirstOrDefault(e => e.Id == vm.Id && e.UserEmail == user.Email);
            if (crh == null) return NotFound();
            crh.Id = vm.Id;
            crh.Career = vm.Career;
            crh.CompanyName = vm.CompanyName;
            crh.StartDatec = vm.StartDatec ?? DateTime.MinValue; // Handle nullable DateTime
            crh.EndDatec = vm.EndDatec ?? DateTime.MinValue;

            db.SaveChanges();

            TempData["Info"] = "Career History updated successfully.";
            return RedirectToAction("UpdateProfile", "Account");
        }

        return View(vm);
    }
    //------------ End Education Level---------------------


    // GET: Account/Login
    public IActionResult Login()
        {
            return View();
        }


    // POST: Account/Login
    [HttpPost]
    public IActionResult Login(LoginVM vm, string? returnURL)
    {
        var u = db.Users.FirstOrDefault(user => user.Email == vm.Email);

        // Check if user exists
        if (u == null)
        {
            ModelState.AddModelError("", "Login credentials not matched.");
            return View(vm);
        }

        // Check if the user is blocked
        if (LoginTracker.BlockedUsers.TryGetValue(vm.Email, out DateTime unblockTime))
        {
            if (DateTime.Now < unblockTime)
            {
                TempData["Error"] = $"Account is temporarily blocked. Try again after {unblockTime.ToString("HH:mm:ss")}.";
                return View(vm);
            }
            else
            {
                LoginTracker.BlockedUsers.TryRemove(vm.Email, out _);
                LoginTracker.FailedLoginAttempts.TryRemove(vm.Email, out _);
            }
        }

        // Validate password
        if (!hp.VerifyPassword(u.Hash, vm.Password))
        {
            LoginTracker.FailedLoginAttempts.AddOrUpdate(vm.Email, 1, (key, count) => count + 1);

            if (LoginTracker.FailedLoginAttempts[vm.Email] >= 3)
            {
                LoginTracker.BlockedUsers[vm.Email] = DateTime.Now.AddMinutes(15);
                TempData["Error"] = "Account is temporarily blocked due to multiple failed login attempts. Try again later.";
            }
            else
            {
                int remainingAttempts = 3 - LoginTracker.FailedLoginAttempts[vm.Email];
                TempData["Error"] = $"Invalid credentials. You have {remainingAttempts} attempt(s) remaining.";
            }

            return View(vm);
        }


        // Successful login: Reset failed attempts
        LoginTracker.FailedLoginAttempts.TryRemove(vm.Email, out _);
        LoginTracker.BlockedUsers.TryRemove(vm.Email, out _);

        // Proceed with login
        TempData["Info"] = "Login successfully.";
        hp.SignIn(u.Email, u.Role, vm.RememberMe);

        if (string.IsNullOrEmpty(returnURL))
        {
            return RedirectToAction("Index", "Home");
        }

        return Redirect(returnURL);
    }

    // GET: Account/Logout
    public IActionResult Logout(string? returnURL)
        {
            // Clear session data
            HttpContext.Session.Clear();

            TempData["Info"] = "Logout successfully.";

            hp.SignOut();

            return RedirectToAction("Index", "Home");
        }


        // GET: Account/AccessDenied
        public IActionResult AccessDenied(string? returnURL)
        {
            return View();
        }

        // GET: Account/CheckEmail
        public bool CheckEmail(string email)
        {
            return !db.Users.Any(u => u.Email == email);
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }
    [Authorize(Roles ="Admin")]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

    [Authorize(Roles = "Admin")]
    public IActionResult RegisterInstructor()
        {
            return View();
        }
        public IActionResult RegisterEmployer()
        {
            return View();
        }
        // POST: Account/Register
       [HttpPost]
public IActionResult Register(RegisterMVM vm)
{
    if (ModelState.IsValid("Email") && db.Users.Any(u => u.Email == vm.Email))
    {
        ModelState.AddModelError("Email", "Duplicated Email.");
    }

    if (ModelState.IsValid("Photo"))
    {
        var err = hp.ValidatePhoto(vm.Photo);
        if (err != "") ModelState.AddModelError("Photo", err);
    }

    if (ModelState.IsValid)
    {
            // Create a new member
            db.Members.Add(new()
            {
            Email = vm.Email,
            Hash = hp.HashPassword(vm.Password),
            Name = vm.Name,
            DateOfBirth = vm.DateOfBirth,
            Gender = vm.Gender,
            Contact = vm.Contact,
            Address = vm.Address,
            PhotoURL = hp.SavePhoto(vm.Photo, "photos"),
        });

            // Add the member to the database

            db.SaveChanges();

        // Auto-login logic
        HttpContext.Session.SetSessionValue("UserEmail", vm.Email);
        HttpContext.Session.SetSessionValue("UserRole", "Member"); // Adjust based on your role logic
        hp.SignIn(vm.Email, "Member", rememberMe: false);

        TempData["Info"] = "Registered and logged in successfully.";
        return RedirectToAction("SetUpProfile1", "Account");
    }

    return View(vm);
}


        [HttpPost]
        public IActionResult RegisterAdmin(RegisterAVM vm)
        {
            if (ModelState.IsValid("Email") && db.Users.Any(u => u.Email == vm.Email))
            {
                ModelState.AddModelError("Email", "Duplicated Email.");
            }

            if (ModelState.IsValid("Photo"))
            {
                var err = hp.ValidatePhoto(vm.Photo);
                if (err != "") ModelState.AddModelError("Photo", err);
            }

            if (ModelState.IsValid)
            {
                db.Admins.Add(new()
                {
                    Email = vm.Email,
                    Hash = hp.HashPassword(vm.Password),
                    Name = vm.Name,
                    DateOfBirth = vm.DateOfBirth,
                    Gender = vm.Gender,
                    Contact = vm.Contact,
                    PhotoURL = hp.SavePhoto(vm.Photo, "photos"),
                });

                db.SaveChanges();

                TempData["Info"] = "Register successfully. Please login.";
                return RedirectToAction("RegisterInstructor","Account");
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult RegisterInstructor(RegisterIVM vm)
        {
            if (ModelState.IsValid("Email") && db.Users.Any(u => u.Email == vm.Email))
            {
                ModelState.AddModelError("Email", "Duplicated Email.");
            }

            if (ModelState.IsValid("Photo"))
            {
                var err = hp.ValidatePhoto(vm.Photo);
                if (err != "") ModelState.AddModelError("Photo", err);
            }

            if (ModelState.IsValid)
            {
                db.Instructors.Add(new()
                {
                    Email = vm.Email,
                    Hash = hp.HashPassword(vm.Password),
                    Name = vm.Name,
                    DateOfBirth = vm.DateOfBirth,
                    Gender = vm.Gender,
                    Contact = vm.Contact,
                    PhotoURL = hp.SavePhoto(vm.Photo, "photos"),
                });

                db.SaveChanges();

                TempData["Info"] = "Register successfully. Please login.";
                return RedirectToAction("RegisterInstructor","Account" );
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult RegisterEmployer(RegisterEVM vm)
        {
            if (ModelState.IsValid("Email") && db.Users.Any(u => u.Email == vm.Email))
            {
                ModelState.AddModelError("Email", "Duplicated Email.");
            }

            if (ModelState.IsValid("Photo"))
            {
                var err = hp.ValidatePhoto(vm.Photo);
                if (err != "") ModelState.AddModelError("Photo", err);
            }

            if (ModelState.IsValid)
            {
                db.Employers.Add(new()
                {
                    Email = vm.Email,
                    Hash = hp.HashPassword(vm.Password),
                    Name = vm.Name,
                    Gender = vm.Gender,
                    Contact = vm.Contact,
                    BusinessName = vm.BusinessName,
                    PhotoURL = hp.SavePhoto(vm.Photo, "photos"),
                });

                db.SaveChanges();

                TempData["Info"] = "Register successfully. Please login.";
                return RedirectToAction("Login");
            }

            return View(vm);
        }

        // GET: Account/UpdatePassword
        [Authorize]
        public IActionResult UpdatePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult UpdatePassword(UpdatePasswordVM vm)
        {
            var user = db.Users.Find(User.Identity!.Name);
            if (user == null) return RedirectToAction("Index", "Home");

            if (!hp.VerifyPassword(user.Hash, vm.Current))
            {
                ModelState.AddModelError("Current", "Current Password not matched.");
            }

            if (ModelState.IsValid)
            {
                user.Hash = hp.HashPassword(vm.New);
                db.SaveChanges();

                TempData["Info"] = "Password updated.";
                return RedirectToAction("UpdateProfile", "Account");
            }

            return View();
        }


    // GET: Account/UpdateProfile
    [Authorize]
    public IActionResult UpdateProfile(bool showModal = false)
    {
        var user = db.Users.Find(User.Identity!.Name);
        if (user == null) return RedirectToAction("Index", "Home");

        var vm = new UpdateProfileVM
        {
            Email = user.Email,
            Name = user.Name,
            Contact = user.Contact,
            PhotoURL = user.PhotoURL,
            DateOfBirth = user is Member or Admin or Instructor ? ((dynamic)user).DateOfBirth : null,
            Educations = db.Education
                .Where(e => e.UserEmail == user.Email)
                .Select(e => new EducationVM
                {
                    Id = e.Id,
                    Course = e.Course,
                    Institution = e.Institution,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate
                })
                .ToList(),
            Careers = db.CareerHistory
                .Where(c => c.UserEmail == user.Email)
                .Select(c => new CareerVM
                {
                    Id = c.Id,
                    Career = c.Career,
                    CompanyName = c.CompanyName,
                    StartDatec = c.StartDatec,
                    EndDatec = c.EndDatec
                })
                .ToList()
        };

        ViewBag.ShowModal = showModal; // Pass the flag to the view
        return View(vm);
    }




    [HttpPost]
        [Authorize]
        public IActionResult UpdateProfile(UpdateProfileVM vm)
        {
            var user = db.Users.Find(User.Identity!.Name);
            if (user == null) return RedirectToAction("Index", "Home");

            if (vm.Photo != null)
            {
                var err = hp.ValidatePhoto(vm.Photo);
                if (!string.IsNullOrEmpty(err)) ModelState.AddModelError("Photo", err);
            }

        if (ModelState.IsValid)
        {
            user.Name = vm.Name;
            user.Contact = vm.Contact;

            if (User.IsInRole("Member") || User.IsInRole("Instructor") || User.IsInRole("Admin"))
            {
                ((dynamic)user).DateOfBirth = vm.DateOfBirth;
            }

            if (vm.Photo != null)
            {
                hp.DeletePhoto(user.PhotoURL, "photos");
                user.PhotoURL = hp.SavePhoto(vm.Photo, "photos");
            }

            db.SaveChanges();

            TempData["Info"] = "Profile updated successfully.";
            return RedirectToAction("UpdateProfile", "Account");
        }


        vm.PhotoURL = user.PhotoURL;
            return View(vm);
        }

    // GET: Account/UpdateProfile Member



    // GET: Account/ResetPassword
    [HttpGet]
    public IActionResult ResetPassword()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
           return RedirectToAction("Index", "Home");
        }

        return View();
    }

    // POST: Account/ResetPassword
    [HttpPost]
    public IActionResult ResetPassword(ResetPasswordVM vm)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
             return RedirectToAction("Index", "Home");
        }

        var u = db.Users.Find(vm.Email);

        if (u == null)
        {
            ModelState.AddModelError("Email", "Email not found.");
        }

        if (ModelState.IsValid)
        {
            string password = hp.RandomPassword();

            u!.Hash = hp.HashPassword(password);
            db.SaveChanges();

            // Send reset password email
            SendResetPasswordEmail(u, password);

            TempData["Info"] = $"Password reset. Check your email.";
            return RedirectToAction("Login", "Account");
        }

        return View(vm);
    }


    [HttpPost]
    [Authorize]
    public IActionResult DeleteAccount(string Password)
    {
        var userEmail = User.Identity!.Name;
        var user = db.Users.Find(userEmail);

        if (user == null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction("UpdateProfile", new { showModal = true });
        }

        // Validate the password
        if (!hp.VerifyPassword(user.Hash, Password))
        {
            TempData["Error"] = "Password is incorrect. Please try again.";
            return RedirectToAction("UpdateProfile", new { showModal = true });
        }

        try
        {
            // Delete the user account
            db.Users.Remove(user);
            db.SaveChanges();

            // Clear session and sign out the user
            HttpContext.Session.Clear();
            hp.SignOut();

            TempData["Info"] = "Account deleted successfully.";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during account deletion: {ex.Message}");
            TempData["Error"] = "An error occurred while deleting your account. Please try again later.";
            return RedirectToAction("UpdateProfile", new { showModal = true });
        }
    }

    private void SendResetPasswordEmail(User u, string password)
    {
        var mail = new MailMessage();
        mail.To.Add(new MailAddress(u.Email, u.Name));
        mail.Subject = "Reset Password";
        mail.IsBodyHtml = true;

        // TODO
        var url = Url.Action("Login", "Account", null, "https");

        // TODO
        var path = u switch
        {
            Admin => Path.Combine(en.WebRootPath, "photos", "admin.jpg"),
            Member m => Path.Combine(en.WebRootPath, "photos", m.PhotoURL),
            _ => "",
        };

        var att = new Attachment(path);
        mail.Attachments.Add(att);
        att.ContentId = "photo";

        mail.Body = $@"
            <img src='cid:photo' style='width: 200px; height: 200px;
                                        border: 1px solid #333'>
            <p>Dear {u.Name},<p>
            <p>Your password has been reset to:</p>
            <h1 style='color: red'>{password}</h1>
            <p>
                Please <a href='{url}'>login</a>
                with your new password.
            </p>
            <p>From, 🐱 Super Admin</p>
        ";

        hp.SendEmail(mail);
    }

    // POST: Account/DeleteEducation
    [HttpPost]
    [Authorize(Roles = "Member")]
    public IActionResult DeleteEducation(string id)
    {
        // Log the incoming ID for debugging
        Console.WriteLine($"DeleteEducation called with ID: {id}");

        var user = db.Users.Find(User.Identity!.Name);
        if (user == null)
        {
            Console.WriteLine("User not found.");
            return RedirectToAction("Index", "Home");
        }

        var edu = db.Education.FirstOrDefault(e => e.Id == id && e.UserEmail == user.Email);
        if (edu == null)
        {
            Console.WriteLine("Education record not found.");
            return NotFound();
        }

        // Log the record details before deletion
        Console.WriteLine($"Deleting Education: {edu.Course}, {edu.Institution}");

        db.Education.Remove(edu);
        db.SaveChanges();

        Console.WriteLine("Education record deleted successfully.");

        TempData["Info"] = "Education record deleted successfully.";
        return RedirectToAction("UpdateProfile", "Account");
    }

    // POST: Account/DeleteEducation
    [HttpPost]
    [Authorize(Roles = "Member")]
    public IActionResult DeleteCareer(string id)
    {

        var user = db.Users.Find(User.Identity!.Name);
        if (user == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var crh = db.CareerHistory.FirstOrDefault(e => e.Id == id && e.UserEmail == user.Email);
        if (crh == null)
        {
            return NotFound();
        }


        db.CareerHistory.Remove(crh);
        db.SaveChanges();


        TempData["Info"] = "Career  record deleted successfully.";
        return RedirectToAction("UpdateProfile", "Account");
    }


    // POST: Home/ToggleFavourite
    [Authorize]
    [HttpPost]
    public IActionResult ToggleFavourite(string id)
    {
        var user = db.Users.Find(User.Identity!.Name);
        if (user == null)
        {
            return Unauthorized(); // Ensure the user is logged in
        }

        var email = user.Email;
        var favourite = db.Favourites.FirstOrDefault(f => f.JobId == id && f.UserEmail == email);

        if (favourite == null)
        {
            db.Favourites.Add(new Favourite
            {
                JobId = id,
                UserEmail = email,
            });
        }
        else
        {
            db.Favourites.Remove(favourite);
        }

        db.SaveChanges();
        return Redirect(Request.Headers.Referer.ToString());
    }


    // GET: Home/Favourites
    [Authorize]
    public IActionResult Favourites()
    {
        var user = db.Users.Find(User.Identity!.Name);
        if (user == null)
        {
            return Unauthorized(); // Ensure the user is logged in
        }

        var email = user.Email;
        var favouriteStudentIds = db.Favourites
            .Where(f => f.UserEmail == email)
            .Select(f => f.JobId)
            .ToList();

        var model = db.Jobs
            .Where(s => favouriteStudentIds.Contains(s.Id))
            .ToList();

        return View(model);
    }

    // GET: Home/Index
    [Authorize]
    public IActionResult Index()
    {
        var user = db.Users.Find(User.Identity!.Name);
        if (user == null)
        {
            return Unauthorized(); // Ensure the user is logged in
        }

        var email = user.Email;
        var favouriteIds = db.Favourites
            .Where(f => f.UserEmail == email)
            .Select(f => f.JobId)
            .ToList();

        var model = db.Jobs;
        ViewBag.FavouriteIds = favouriteIds;
        return View(model);
    }

    [Authorize]
    // GET: Home/Detail


    // GET: Account/Detail
    [HttpGet]
    public IActionResult Detail(string id)
    {
        var student = db.Jobs.Find(id);
        if (student == null)
        {
            return NotFound();
        }

        var reportVM = new ReportVM
        {
            ReporterEmail = User.Identity!.Name,
            JobID = student.Id
        };

        return View(Tuple.Create(student, reportVM));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult ManageMembers()
    {
        var members = db.Members.Select(m => new
        {
            m.Email,
            m.Name,
            m.Contact,
            m.DateOfBirth,
            m.Gender,
            m.Address,
            m.PhotoURL
        }).ToList();

        return View(members);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteMember(string email)
    {
        var member = db.Members.FirstOrDefault(m => m.Email == email);
        if (member == null)
        {
            TempData["Error"] = "Member not found.";
            return RedirectToAction("ManageMembers");
        }

        db.Members.Remove(member);
        db.SaveChanges();

        TempData["Info"] = "Member deleted successfully.";
        return RedirectToAction("ManageMembers");
    }
    [Authorize(Roles = "Admin")]

    // GET: ReportedJobs
    public IActionResult ReportedJobs()
    {
        var reports = db.Reports
            .Select(r => new ReportedJobVM
            {
                ReportId = r.Id,
                JobId = r.JobID,
                JobTitle = r.Job.Title,
                ReporterEmail = r.ReporterEmail,
                Reason = r.Reason,
                AdditionalReason = r.AddReason,
                ReportDate = r.ReportDate
            })
            .ToList();

        return View(reports);
    }

    // POST: DeleteReport
    [HttpPost]
    public IActionResult DeleteReport(string reportId)
    {
        var report = db.Reports.Find(reportId);
        if (report != null)
        {
            db.Reports.Remove(report);
            db.SaveChanges();
            TempData["Info"] = "Report deleted successfully.";
        }
        else
        {
            TempData["Error"] = "Report not found.";
        }

        return RedirectToAction("ReportedJobs");
    }

}

