using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace JobPostingSystem.Models;

#nullable disable warnings

public class DB : DbContext
{
    public DB(DbContextOptions options) : base(options) { }

    // DB Sets
    public DbSet<User> Users { get; set; }
    public DbSet<TempUser> TempUsers { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<CompletedCourse> CompletedCourses { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<UserAnswer> UserAnswers { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Employer> Employers { get; set; }

    public DbSet<Education> Education { get; set; }
    public DbSet<CareerHistory> CareerHistory { get; set; }

    public DbSet<Favourite> Favourites { get; set; }
    public DbSet<Report> Reports { get; set; }

    public DbSet<Review> Reviews { get; set; }
}

// Entity Classes -------------------------------------------------------------

public class Instructor : User
{
}

public class Employer : User
{
    //[MaxLength(200)]
    //public string? BusinessName { get; set; }// only for employer
}

public class Education
{
    [Key]
    [MaxLength(4)]
    public string Id { get; set; } // Change type to int for numeric ID
    public string Course { get; set; }
    public string Institution { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string UserEmail { get; set; }
    public User Users { get; set; }
}

public class CareerHistory
{
    [Key]
    [MaxLength(4)]
    public string Id { get; set; }
    public string Career { get; set; }
    public string CompanyName { get; set; }
    public DateTime StartDatec { get; set; }
    public DateTime EndDatec { get; set; }

    [Required]
    [MaxLength(100)]
    public string UserEmail { get; set; }
    public User User { get; set; }
}

public class Report
{
    [Key]
    public string Id { get; set; }

    [Required]
    public string JobID { get; set; }

    [Required]
    [EmailAddress]
    public string ReporterEmail { get; set; }

    [Required]
    public string Reason { get; set; }

    public string? AddReason { get; set; }

    [Required]
    public DateTime ReportDate { get; set; } = DateTime.Now;

    // Navigation
    public Job Job { get; set; }
}

public class Favourite
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string JobId { get; set; }

    [Required]
    public string UserEmail { get; set; } // New column for the user's email

    // Navigation
    public Job Job { get; set; }


}

public class Review
{
    [Key]
    public int Id { get; set; } // Primary key

    public string CourseId { get; set; } // Foreign key for Course (renamed Course.Id to CourseId)
    public string UserId { get; set; } // Foreign key for User

    [MaxLength(500)]
    public string Comment { get; set; } // Review comment
    public int Rating { get; set; } // Rating (1-5)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp for review creation

    // Navigation properties
    public Course Course { get; set; } // Links Review to Course (CourseId)
    public User User { get; set; } // Links Review to User
    public bool IsDeleted { get; set; } // New property
}

public class Job
{
    [Key, MaxLength(4)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string Title { get; set; }
    public int Type { get; set; }
    [MaxLength(100)]
    public string Description { get; set; }
    [MaxLength(100)]
    public string CompanyName { get; set; }
    [MaxLength(100)]
    public string Location { get; set; }
    [MaxLength(100)]
    public string EducationLevel { get; set; }
    [MaxLength(100)]
    public string ExperienceLevel { get; set; }
    public int MinSalary { get; set; }
    public int MaxSalary { get; set; }
    public DateOnly PostDate { get; set; }
    public DateOnly Deadline { get; set; }
    public string PhotoURL { get; set; }

}

public class Application
{
    [Key, MaxLength(4)]
    public string Id { get; set; }
    public string ResumeURL { get; set; }
    public string CoverLetterURL { get; set; }
    [MaxLength(100)]
    public string UserEmail { get; set; }
    [MaxLength(4)]
    public string JobId { get; set; }
    public int Status { get; set; }
    // Navigation
    public User User { get; set; }
    public Job Job { get; set; }
}

public class Course
{
    [Key, MaxLength(4)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string Description { get; set; }
    [MaxLength(100)]
    public string PhotoURL { get; set; }
    //public int status { get; set; } //yinggai no need validation cuz not user input
    //Navigation
    public List<Chapter> Chapters { get; set; } = [];
    public Quiz Quiz { get; set; }
}

public class Chapter
{
    [Key, MaxLength(4)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string Description { get; set; }
    [MaxLength(100)]
    public string VideoURL { get; set; }
    // FK
    public string CourseId { get; set; }
    // Navigation
    public Course Course { get; set; }
}

public class Quiz
{
    [Key, MaxLength(4)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string Description { get; set; }
    public int Duration { get; set; }
    // FK
    public string CourseId { get; set; }
    // Navigation
    public Course Course { get; set; }
    public List<Question> Questions { get; set; } = [];
}

public class Question
{
    [Key, MaxLength(4)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string OptionA { get; set; }
    [MaxLength(100)]
    public string OptionB { get; set; }
    [MaxLength(100)]
    public string OptionC { get; set; }
    [MaxLength(100)]
    public string OptionD { get; set; }
    [MaxLength(1)]
    public string Answer { get; set; }
    // FK   
    public string QuizId { get; set; }
    // Navigation
    public Quiz Quiz { get; set; }
    //public UserAnswer UserAnswer { get; set; }
}


public class UserAnswer
{
    [Key, MaxLength(4)]
    public string Id { get; set; } //Y000
    [MaxLength(1)]
    public string ChosenAnswer { get; set; }
    public int Status { get; set; }
    public int Duration { get; set; }
    // FK
    public string QuestionId { get; set; }
    //public string QuizId { get; set; }
    public string CompletedCourseId { get; set; }
    public string UserEmail { get; set; }
    // Navigation
    public Question Question { get; set; }
    public User User { get; set; }
    public CompletedCourse CompletedCourse { get; set; }
}


public class User
{
    [Key, MaxLength(100)]
    public string Email { get; set; }

    [MaxLength(100)]
    public string Hash { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(100)]
    public string PhotoURL { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [MaxLength(1)]
    [RegularExpression(@"^[FM]$", ErrorMessage = "Gender must be 'F' (Female) or 'M' (Male).")]
    public string Gender { get; set; }

    [MaxLength(11)]
    [RegularExpression(@"^01\d{8,9}$", ErrorMessage = "Contact number must start with '01' and contain 10 or 11 digits.")]
    public string Contact { get; set; }

    [MaxLength(50)]
    public string? BusinessName { get; set; }

    public string Role => GetType().Name;
    public List<UserAnswer> UserAnswers { get; set; } = [];
}

public class TempUser
{
    [Key, MaxLength(100)]
    public string Email { get; set; }
    [MaxLength(100)]
    public string Hash { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }

    public string Role => GetType().Name;

    //public int numberOfFailedAttempts { get; set; }
    //public DateTime BlockedDateTime { get; set; }
}
public class Admin : User
{

}

// TODO
public class Member : User
{
    //[MaxLength(200)]
    //public string? BusinessName { get; set; }
    [MaxLength(150)]
    public string? Address { get; set; }
}

public class CompletedCourse
{
    [Key, MaxLength(4)]
    public string Id { get; set; } //W000
    [MaxLength(100)]
    public string CourseName { get; set; }
    public double Percentage { get; set; }
    public int NoOfTries { get; set; }
    // FK
    public string CourseId { get; set; }
    public string UserEmail { get; set; }
    // Navigation
    public Course Course { get; set; }
    public User User { get; set; }
    public List<UserAnswer> UserAnswers { get; set; } = [];
}