global using JobPostingSystem.Models;
global using JobPostingSystem;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure the database
builder.Services.AddSqlServer<DB>($@"
    Data Source=(LocalDB)\MSSQLLocalDB;
    AttachDbFilename={builder.Environment.ContentRootPath}\DB.mdf;
");

// Add scoped services
builder.Services.AddScoped<Helper>();

// Add authentication and authorization
builder.Services.AddAuthentication().AddCookie();
builder.Services.AddHttpContextAccessor();

// Configure session services
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Protect against XSS
    options.Cookie.IsEssential = true; // Ensure session is always enabled
});

var app = builder.Build();

// Configure middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Enable authentication
app.UseAuthorization();
app.UseSession(); // Enable session middleware

// Configure default routing
app.MapDefaultControllerRoute();

app.Run();
