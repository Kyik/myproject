using JobPostingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Demo.Controllers;

public class CourseController : Controller
{
    private readonly DB db;
    private readonly IWebHostEnvironment en;
    private readonly Helper hp;

    public CourseController(DB db, IWebHostEnvironment en, Helper hp)
    {
        this.db = db;
        this.en = en;
        this.hp = hp;
    }

    // GET: Course/Index
    [Authorize]
    public IActionResult Index()
    {
        var model = db.Courses.Include(c => c.Quiz).Include(c => c.Chapters);
        return View(model);
    }


    // GET: Course/CheckId
    public bool CheckId(string id)
    {
        return !db.Courses.Any(p => p.Id == id);
    }

    private string NextId()
    {
        string max = db.Courses.Max(p => p.Id) ?? "C000";
        int n = int.Parse(max[1..]);
        return (n + 1).ToString("'C'000");
    }

    private string NextChapterId()
    {
        string max = db.Chapters.Max(p => p.Id) ?? "H000";
        int n = int.Parse(max[1..]);
        return (n + 1).ToString("'H'000");
    }
    private string NextQuizId()
    {
        string max = db.Quizzes.Max(p => p.Id) ?? "Q000";
        int n = int.Parse(max[1..]);
        return (n + 1).ToString("'Q'000");
    }
    private string NextQuestionId()
    {
        string max = db.Questions.Max(p => p.Id) ?? "Z000";
        int n = int.Parse(max[1..]);
        return (n + 1).ToString("'Z'000");
    }

    // GET: Course/Insert
    [Authorize(Roles = "Admin")]
    public IActionResult Insert()
    {
        var vm = new CourseInsertVM
        {
            Id = NextId(),
            ChapterId = NextChapterId(),
            QuizId = NextQuizId(),
            QuestionId = NextQuestionId(),
        };

        return View(vm);
    }

    // POST: Course/Insert
    [HttpPost]
    public IActionResult Insert(CourseInsertVM vm)
    {
        if (ModelState.IsValid("Id") && db.Courses.Any(p => p.Id == vm.Id))
        {
            ModelState.AddModelError("Id", "Duplicated Id.");
        }

        if (ModelState.IsValid("Photo"))
        {
            var e = hp.ValidatePhoto(vm.Photo);
            if (e != "") ModelState.AddModelError("Photo", e);
        }

        if (ModelState.IsValid)
        {
            db.Courses.Add(new()
            {
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description,
                PhotoURL = hp.SavePhoto(vm.Photo, "courses"),
            });
            string max = vm.ChapterId;
            int temp = 0;
            foreach (var s in vm.ChapterName)
            {
                db.Chapters.Add(new()
                {
                    Id = max,
                    Name = vm.ChapterName[temp],
                    Description = vm.ChapterDescription[temp],
                    VideoURL = hp.SaveVideo(vm.Video[temp], "coursesVid"),
                    CourseId = vm.Id,
                });
                int n = int.Parse(max[1..]);
                max = (n + 1).ToString("'H'000");
                temp++;
            }
            //Quiz and Question Insert
            db.Quizzes.Add(new()
            {
                Id = vm.QuizId,
                Name = vm.QuizName,
                Description = vm.QuizDescription,
                Duration = vm.QuizDuration,
                CourseId = vm.Id,
            });
            string maxQuestionId = vm.QuestionId;
            int questionTemp = 0;
            foreach (var s in vm.QuestionName)
            {
                db.Questions.Add(new()
                {
                    Id = maxQuestionId,
                    Name = vm.QuestionName[questionTemp],
                    OptionA = vm.OptionA[questionTemp],
                    OptionB = vm.OptionB[questionTemp],
                    OptionC = vm.OptionC[questionTemp],
                    OptionD = vm.OptionD[questionTemp],
                    QuizId = vm.QuizId,
                });
                int n = int.Parse(maxQuestionId[1..]);
                maxQuestionId = (n + 1).ToString("'Z'000");
                questionTemp++;
            }
            db.SaveChanges();

            TempData["Info"] = "Course inserted.";
            return RedirectToAction("Index");
        }

        return View();
    }

