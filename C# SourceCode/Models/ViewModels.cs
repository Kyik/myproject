using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace JobPostingSystem.Models;

// View Models ----------------------------------------------------------------

#nullable disable warnings

public class JobInsertVM
{
    [StringLength(4)]
    [RegularExpression(@"[J]\d{3}$", ErrorMessage = "Invalid {0}.")]
    public string Id { get; set; }

    [StringLength(100)]
    public string Title { get; set; }

    public int Type { get; set; }

    [StringLength(100)]
    public string Description { get; set; }

    [StringLength(100)]
    public string CompanyName { get; set; }

    [StringLength(100)]
    public string Location { get; set; }

    public string EducationLevel { get; set; }

    public string ExperienceLevel { get; set; }
    public int MinSalary { get; set; }
    public int MaxSalary { get; set; }
    public DateOnly Deadline { get; set; }
    public IFormFile Photo { get; set; }
}

public class ApplyJobVM
{
    //[StringLength(4)]
    //[RegularExpression(@"[A]\d{3}$", ErrorMessage = "Invalid {0}.")]
    public string ApplicationId { get; set; }
    //[StringLength(4)]
    //[RegularExpression(@"[J]\d{3}$", ErrorMessage = "Invalid {0}.")]
    public string JobId { get; set; }
    //[StringLength(100)]
    public string JobTitle { get; set; }
    public IFormFile Resume { get; set; }
    public IFormFile CoverLetter { get; set; }
    // [StringLength(100)]
    //[RegularExpression(@"[A]\d{3}$", ErrorMessage = "Invalid {0}.")]
    public string UserEmail { get; set; }
}


public class JobUpdateVM
{
    [StringLength(4)]
    [RegularExpression(@"[J]\d{3}$", ErrorMessage = "Invalid {0}.")]
    public string Id { get; set; }

    [StringLength(100)]
    public string Title { get; set; }

    public int Type { get; set; }

    [StringLength(100)]
    public string Description { get; set; }

    [StringLength(100)]
    public string CompanyName { get; set; }

    [StringLength(100)]
    public string Location { get; set; }

    [StringLength(100)]
    public string Cover { get; set; }

    public string EducationLevel { get; set; }

    public string ExperienceLevel { get; set; }
    public int MinSalary { get; set; }
    public int MaxSalary { get; set; }
    public DateOnly PostDate { get; set; }
    public DateOnly Deadline { get; set; }
    public IFormFile Photo { get; set; }
    public string PhotoURL { get; set; }
}

public class LoginVM
{
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    [StringLength(100, MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
public class EducationVM
{
    [StringLength(4)]
    //[RegularExpression(@"E\d{3}", ErrorMessage = "Invalid {0} format.")]
    //[Remote("CheckId", "Education", ErrorMessage = "Duplicated {0}.")]
    public string Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Course { get; set; }

    [Required]
    [MaxLength(200)]
    public string Institution { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }


    public string UserEmail { get; set; }
}

public class UpdateEducationVM
{
    [StringLength(4)]
    //[RegularExpression(@"E\d{3}", ErrorMessage = "Invalid {0} format.")]
    //[Remote("CheckId", "Education", ErrorMessage = "Duplicated {0}.")]
    public string? Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Course { get; set; }

    [Required]
    [MaxLength(200)]
    public string Institution { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }


    public string? UserEmail { get; set; }
}

public class CareerVM
{
    [StringLength(4)]
    //[RegularExpression(@"E\d{3.}", ErrorMessage = "Invalid {0} format.")]
    //[Remote("CheckId", "Education", ErrorMessage = "Duplicated {0}.")]
    public string Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Career { get; set; }

    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; }

    public DateTime? StartDatec { get; set; }

    public DateTime? EndDatec { get; set; }




    public string UserEmail { get; set; }
}

public class UpdateCareerVM
{
    [StringLength(4)]
    //[RegularExpression(@"E\d{3.}", ErrorMessage = "Invalid {0} format.")]
    //[Remote("CheckId", "Education", ErrorMessage = "Duplicated {0}.")]
    public string? Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Career { get; set; }

    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; }

    public DateTime? StartDatec { get; set; }

    public DateTime? EndDatec { get; set; }




    public string? UserEmail { get; set; }
}


public class RegisterMVM
{
    [StringLength(100)]
    [EmailAddress]
    [Remote("CheckEmail", "Account", ErrorMessage = "Duplicated {0}.")]
    public string Email { get; set; }

