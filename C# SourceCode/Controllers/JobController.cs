using JobPostingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace Demo.Controllers;

public class JobController : Controller
{
    private readonly DB db;
    private readonly IWebHostEnvironment en;
    private readonly Helper hp;

    public JobController(DB db, IWebHostEnvironment en, Helper hp)
    {
        this.db = db;
        this.en = en;
        this.hp = hp;
    }

    // GET: Job/Index
    [Authorize]
    public IActionResult Index(string? title, string? sort, string? dir, int page = 1)
    {
        // (1) Searching ------------------------
        ViewBag.Title = title = title?.Trim() ?? "";

        var searched = db.Jobs.Where(s => s.Title.Contains(title));

        // (2) Sorting --------------------------
        ViewBag.Sort = sort;
        ViewBag.Dir = dir;

        Func<Job, object> fn = sort switch //macam is those can sort eh column
        {
            "Id" => s => s.Id,
            "Title" => s => s.Title,
            "Type" => s => s.Type,
            "Description" => s => s.Description,
            _ => s => s.Id,
        };

        var sorted = dir == "des" ?
                     searched.OrderByDescending(fn) :
                     searched.OrderBy(fn);

        // (3) Paging ---------------------------
        if (page < 1)
        {
            return RedirectToAction(null, new { title, sort, dir, page = 1 });
        }

        var m = sorted.ToPagedList(page, 10);

        if (page > m.PageCount && m.PageCount > 0)
        {
            return RedirectToAction(null, new { title, sort, dir, page = m.PageCount });
        }


        if (Request.IsAjax())
        {
            return PartialView("_Job", m);
        }


        return View(m);
        //var model = db.Jobs;
        //return View(model);
    }

    private string NextJobId()
    {
        string max = db.Jobs.Max(p => p.Id) ?? "J000";
        int n = int.Parse(max[1..]);
        return (n + 1).ToString("'J'000");
    }
    private string NextApplicationId()
    {
        string max = db.Applications.Max(p => p.Id) ?? "A000";
        int n = int.Parse(max[1..]);
        return (n + 1).ToString("'A'000");
    }

    [Authorize(Roles = "Member")]
    public IActionResult ApplicationHistory()
    {
        var s = db.Applications.Where(x => x.UserEmail == User.Identity!.Name).Include(x => x.Job);
        return View(s);
    }

    public IActionResult PostingDetail(string? id)
    {
        var s = db.Applications.Where(x => x.JobId == id).Include(x => x.Job);
        return View(s);
    }

    // POST: Job/Approve
    [HttpPost]
    public IActionResult Approve(string? id)
    {
        var p = db.Applications.Find(id);

        if (p != null)
        {
            p.Status = 1;
            db.SaveChanges();

            TempData["Info"] = "Application Accepted";
        }

        return Redirect("/Job/PostingDetail/" + p.JobId);
    }

    // POST: Job/Reject
    [HttpPost]
    public IActionResult Reject(string? id)
    {
        var p = db.Applications.Find(id);

        if (p != null)
        {
            p.Status = 2;
            db.SaveChanges();

            TempData["Info"] = "Application Rejected!";
        }

        return Redirect("/Job/PostingDetail/" + p.JobId);
    }

    [Authorize(Roles = "Employer")]
    public IActionResult PostingHistory()
    {
        var s = db.Jobs;//.Where(x => x.EmployerEmail == User.Identity!.Name);
        return View(s);
    }

    [Authorize(Roles = "Employer, Admin")]
    public IActionResult Insert()
    {
        var vm = new JobInsertVM
        {
            Id = NextJobId(),
        };

        return View(vm);
    }