    // GET: Course/Update
    public IActionResult Update(string? id)
    {
        var p = db.Courses.Include(c => c.Chapters).FirstOrDefault(c => c.Id == id);

        if (p == null)
        {
            return RedirectToAction("Index");
        }

        var vm = new CourseUpdateVM
        {
            Id = p.Id,
            Name = p.Name,
            PhotoURL = p.PhotoURL,
            Chapters = p.Chapters,
        };

        return View(vm);
    }

    // POST: Course/Update
    [HttpPost]
    public IActionResult Update(CourseUpdateVM vm)
    {
        var p = db.Courses.Find(vm.Id);

        if (p == null)
        {
            return RedirectToAction("Index");
        }

        if (vm.Photo != null)
        {
            var e = hp.ValidatePhoto(vm.Photo);
            if (e != "") ModelState.AddModelError("Photo", e);
        }

        if (ModelState.IsValid)
        {
            p.Name = vm.Name;
            //p.Price = vm.Price;

            if (vm.Photo != null)
            {
                hp.DeletePhoto(p.PhotoURL, "courses");
                p.PhotoURL = hp.SavePhoto(vm.Photo, "courses");
            }
            db.SaveChanges();

            TempData["Info"] = "Course updated.";
            return RedirectToAction("Index");
        }

        vm.PhotoURL = p.PhotoURL;
        return View(vm);
    }

    // POST: Course/Delete
    [HttpPost]
    public IActionResult Delete(string? id)
    {
        var p = db.Courses.Find(id);

        if (p != null)
        {
            hp.DeletePhoto(p.PhotoURL, "courses");
            db.Courses.Remove(p);
            db.SaveChanges();

            TempData["Info"] = "Course deleted.";
        }

        return RedirectToAction("Index");
    }

    public IActionResult CourseDetail(string? id, string? chapter, int? starFilter)
    {
        var chapterId = chapter?.Trim() ?? "";
        var p = db.Courses
            .Include(c => c.Quiz)
            .Include(c => c.Chapters)
            .FirstOrDefault(c => c.Id == id);

        if (p == null)
        {
            return RedirectToAction("Index");
        }

        if (Request.IsAjax())
        {
            Chapter tempChapter = db.Chapters.Find(chapterId);
            //List<Chapter> tempChapterList = new List<Chapter>();
            //tempChapterList.Add(tempChapter);
            List<Chapter> tempChapterList = [tempChapter];
            p.Chapters = tempChapterList;
            return PartialView("_Chapter", p);
        }

        // Retrieve the authenticated user's email from User.Identity.Name
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            // If the user is not logged in, redirect to login or handle unauthorized access
            return RedirectToAction("Login", "Account");
        }

        // Use the authenticated user's email
        ViewBag.UserEmail = userEmail;

        // Get reviews for the student/course, excluding deleted ones
        var reviews = db.Reviews
                        .Include(r => r.User)
                        .Where(r => r.CourseId == id && !r.IsDeleted) // Filter out deleted reviews
                        .OrderByDescending(r => r.CreatedAt)
                        .ToList();

        // Filter reviews by star rating if a filter is selected
        if (starFilter.HasValue)
        {
            reviews = reviews.Where(r => r.Rating == starFilter.Value).ToList();
        }

        // Calculate average rating
        var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

        // Calculate star breakdown
        var starBreakdown = new Dictionary<int, int>();
        for (int i = 1; i <= 5; i++)
        {
            starBreakdown[i] = db.Reviews.Count(r => r.CourseId == id && r.Rating == i && !r.IsDeleted);
        }

        // Calculate maximum reviews for scaling
        int maxReviews = starBreakdown.Values.Max();

        ViewBag.AverageRating = averageRating;
        ViewBag.StarBreakdown = starBreakdown;
        ViewBag.TotalReviews = db.Reviews.Count(r => r.CourseId == id && !r.IsDeleted);
        ViewBag.MaxReviews = maxReviews;
        ViewBag.StarFilter = starFilter; // Pass the selected filter to the view
        ViewBag.Reviews = reviews;

