using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options=>
options.UseSqlite("Data Source=LibraryApi.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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