    [StringLength(100, MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [StringLength(100, MinimumLength = 5)]
    [Compare("Password")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string Confirm { get; set; }

    [StringLength(100)]
    public string Name { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateOnly DateOfBirth { get; set; }

    [StringLength(1)]
    [RegularExpression(@"^[FM]$", ErrorMessage = " Invalid {0}")]
    public string Gender { get; set; }

    [StringLength(11, MinimumLength = 10, ErrorMessage = "Invalid Contact Number")]
    [RegularExpression(@"^01\d{8,9}$", ErrorMessage = "Invalid Contact Number")]
    [Display(Name = "Contact Number")]
    public string Contact { get; set; }



    [StringLength(150, ErrorMessage = "Content exceed number of character.")]
    public string Address { get; set; }




    public IFormFile Photo { get; set; }
}

public class UpdatePasswordVM
{
    [StringLength(100, MinimumLength = 5)]
    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string Current { get; set; }

    [StringLength(100, MinimumLength = 5)]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string New { get; set; }

    [StringLength(100, MinimumLength = 5)]
    [Compare("New")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string Confirm { get; set; }
}

public class UpdateProfileVM
{
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string? Email { get; set; }

    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [StringLength(11, MinimumLength = 10, ErrorMessage = "Invalid Contact Number")]
    [RegularExpression(@"^01\d{8,9}$", ErrorMessage = "Contact number must start with '01' and contain 10 or 11 digits.")]
    public string Contact { get; set; }

    public string? PhotoURL { get; set; }

    public IFormFile? Photo { get; set; }


    [StringLength(150, ErrorMessage = "Content exceed number of character.")]
    public string? Address { get; set; }


    public List<EducationVM> Educations { get; set; } = new List<EducationVM>();
    public List<CareerVM> Careers { get; set; } = new List<CareerVM>();

}

public class UpdateProfileMVM
{
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string? Email { get; set; }

    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [StringLength(11, MinimumLength = 10, ErrorMessage = "Invalid Contact Number")]
    [RegularExpression(@"^01\d{8,9}$", ErrorMessage = "Contact number must start with '01' and contain 10 or 11 digits.")]
    public string Contact { get; set; }

    public string? PhotoURL { get; set; }

    public IFormFile? Photo { get; set; }


    [StringLength(150, ErrorMessage = "Content exceed number of character.")]
    public string? Address { get; set; }


}



public class ResetPasswordVM
{
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; }
}

//-----Admin---------------------//
public class RegisterAVM
{
    [StringLength(100)]
    [EmailAddress]
    [Remote("CheckEmail", "Account", ErrorMessage = "Duplicated {0}.")]
    public string Email { get; set; }

    [StringLength(100, MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [StringLength(100, MinimumLength = 5)]
    [Compare("Password")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string Confirm { get; set; }

    [StringLength(100)]
    public string Name { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateOnly DateOfBirth { get; set; }

    [StringLength(1)]
    [RegularExpression(@"^[FM]$", ErrorMessage = " Invalid {0}")]
    public string Gender { get; set; }

    [StringLength(11, MinimumLength = 10, ErrorMessage = "Invalid Contact Number")]
    [RegularExpression(@"^01\d{8,9}$", ErrorMessage = "Invalid Contact Number")]
    [Display(Name = "Contact Number")]
    public string Contact { get; set; }

    public IFormFile Photo { get; set; }

}

//------ Instructor ---------//
public class RegisterIVM
{
    [StringLength(100)]
    [EmailAddress]
    [Remote("CheckEmail", "Account", ErrorMessage = "Duplicated {0}.")]
    public string Email { get; set; }

    [StringLength(100, MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [StringLength(100, MinimumLength = 5)]
    [Compare("Password")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string Confirm { get; set; }

    [StringLength(100)]
    public string Name { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateOnly DateOfBirth { get; set; }

    [StringLength(1)]
    [RegularExpression(@"^[FM]$", ErrorMessage = " Invalid {0}")]
    public string Gender { get; set; }

    [StringLength(11, MinimumLength = 10, ErrorMessage = "Invalid Contact Number")]
    [RegularExpression(@"^01\d{8,9}$", ErrorMessage = "Invalid Contact Number")]
    [Display(Name = "Contact Number")]
    public string Contact { get; set; }



    public IFormFile Photo { get; set; }

}

//------- Employer --------//
public class RegisterEVM
{
    [StringLength(100)]
    [EmailAddress]
    [Remote("CheckEmail", "Account", ErrorMessage = "Duplicated {0}.")]
    public string Email { get; set; }

    [StringLength(100, MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [StringLength(100, MinimumLength = 5)]
    [Compare("Password")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string Confirm { get; set; }

    [StringLength(100)]
    public string Name { get; set; }



    [StringLength(1)]
    [RegularExpression(@"^[FM]$", ErrorMessage = " Invalid {0}")]
    public string Gender { get; set; }

    [StringLength(11, MinimumLength = 10, ErrorMessage = "Invalid Contact Number")]
    [RegularExpression(@"^01\d{8,9}$", ErrorMessage = "Invalid Contact Number")]
    [Display(Name = "Contact Number")]
    public string Contact { get; set; }

    [StringLength(50)]
    public string BusinessName { get; set; }


    public IFormFile Photo { get; set; }

}

public class EmailVM
{
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }

    public bool IsBodyHtml { get; set; }
}

public class StudentVM
{
    [StringLength(10)]
    [RegularExpression(@"^\d{2}[A-Z]{3}\d{5}$", ErrorMessage = "Invalid {0}.")]
    [Remote("CheckId", "Home", ErrorMessage = "Duplicated {0}.")]
    public string Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(1)]
    [RegularExpression(@"^[FM]$", ErrorMessage = "Invalid {0}.")]
    public string Gender { get; set; }


}
public class ReportVM
{
    [Required]
    public string JobID { get; set; }

    [Required]
    [EmailAddress]
    public string ReporterEmail { get; set; }

    [Required(ErrorMessage = "Please select a reason.")]
    public string Reason { get; set; }

    public string? AddReason { get; set; }
}
//------KY---------------------------//


//-------------------YT-------------------
public class ReviewVM
{
    public int Id { get; set; } // Review ID

    [Required]
    [Display(Name = "Course ID")]
    [StringLength(10)]
    public string CourseId { get; set; } // Links the review to a course (Student.Id)

    [Required]
    [EmailAddress]
    [Display(Name = "User Email")]
    public string UserId { get; set; } // User's email address

    [Required]
    [MaxLength(500)]
    [Display(Name = "Comment")]
    public string Comment { get; set; } // Review comment

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    [Display(Name = "Rating")]
    public int Rating { get; set; } // Rating between 1 and 5

    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp for review creation
}

public class TopRatedCourseVM
{
    public string CourseId { get; set; }
    public string CourseName { get; set; }
    public string Description { get; set; }
    public string PhotoURL { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}

public class ReviewSummary
{
    public int Rating { get; set; } // The star rating (1-5)
    public int Count { get; set; }  // The count of reviews for this rating
    public double Percentage { get; set; } // Percentage of total reviews
}

public class UserSummary
{
    public string UserType { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}
//---------------END OF YT----------------

public class CourseInsertVM
{
    [StringLength(4)]
    [RegularExpression(@"C\d{3}", ErrorMessage = "Invalid {0} format.")]
    [Remote("CheckId", "Course", ErrorMessage = "Duplicated {0}.")]
    public string Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(100)]
    public string Description { get; set; }
    public IFormFile Photo { get; set; }
    [StringLength(4)]
    public string ChapterId { get; set; }

    //[StringLength(100)]
    public string[] ChapterName { get; set; }
    //[StringLength(100)]
    public string[] ChapterDescription { get; set; }
    public IFormFile[] Video { get; set; }
    /// <summary>
    /// QUIZ AND QUESTIONS
    /// </summary>
    [StringLength(4)]
    public string QuizId { get; set; }
    [StringLength(100)]
    public string QuizName { get; set; }

    [StringLength(100)]
    public string QuizDescription { get; set; }
    public int QuizDuration { get; set; }
    [StringLength(4)]
    public string QuestionId { get; set; }
    public string[] QuestionName { get; set; }
    public string[] OptionA { get; set; }
    public string[] OptionB { get; set; }
    public string[] OptionC { get; set; }
    public string[] OptionD { get; set; }

}

public class CourseUpdateVM
{
    public string Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(100)]
    public string Description { get; set; }

    [Range(0.01, 9999.99)]
    [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid {0} format.")]
    public decimal Price { get; set; }

    // Other properties
    public string? PhotoURL { get; set; }
    public IFormFile? Photo { get; set; }
    public List<Chapter>? Chapters { get; set; }
    public IFormFile[] Video { get; set; }
}

public class CourseDetailVM
{
    public string Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(100)]
    public string Description { get; set; }

    [Range(0.01, 9999.99)]
    [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid {0} format.")]
    public decimal Price { get; set; }

    // Other properties
    public string? PhotoURL { get; set; }
    public IFormFile? Photo { get; set; }
    public List<Chapter>? Chapters { get; set; }
    public IFormFile[] Video { get; set; }
}

public class QuizVM
{
    public string Id { get; set; }
    public Quiz Quiz { get; set; }
    public string[] Answers { get; set; }
}
public class ReportedJobVM
{
    public string ReportId { get; set; }
    public string JobId { get; set; }
    public string JobTitle { get; set; }
    public string ReporterEmail { get; set; }
    public string Reason { get; set; }
    public string AdditionalReason { get; set; }
    public DateTime ReportDate { get; set; }
}
