using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options=>
options.UseSqlite("Data Source=LibraryApi.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtKey = "this-is-a-very-secret-key-change-it-later-12345";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = 
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/login", (LoginRequest request) =>
{
    if (request.Username != "admin" || request.Password != "1234")
    {
        return Results.Unauthorized();
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, request.Username),
        new Claim(ClaimTypes.Role, "Admin")
    };

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: credentials
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new { token = tokenString });
});


app.MapGet("/books" , async (AppDbContext db) =>
{
    return Results.Ok(await db.Books.ToListAsync());
});

app.MapGet("/books/{id}", async (AppDbContext db , int id) =>
{
    var book = await db.Books.FindAsync(id);
    return book is null ? Results.NotFound("book not found!"):Results.Ok(book);
});

app.MapPost("/books" , async (AppDbContext db , Book book) =>
{
    if (string.IsNullOrWhiteSpace(book.Name))
    {
        return Results.BadRequest("Book name is required");
    }
    if (book.Price<= 0)
    {
        return Results.BadRequest("price must be greater than zero");
    }
    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/books/{book.Id}" , book);
});

app.MapPut ("/books/{id}" , async (AppDbContext db , int id , Book updatedbook) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();
    
    book.Name = updatedbook.Name;
    book.Author = updatedbook.Author;
    book.Price = updatedbook.Price;

    await db.SaveChangesAsync();
    return Results.Ok(book);
});

app.MapDelete("/books/{id}", async (AppDbContext db, int id) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    db.Books.Remove(book);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();