    // POST: Home/Insert
    [HttpPost]
    public IActionResult Insert(JobInsertVM vm)
    {
        // TODO
        if (ModelState.IsValid("Id") && db.Jobs.Any(s => s.Id == vm.Id))
        {
            ModelState.AddModelError("Id", "Duplicated Id.");
        }

        if (ModelState.IsValid("Photo"))
        {
            var e = hp.ValidatePhoto(vm.Photo);
            if (e != "") ModelState.AddModelError("Photo", e);
        }

        if (ModelState.IsValid("MinSalary") && ModelState.IsValid("MaxSalary"))
        {
            if (vm.MinSalary > vm.MaxSalary) ModelState.AddModelError("MinSalary", "Minimum Salary cannot be more than Maximum Salary");
        }

        if (ModelState.IsValid)
        {
            db.Jobs.Add(new()
            {
                Id = vm.Id.Trim().ToUpper(),
                Title = vm.Title.Trim().ToUpper(),
                Type = vm.Type,
                Description = vm.Description,
                CompanyName = vm.CompanyName,
                Location = vm.Location,
                EducationLevel = vm.EducationLevel,
                ExperienceLevel = vm.ExperienceLevel,
                MinSalary = vm.MinSalary,
                MaxSalary = vm.MaxSalary,
                PhotoURL = hp.SavePhoto(vm.Photo, "jobPics"),
            });
            db.SaveChanges();

            TempData["Info"] = "Record inserted.";
            return RedirectToAction("Index");
        }
        return View();
    }

    // GET: Home/Update
    public IActionResult Update(string? id)
    {
        var s = db.Jobs.Find(id);

        if (s == null)
        {
            return RedirectToAction("Index");
        }

        // TODO
        var vm = new JobUpdateVM
        {
            Id = s.Id.Trim().ToUpper(),
            Title = s.Title.Trim().ToUpper(),
            Type = s.Type,
            Description = s.Description,
            CompanyName = s.CompanyName,
            Location = s.Location,
            EducationLevel = s.EducationLevel,
            ExperienceLevel = s.ExperienceLevel,
            MinSalary = s.MinSalary,
            MaxSalary = s.MaxSalary,
            PhotoURL = s.PhotoURL,
        };

        return View(vm);
    }

    // GET: Job/JobDetail
    public IActionResult JobDetail(string? id)
    {
        var user = db.Users.Find(User.Identity!.Name);
       

        var email = user.Email;
        var p = db.Jobs.Find(id);
        var favouriteIds = db.Favourites
          .Where(f => f.UserEmail == email)
          .Select(f => f.JobId)
          .ToList();

        ViewBag.FavouriteIds = favouriteIds;

        if (p == null)
        {
            return RedirectToAction("Index");
        }

        return View(p);
    }

    // GET: Job/ApplyJob
    public IActionResult ApplyJob(string? id)
    {
        var p = db.Jobs.Find(id);

        if (p == null)
        {
            return RedirectToAction("Index");
        }

        if (db.Applications.Any(p => p.JobId == id && p.UserEmail == User.Identity.Name))
        {
            TempData["Info"] = "You have already applied for this job.";
            return RedirectToAction("Index");
        }

        var vm = new ApplyJobVM
        {
            ApplicationId = NextApplicationId(),
            JobId = id,
            JobTitle = p.Title,
        };

        return View(vm);
    }

    // POST: Job/ApplyJob
    [HttpPost]
    public IActionResult ApplyJob(ApplyJobVM vm)
    {
        if (ModelState.IsValid("Resume"))
        {
            var e = hp.ValidatePhoto(vm.Resume);
            if (e != "") ModelState.AddModelError("Resume", e);
        }

        if (ModelState.IsValid("CoverLetter"))
        {
            var e = hp.ValidatePhoto(vm.CoverLetter);
            if (e != "") ModelState.AddModelError("CoverLetter", e);
        }
        if (ModelState.IsValid)
        {
            db.Applications.Add(new()
            {
                Id = vm.ApplicationId,
                JobId = vm.JobId,
                ResumeURL = hp.SavePhoto(vm.Resume, "applicationPics"),
                CoverLetterURL = hp.SavePhoto(vm.CoverLetter, "applicationPics"),
                UserEmail = vm.UserEmail,
            });
            db.SaveChanges();

            TempData["Info"] = "Record inserted.";
            return RedirectToAction("Index");

        }
        /*
        if (!ModelState.IsValid("CoverLetter"))
        {
            return RedirectToAction("Course");
        }
        */
        //TempData["Info"] = "Record inserted.";
        //return RedirectToAction("Index");
        return View(vm);
    }

