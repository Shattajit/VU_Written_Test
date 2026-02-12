using System;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Member(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public void BorrowBook(Book book)
        {
            book.Borrow();
        }

        public void ReturnBook(Book book)
        {
            book.Return();
        }

        public void ReserveBook(Book book)
        {
            book.Reserve();
        }
    }
}
