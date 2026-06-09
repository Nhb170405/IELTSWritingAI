using IELTSWritingAI.Data;
using IELTSWritingAI.Models;
using IELTSWritingAI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

builder.Services.Configure<GeminiOptions>(builder.Configuration.GetSection("Gemini"));
builder.Services.AddHttpClient<IGeminiService, GeminiService>();
builder.Services.AddScoped<IEssayScoringService, EssayScoringService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

await SeedDatabaseAsync(app);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Writing}/{action=Index}/{id?}");

app.Run();

static async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();

    if (await db.WritingTopics.AnyAsync())
    {
        return;
    }

    db.WritingTopics.AddRange(
        new WritingTopic { TaskType = WritingTaskType.Task2, TopicText = "Some people believe that university students should study whatever they like, while others believe they should only study subjects useful for the future. Discuss both views and give your opinion." },
        new WritingTopic { TaskType = WritingTaskType.Task2, TopicText = "In many countries, people are now living longer than ever before. What are the advantages and disadvantages of this trend?" },
        new WritingTopic { TaskType = WritingTaskType.Task2, TopicText = "Some people think that children should begin formal education at a very early age. Others believe they should start school later. Discuss both views and give your opinion." },
        new WritingTopic { TaskType = WritingTaskType.Task2, TopicText = "Many people use social media every day to communicate and get news. Do the advantages of this development outweigh the disadvantages?" },
        new WritingTopic { TaskType = WritingTaskType.Task1, TopicText = "The chart below shows the percentage of households in one country that owned selected electronic devices from 2000 to 2020. Summarise the information by selecting and reporting the main features, and make comparisons where relevant." },
        new WritingTopic { TaskType = WritingTaskType.Task1, TopicText = "The table below gives information about the average monthly spending of college students in three different countries. Summarise the information by selecting and reporting the main features, and make comparisons where relevant." }
    );

    await db.SaveChangesAsync();
}
