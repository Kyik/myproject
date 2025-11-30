using System.Net.Mail;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Demo.Controllers;

public class HomeController : Controller
{
    private readonly DB db;
    private readonly IWebHostEnvironment en;
    private readonly Helper hp;

    public HomeController(DB db, IWebHostEnvironment en, Helper hp)
    {
        this.db = db;
        this.en = en;
        this.hp = hp;
    }

    // GET: Home/Index
    public IActionResult Index()
    {
        /*
        var model = db.Jobs;
        return View(model);
        */
        return Redirect("Job");
    }

    public IActionResult Detail(string? id, int? starFilter)
    {
        // Retrieve the authenticated user's email from User.Identity.Name
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            // If the user is not logged in, redirect to login or handle unauthorized access
            return RedirectToAction("Login", "Account");
        }

        var model = db.Courses
                      .FirstOrDefault(s => s.Id == id);

        if (model == null)
        {
            return RedirectToAction("Index");
        }

        // Use the authenticated user's email
        ViewBag.UserEmail = userEmail;

        // Get reviews for the course, excluding deleted ones
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

        return View(model);
    }


    // GET: Home/Both
    [Authorize]
    public IActionResult Both()
    {
        return View();
    }

    // GET: Home/Member
    [Authorize(Roles = "Member")]
    public IActionResult Member()
    {
        return View();
    }

    // GET: Home/Instructor
    [Authorize(Roles = "Instructor")]
    public IActionResult Instructor()
    {
        return View();
    }

    // GET: Home/Admin
    [Authorize(Roles = "Admin")]
    public IActionResult Admin()
    {
        return View();
    }


    //Extra for Send Email

    // GET: Home/Email
    public IActionResult Email()
    {
        return View();
    }

    // POST: Home/Email
    [HttpPost]
    public IActionResult Email(EmailVM vm)
    {
        if (ModelState.IsValid)
        {
            // Construct email
            var mail = new MailMessage();
            mail.To.Add(new MailAddress(vm.Email, "My Lovely"));
            mail.Subject = vm.Subject;
            mail.Body = vm.Body;
            mail.IsBodyHtml = vm.IsBodyHtml;

            // File attachment (optional)
            var path = Path.Combine(en.ContentRootPath, "Secret.pdf");
            var att = new Attachment(path);
            mail.Attachments.Add(att);

            // Send email
            hp.SendEmail(mail);

            TempData["Info"] = "Email sent.";
            return RedirectToAction();
        }

        return View(vm);
    }
    // start from here, below all new add one

    // POST: Home/AddReview
    [HttpPost]
    public IActionResult AddReview(string courseId, string userId, string comment, int rating)
    {
        // Check if the user is authenticated
        if (!User.Identity.IsAuthenticated)
        {
            TempData["Error"] = "You must be logged in to add a review.";
            return RedirectToAction("Detail", new { id = courseId });
        }

        // Validate that courseId exists in courses and userId matches the logged-in user's ID
        if (db.Courses.Any(s => s.Id == courseId) && User.Identity.Name == userId)
        {
            // Define Malaysia Time Zone
            TimeZoneInfo malaysiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"); // "Singapore Standard Time" works for Malaysia
            DateTime malaysiaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, malaysiaTimeZone);

            // Use malaysiaTime wherever you save dates
            db.Reviews.Add(new Review
            {
                CourseId = courseId,
                UserId = userId,
                Comment = comment,
                Rating = rating,
                CreatedAt = malaysiaTime // Save Malaysia time
            });

            db.SaveChanges();
            TempData["Info"] = "Review added successfully.";
        }
        else
        {
            TempData["Error"] = "Invalid course or user.";
        }

        return Redirect("/Course/CourseDetail/" + courseId);
    }

    // GET: Home/AllReviews
    public IActionResult AllReviews(string sortOrder)
    {
        IQueryable<Review> activeReviews = db.Reviews
            .Include(r => r.Course)
            .Include(r => r.User)
            .Where(r => !r.IsDeleted); // Only active reviews

        IQueryable<Review> deletedReviews = db.Reviews
            .Include(r => r.Course)
            .Include(r => r.User)
            .Where(r => r.IsDeleted); // Only deleted reviews

        // Apply sorting for active reviews
        switch (sortOrder)
        {
            case "rating_asc":
                activeReviews = activeReviews.OrderBy(r => r.Rating);
                break;
            case "rating_desc":
                activeReviews = activeReviews.OrderByDescending(r => r.Rating);
                break;
            case "date_asc":
                activeReviews = activeReviews.OrderBy(r => r.CreatedAt);
                break;
            case "date_desc":
            default:
                activeReviews = activeReviews.OrderByDescending(r => r.CreatedAt);
                break;
        }

        ViewBag.SortOrder = sortOrder;
        ViewBag.DeletedReviews = deletedReviews.ToList(); // Pass deleted reviews to the view
        return View(activeReviews.ToList());
    }


    // POST: Home/DeleteReview
    [HttpPost]
    public IActionResult DeleteReview(int id)
    {
        var review = db.Reviews.Find(id);

        if (review != null)
        {
            review.IsDeleted = true; // Mark as deleted
            db.SaveChanges();

            TempData["Info"] = "Review deleted successfully.";
        }
        else
        {
            TempData["Error"] = "Review not found.";
        }

        return RedirectToAction("AllReviews");
    }

    // POST: Home/DeleteSelectedReviews
    [HttpPost]
    public IActionResult DeleteSelectedReviews(int[] selectedIds)
    {
        if (selectedIds != null && selectedIds.Length > 0)
        {
            var reviewsToDelete = db.Reviews.Where(r => selectedIds.Contains(r.Id));
            foreach (var review in reviewsToDelete)
            {
                review.IsDeleted = true; // Mark as deleted
            }

            db.SaveChanges();
            TempData["Info"] = $"{reviewsToDelete.Count()} review(s) deleted.";
        }
        else
        {
            TempData["Error"] = "No reviews selected for deletion.";
        }

        return RedirectToAction("AllReviews");
    }

    // POST: Home/DeleteAllReviews
    [HttpPost]
    public IActionResult DeleteAllReviews()
    {
        db.Reviews.ExecuteDelete();
        TempData["Info"] = "All reviews deleted successfully.";
        return RedirectToAction("AllReviews");
    }

    // POST: Home/RestoreReviews
    [HttpPost]
    public IActionResult RestoreReviews()
    {
        // Fetch deleted reviews (where IsDeleted is true)
        var deletedReviews = db.Reviews.Where(r => r.IsDeleted).ToList();

        // Restore the deleted reviews
        foreach (var review in deletedReviews)
        {
            review.IsDeleted = false;
        }

        db.SaveChanges();

        TempData["Info"] = $"{deletedReviews.Count} review(s) restored.";
        return RedirectToAction("AllReviews");
    }

    public IActionResult Reports()
    {
        return View();
    }


    // GET: Home/TopRatedCourses
    public IActionResult TopRatedCourses()
    {
        // Dynamically compute AverageRating and ReviewCount
        var courseRatings = db.Reviews
            .Where(r => !r.IsDeleted) // Only consider non-deleted reviews
            .GroupBy(r => r.CourseId) // Group reviews by CourseId
            .Select(g => new
            {
                CourseId = g.Key,
                ReviewCount = g.Count(), // Total number of reviews
                AverageRating = g.Average(r => r.Rating) // Average rating for the course
            });

        // Join computed ratings with Courses table
        var model = db.Courses
            .Join(courseRatings,
                course => course.Id,
                rating => rating.CourseId,
                (course, rating) => new TopRatedCourseVM
                {
                    CourseId = course.Id,
                    CourseName = course.Name,
                    Description = course.Description,
                    PhotoURL = course.PhotoURL,
                    AverageRating = rating.AverageRating,
                    ReviewCount = rating.ReviewCount
                })
            .OrderByDescending(c => c.AverageRating) // Sort by highest rating
            .ThenByDescending(c => c.ReviewCount) // Secondary sort by number of reviews
            .Take(10) // Limit to top 10 courses
            .ToList();

        return View(model);
    }

    public IActionResult ReviewSummary()
    {
        // Group reviews by Rating and calculate the count for each rating
        var reviewCounts = db.Reviews
            .Where(r => !r.IsDeleted) // Exclude deleted reviews
            .GroupBy(r => r.Rating)
            .Select(g => new ReviewSummary
            {
                Rating = g.Key, // The star rating (1-5)
                Count = g.Count(),
                Percentage = db.Reviews.Where(r => !r.IsDeleted).Count() > 0
                    ? Math.Round((double)g.Count() / db.Reviews.Where(r => !r.IsDeleted).Count() * 100, 2)
                    : 0
            })
            .OrderByDescending(r => r.Rating)
            .ToList();

        ViewBag.TotalReviews = db.Reviews.Where(r => !r.IsDeleted).Count();
        return View(reviewCounts);
    }

    [HttpGet]
    public IActionResult UserSummary()
    {
        try
        {
            var users = db.Users.ToList(); // Fetch users
            var userSummary = users
                .GroupBy(u => u.Role)
                .Select(g => new UserSummary
                {
                    UserType = g.Key,
                    Count = g.Count(),
                    Percentage = users.Count > 0
                        ? Math.Round((double)g.Count() / users.Count * 100, 2)
                        : 0
                })
                .OrderByDescending(u => u.Count)
                .ToList();

            ViewBag.TotalUsers = users.Count;
            return View(userSummary);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"An error occurred: {ex.Message}";
            return RedirectToAction("Error");
        }
    }

    [HttpPost]
    public IActionResult GenerateReviewSummaryPdf(string chartImage)
    {
        if (string.IsNullOrEmpty(chartImage))
        {
            TempData["Error"] = "No chart image received.";
            return RedirectToAction("ReviewSummary");
        }

        try
        {
            // Decode the base64 image
            byte[] chartBytes = Convert.FromBase64String(chartImage.Split(',')[1]);

            // Fetch review summary data
            var reviewSummary = db.Reviews
                .Where(r => !r.IsDeleted)
                .GroupBy(r => r.Rating)
                .Select(g => new ReviewSummary
                {
                    Rating = g.Key,
                    Count = g.Count(),
                    Percentage = db.Reviews.Count(r => !r.IsDeleted) > 0
                        ? Math.Round((double)g.Count() / db.Reviews.Count(r => !r.IsDeleted) * 100, 2)
                        : 0
                })
                .OrderByDescending(r => r.Rating)
                .ToList();

            // Get the current date
            var currentDate = DateTime.Now.ToString("MMMM dd, yyyy");

            // Create the PDF
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var pdfDocument = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    // Add the header with the current date
                    page.Header().Row(row =>
                    {
                        row.RelativeColumn().Text("Lickit Job Posting : Review Summary").FontSize(20).Bold();
                        row.ConstantColumn(100).AlignRight().Text(currentDate).FontSize(11).Italic();
                    });

                    page.Content().Column(column =>
                    {
                        // Add table for review summary
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Star Rating").Bold();
                                header.Cell().Text("Count").Bold();
                                header.Cell().Text("Percentage").Bold();
                            });

                            foreach (var review in reviewSummary)
                            {
                                table.Cell().Text($"{review.Rating} Stars");
                                table.Cell().Text(review.Count.ToString());
                                table.Cell().Text($"{review.Percentage:F2}%");
                            }
                        });

                        // Add space between table and chart
                        column.Item().PaddingVertical(20); // Adds vertical spacing (20 points)

                        // Add chart image to the PDF
                        if (chartBytes.Length > 0)
                        {
                            column.Item().Image(chartBytes).FitWidth(); // Ensure the chart fits the page width
                        }
                    });
                });
            });

            var pdfBytes = pdfDocument.GeneratePdf();
            return File(pdfBytes, "application/pdf", "ReviewSummaryReport.pdf");
        }
        catch (Exception ex)
        {
            TempData["Error"] = "An error occurred while generating the PDF: " + ex.Message;
            return RedirectToAction("ReviewSummary");
        }
    }

    [HttpPost]
    public IActionResult GenerateTopRatedCoursesPdf(string chartImage)
    {
        if (string.IsNullOrEmpty(chartImage))
        {
            TempData["Error"] = "No chart image received.";
            return RedirectToAction("TopRatedCourses");
        }

        try
        {
            // Decode the base64 image
            byte[] chartBytes = Convert.FromBase64String(chartImage.Split(',')[1]);

            // Fetch top-rated courses data
            var courseRatings = db.Reviews
                .Where(r => !r.IsDeleted)
                .GroupBy(r => r.CourseId)
                .Select(g => new
                {
                    CourseId = g.Key,
                    ReviewCount = g.Count(),
                    AverageRating = g.Average(r => r.Rating)
                });

            var model = db.Courses
                .Join(courseRatings,
                    course => course.Id,
                    rating => rating.CourseId,
                    (course, rating) => new TopRatedCourseVM
                    {
                        CourseId = course.Id,
                        CourseName = course.Name,
                        Description = course.Description,
                        PhotoURL = course.PhotoURL,
                        AverageRating = rating.AverageRating,
                        ReviewCount = rating.ReviewCount
                    })
                .OrderByDescending(c => c.AverageRating)
                .ThenByDescending(c => c.ReviewCount)
                .Take(10)
                .ToList();

            // Get current date
            var currentDate = DateTime.Now.ToString("MMMM dd, yyyy");

            // Generate PDF using QuestPDF
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var pdfDocument = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    // Header
                    page.Header().Row(row =>
                    {
                        row.RelativeColumn().Text("Lickit Job Posting : Top Rated Courses Report").FontSize(20).Bold();
                        row.ConstantColumn(100).AlignRight().Text(currentDate).FontSize(11).Italic();
                    });

                    // Content
                    page.Content().Column(column =>
                    {
                        // Add table for top-rated courses
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100); // Photo column
                                columns.RelativeColumn(); // Course Name
                                columns.RelativeColumn(); // Description
                                columns.ConstantColumn(80); // Avg Rating
                                columns.ConstantColumn(80); // Reviews
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Photo").Bold();
                                header.Cell().Text("Course Name").Bold();
                                header.Cell().Text("Description").Bold();
                                header.Cell().Text("Avg Rating").Bold();
                                header.Cell().Text("Reviews").Bold();
                            });

                            foreach (var course in model)
                            {
                                table.Cell().Element(cell =>
                                {
                                    if (!string.IsNullOrEmpty(course.PhotoURL))
                                    {
                                        var imageBytes = System.IO.File.Exists("wwwroot/courses/" + course.PhotoURL)
                                            ? System.IO.File.ReadAllBytes("wwwroot/courses/" + course.PhotoURL)
                                            : null;

                                        if (imageBytes != null)
                                        {
                                            cell.Image(imageBytes, ImageScaling.FitWidth);
                                        }
                                        else
                                        {
                                            cell.Text("No Image1");
                                        }
                                    }
                                    else
                                    {
                                        cell.Text("No Image");
                                    }
                                });

                                table.Cell().Text(course.CourseName);
                                table.Cell().Text(course.Description);
                                table.Cell().Text(course.AverageRating.ToString("F1"));
                                table.Cell().Text(course.ReviewCount.ToString());
                            }
                        });

                        // Add chart to the PDF
                        column.Item().PaddingVertical(20); // Add spacing
                        column.Item().Image(chartBytes).FitWidth(); // Embed chart image
                    });
                });
            });

            var pdfBytes = pdfDocument.GeneratePdf();
            return File(pdfBytes, "application/pdf", "TopRatedCoursesReport.pdf");
        }
        catch (Exception ex)
        {
            TempData["Error"] = "An error occurred while generating the PDF: " + ex.Message;
            return RedirectToAction("TopRatedCourses");
        }
    }

    [HttpPost]
    public IActionResult GenerateUserSummaryPdf(string chartImage)
    {
        if (string.IsNullOrEmpty(chartImage))
        {
            TempData["Error"] = "No chart image received.";
            return RedirectToAction("UserSummary");
        }

        try
        {
            // Decode the base64 chart image
            byte[] chartBytes;
            try
            {
                string base64Data = chartImage.Contains(",") ? chartImage.Split(',')[1] : chartImage;
                chartBytes = Convert.FromBase64String(base64Data);
            }
            catch
            {
                TempData["Error"] = "Invalid chart image format.";
                return RedirectToAction("UserSummary");
            }

            // Perform grouping on client-side fetched data
            var users = db.Users.ToList();
            var userSummary = users
                .GroupBy(u => u.Role)
                .Select(g => new UserSummary
                {
                    UserType = g.Key,
                    Count = g.Count(),
                    Percentage = users.Count > 0
                        ? Math.Round((double)g.Count() / users.Count * 100, 2)
                        : 0
                })
                .OrderByDescending(u => u.Count)
                .ToList();

            var currentDate = DateTime.Now.ToString("MMMM dd, yyyy");

            // Generate PDF document
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var pdfDocument = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    // Header
                    page.Header().Row(row =>
                    {
                        row.RelativeColumn().Text("Lickit Job Posting : User Summary Report").FontSize(20).Bold();
                        row.ConstantColumn(100).AlignRight().Text(currentDate).FontSize(11).Italic();
                    });

                    // Content
                    page.Content().Column(column =>
                    {
                        // Table for user summary
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("User Type").Bold();
                                header.Cell().Text("Count").Bold();
                                header.Cell().Text("Percentage").Bold();
                            });

                            foreach (var user in userSummary)
                            {
                                table.Cell().Text(user.UserType ?? "Unknown");
                                table.Cell().Text(user.Count.ToString());
                                table.Cell().Text($"{user.Percentage:F2}%");
                            }
                        });

                        column.Item().PaddingVertical(20);
                        column.Item().Image(chartBytes).FitWidth(); // Embed chart image
                    });
                });
            });

            var pdfBytes = pdfDocument.GeneratePdf();
            return File(pdfBytes, "application/pdf", "UserSummaryReport.pdf");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"An error occurred while generating the PDF: {ex.Message}";
            return RedirectToAction("UserSummary");
        }
    }

}