        return View(p);
    }
    public IActionResult Quiz(string? id)
    {
        var p = db.Quizzes
            .Include(c => c.Course)
            .ThenInclude(c => c.Chapters)
            .Include(c => c.Questions)
            .FirstOrDefault(c => c.Id == id);
        QuizVM vm = new QuizVM();
        vm.Quiz = p;
        vm.Id = p.Id;

        if (p == null)
        {
            return RedirectToAction("Index");
        }

        return View(vm);
    }


    [HttpPost]
    public IActionResult Quiz(QuizVM vm)
    {
        var p = db.Quizzes
            .Include(c => c.Course)
            .ThenInclude(c => c.Chapters)
            .Include(c => c.Questions)
            .FirstOrDefault(c => c.Id == vm.Id);
        vm.Quiz = p;

        if (vm.Answers.IsNullOrEmpty())
        {
            ModelState.AddModelError("Answers", "Please select an answer");
        }

        if (ModelState.IsValid("Id") && ModelState.IsValid("Answers"))
        {
            var cc = db.CompletedCourses.FirstOrDefault(p => p.UserEmail == User.Identity!.Name && p.CourseId == vm.Quiz.CourseId);
            var ccmax = "";

            foreach (var s in vm.Answers)
            {
                if (s.IsNullOrEmpty())
                {
                    ModelState.AddModelError("Answers", "Please select an answer");
                }
            }

            var delete = db.UserAnswers.Where(p => p.UserEmail == User.Identity!.Name && p.Question.QuizId == vm.Quiz.Id);

            if (!delete.IsNullOrEmpty()) //if never play quiz before
            {
                foreach (var s in delete)
                {
                    db.UserAnswers.Remove(s);
                }
                ccmax = cc.Id;
            }
            else //if already play quiz before
            {
                ccmax = db.CompletedCourses.Max(p => p.Id) ?? "W000";
                int ccn = int.Parse(ccmax[1..]);
                ccmax = (ccn + 1).ToString("'W'000");
            }

            string max = db.UserAnswers.Max(p => p.Id) ?? "Y000";
            int temp = 0, correct = 0;
            foreach (var s in vm.Quiz.Questions)
            {
                int n = int.Parse(max[1..]);
                max = (n + 1).ToString("'Y'000");
                int status;
                if (vm.Answers[temp] == vm.Quiz.Questions[temp].Answer)
                {
                    status = 1;
                    correct++;
                }
                else
                {
                    status = 0;
                }
                db.UserAnswers.Add(new()
                {
                    Id = max,
                    ChosenAnswer = vm.Answers[temp],
                    Status = status,
                    Duration = 0,
                    QuestionId = vm.Quiz.Questions[temp].Id,
                    CompletedCourseId = ccmax,
                    UserEmail = User.Identity!.Name,
                });
                temp++;
            }
            if (!delete.IsNullOrEmpty())
            {
                cc.NoOfTries++;
                cc.Percentage = Math.Round((double)correct / (double)temp, 2) * 100;
            }
            else
            {
                db.CompletedCourses.Add(new()
                {
                    Id = ccmax,
                    CourseName = vm.Quiz.Course.Name,
                    CourseId = vm.Quiz.CourseId,
                    Percentage = Math.Round((double)correct / (double)temp, 2) * 100,
                    NoOfTries = 1,
                    UserEmail = User.Identity!.Name,
                });
            }
            db.SaveChanges();

            TempData["Info"] = "Quiz Answered!";
            return RedirectToAction("Index");
        }

        return View(vm);
    }

    public IActionResult QuizHistory(string? courseName)
    {
        courseName = courseName?.Trim() ?? "";
        var p = db.CompletedCourses
            .Where(c => c.CourseName.Contains(courseName))
            .Include(c => c.Course)
            .ThenInclude(c => c.Quiz)
            .Include(c => c.Course)
            .ThenInclude(c => c.Chapters);

        if (p == null)
        {
            return RedirectToAction("Index");
        }

        if (Request.IsAjax())
        {
            return PartialView("_QuizHistory", p);
        }

        return View(p);
    }

    public IActionResult QuizHistoryDetail(string id)
    {
        id = id.Trim();
        var p = db.CompletedCourses
            .Include(c => c.User)
            .ThenInclude(c => c.UserAnswers.Where(x => x.CompletedCourseId == id))
            .Include(c => c.Course)
            .ThenInclude(c => c.Quiz)
            .ThenInclude(c => c.Questions)
            .Include(c => c.Course)
            .ThenInclude(c => c.Chapters)
            .FirstOrDefault(c => c.Id.Contains(id));

        if (p == null)
        {
            return RedirectToAction("Index");
        }

        return View(p);
    }
}

