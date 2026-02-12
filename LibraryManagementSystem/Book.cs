using System;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    public class Book
    {
        public int Id {get; set;}
        public string Title {get; set;}
        public Category Category {get; set;}
        public List<Author> Authors {get; set;}  = new List<Author>();
        public BookState State {get; private set;} = BookState.Available;

        public Book(string title, Category category, List<Author> authors)
        {
            Title = title;
            Category = category; 
            Authors = authors;
        }

        public void Borrow()
        {
            if (State == BookState.Available)
                State = BookState.Borrowed;
            else
                Console.WriteLine($"Cannot borrow book. Current state: {State}");
        }

        public void Return()
        {
            if (State == BookState.Borrowed)
                State = BookState.Available;
            else
                Console.WriteLine($"Cannot return book. Current state: {State}");
        }

        public void Reserve()
        {
            if (State == BookState.Available)
                State = BookState.Reserved;
            else
                Console.WriteLine($"Cannot reserve book. Current state: {State}");
        }

        public void ReportLost()
        {
            State = BookState.Lost;
        }

        public void SendToMaintenance()
        {
            State = BookState.UnderMaintenance;
        }

        public void CompleteMaintenance()
        {
            if (State == BookState.UnderMaintenance)
                State = BookState.Available;
            else
                Console.WriteLine($"Cannot complete maintenance. Current state: {State}");
        }
    }
}
