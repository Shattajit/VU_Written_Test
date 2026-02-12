using System;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            // Authors
            var author1 = new Author { Id = 1, Name = "J.K. Rowling" };
            var author2 = new Author { Id = 2, Name = "George Orwell" };

            // Categories
            var category1 = new Category { Id = 1, Name = "Fantasy" };
            var category2 = new Category { Id = 2, Name = "Dystopian" };

            // Books
            var book1 = new Book("Harry Potter", category1, new List<Author> { author1 });
            var book2 = new Book("1984", category2, new List<Author> { author2 });

            // Member
            var member = new Member(1, "Shattajit");

            // Borrow a book
            member.BorrowBook(book1);
            Console.WriteLine($"{book1.Title} is now {book1.State}");

            // Return a book
            member.ReturnBook(book1);
            Console.WriteLine($"{book1.Title} is now {book1.State}");

            // Reserve a book
            member.ReserveBook(book2);
            Console.WriteLine($"{book2.Title} is now {book2.State}");

            // Send book to maintenance
            book2.SendToMaintenance();
            Console.WriteLine($"{book2.Title} is now {book2.State}");
        }
    }
}
