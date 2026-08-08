using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Models;

namespace LibraryApi.Endpoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this WebApplication app)
    {
        app.MapGet("/books", async (AppDbContext db) =>
            Results.Ok(await db.Books.ToListAsync()));

        app.MapGet("/books/{id}", async (AppDbContext db, int id) =>
        {
            var book = await db.Books.FindAsync(id);
            return book is null ? Results.NotFound() : Results.Ok(book);
        });

        app.MapPost("/books", async (AppDbContext db, Book book) =>
        {
            if (string.IsNullOrWhiteSpace(book.Name))
                return Results.BadRequest("Book name is required");
            if (book.Price <= 0)
                return Results.BadRequest("Price must be greater than zero");

            db.Books.Add(book);
            await db.SaveChangesAsync();
            return Results.Created($"/books/{book.Id}", book);
        }).RequireAuthorization();

        app.MapPut("/books/{id}", async (AppDbContext db, int id, Book updatedBook) =>
        {
            var book = await db.Books.FindAsync(id);
            if (book is null) return Results.NotFound();

            book.Name = updatedBook.Name;
            book.Author = updatedBook.Author;
            book.Price = updatedBook.Price;

            await db.SaveChangesAsync();
            return Results.Ok(book);
        }).RequireAuthorization();

        app.MapDelete("/books/{id}", async (AppDbContext db, int id) =>
        {
            var book = await db.Books.FindAsync(id);
            if (book is null) return Results.NotFound();

            db.Books.Remove(book);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization();
    }
}