    // POST: Home/Update
    [HttpPost]
    public IActionResult Update(JobUpdateVM vm)
    {
        var s = db.Jobs.Find(vm.Id);

        if (s == null)
        {
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid("MinSalary") && ModelState.IsValid("MaxSalary"))
        {
            if (vm.MinSalary > vm.MaxSalary) ModelState.AddModelError("MinSalary", "Minimum Salary cannot be more than Maximum Salary");
        }

        //if (ModelState.IsValid)
        //{
        s.Title = vm.Title.Trim().ToUpper();
        s.Type = vm.Type;
        s.Description = vm.Description;
        s.CompanyName = vm.CompanyName;
        s.Location = vm.Location;
        s.EducationLevel = vm.EducationLevel;
        s.ExperienceLevel = vm.ExperienceLevel;
        s.MinSalary = vm.MinSalary;
        s.MaxSalary = vm.MaxSalary;
        if (vm.Photo != null)
        {
            hp.DeletePhoto(vm.PhotoURL, "jobPics");
            s.PhotoURL = hp.SavePhoto(vm.Photo, "jobPics");
        }
        db.SaveChanges();

        TempData["Info"] = "Record updated.";
        return RedirectToAction("Index");
        //}

        return View(vm);
    }

    // POST: Home/Delete
    [HttpPost]
    public IActionResult Delete(string? id)
    {
        var s = db.Jobs.Find(id);

        if (s != null)
        {
            db.Jobs.Remove(s);
            db.SaveChanges();

            TempData["Info"] = "Record deleted.";
        }

        return RedirectToAction("Index");
    }

    // POST: Home/DeleteMany
    [HttpPost]
    public IActionResult DeleteMany(string[] ids)
    {
        // TODO
        int n = db.Jobs.Where(s => ids.Contains(s.Id)).ExecuteDelete();

        TempData["Info"] = $"{n} record(s) deleted.";
        return RedirectToAction("Index");
    }
    /*
    // POST: Home/Restore
    [HttpPost]
    public IActionResult Restore()
    {
        // (1) Delete all records
        db.Students.ExecuteDelete();

        // ------------------------------------------------

        // (2) Insert all records from "Students.txt"
        string path = Path.Combine(en.ContentRootPath, "Students.txt");

        foreach (string line in System.IO.File.ReadLines(path))
        {
            if (line.Trim() == "") continue;

            var data = line.Split("\t", StringSplitOptions.TrimEntries);

            db.Students.Add(new()
            {
                Id = data[0],
                Name = data[1],
                Gender = data[2],
                ProgramId = data[3],
            });
        }

        db.SaveChanges();

        TempData["Info"] = "Record(s) restored.";
        return RedirectToAction("Demo");
    }
    */

    [HttpPost]
    [Authorize]
    public IActionResult SubmitReport(ReportVM vm)
    {
        if (ModelState.IsValid)
        {
            var report = new Report
            {
                Id = Guid.NewGuid().ToString("N").Substring(0, 10), // 10-character unique ID
                JobID = vm.JobID,
                ReporterEmail = vm.ReporterEmail,
                Reason = vm.Reason,
                AddReason = vm.AddReason
            };

            db.Reports.Add(report);
            db.SaveChanges();

            TempData["Info"] = "Report submitted successfully.";
            return RedirectToAction("JobDetail", new { id = vm.JobID });
        }

        // Reload the student and display the form again
        var student = db.Jobs.Find(vm.JobID);
        if (student == null)
        {
            return NotFound();
        }

        return View("Detail", Tuple.Create(student, vm));
    }

  
    


}