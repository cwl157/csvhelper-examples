using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvHelperExamples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<Book> books = new List<Book>();
            string choice = "";
            while (choice != "8")
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("1. Add Book");
                Console.WriteLine("2. Display Books");
                Console.WriteLine("3. Import Books");
                Console.WriteLine("4. Import Some Books");
                Console.WriteLine("5. Import Book Titles Only");
                Console.WriteLine("6. Export Books");
                Console.WriteLine("7. Export Some books");
                Console.WriteLine("8. Exit");
                choice = Console.ReadLine();

                if (choice == "1")
                {
                    Book b = new Book();
                    Console.WriteLine("Title: ");
                    b.Title = Console.ReadLine();
                    Console.WriteLine("Author: ");
                    b.Author = Console.ReadLine();
                    Console.WriteLine("Length: ");
                    string ls = Console.ReadLine();
                    int ln;
                    if (int.TryParse(ls, out ln))
                    {
                        b.Length = ln;
                        books.Add(b);
                    }
                    else
                    {
                        Console.WriteLine("Invalid number");
                    }
                }
                else if (choice == "2")
                {
                    int totalPages = 0;
                    foreach (Book b in books)
                    {
                        totalPages += b.Length;
                        Console.WriteLine($"Title: {b.Title} By: {b.Author} Length: {b.Length}");
                    }
                    if (books.Count > 0)
                    {
                        Console.WriteLine($"Total Pages: {totalPages}");
                    }
                }
                else if (choice == "3")
                {
                    Console.WriteLine("Full file path of source file");
                    string filePath = Console.ReadLine();
                    Console.WriteLine("Enter delimiter");
                    string delimiter = Console.ReadLine();
                    books = Read(filePath, delimiter);
                }
                else if (choice == "4")
                {
                    Console.WriteLine("Full file path of source file");
                    string filePath = Console.ReadLine();
                    Console.WriteLine("Enter delimiter");
                    string delimiter = Console.ReadLine();
                    Console.WriteLine("Number of records to import");
                    string numberToImport = Console.ReadLine();
                    books = Read(filePath, delimiter, numberToImport);
                }
                else if (choice == "5")
                {
                    Console.WriteLine("Full file path of source file");
                    string filePath = Console.ReadLine();
                    Console.WriteLine("Enter delimiter");
                    string delimiter = Console.ReadLine();
                    books = ReadTitles(filePath, delimiter);
                    
                }
                else if (choice == "6")
                {
                    Console.WriteLine("Full file path of destination file");
                    string filePath = Console.ReadLine();
                    Console.WriteLine("Enter delimiter");
                    string delimiter = Console.ReadLine();
                    Write(books, filePath, delimiter);
                }
                else if (choice == "7")
                {
                    Console.WriteLine("Full file path of destination file");
                    string filePath = Console.ReadLine();
                    Console.WriteLine("Enter delimiter");
                    string delimiter = Console.ReadLine();
                    Console.WriteLine("Number of recrods to export");
                    string numberToExport = Console.ReadLine();
                    Write(books, filePath, delimiter, numberToExport);
                }
                else if (choice == "8")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid option");
                }
            }
        }
    
        private static List<Book> Read(string filePath, string delimiter, string numberOfRecords)
        {
            int count = 0;
            List<Book> results = new List<Book>();
            if (!int.TryParse(numberOfRecords, out count))
            {
                Console.WriteLine("Not a valid number");
                return results;
            }
            // Read from csv record by record up to count
            try
            {
                using (StreamReader reader = File.OpenText(filePath))
                {
                    CsvReader csv = new CsvReader(reader);
                    csv.Configuration.Delimiter = delimiter;
                    csv.Configuration.RegisterClassMap<BookMap>();
                    // From the CSV Helper docs:
                    // The header in a CSV file is just another record, but it has special meaning.
                    // If your file has a header record, you'll need to read the header after the first read.
                    // After that you can loop the records and read them. This will allow you to be able to read
                    // headers on different rows, or even multiple headers.
                    csv.Read();
                    csv.ReadHeader();
                    for (int i = 0; i < count; i++)
                    {
                        if (csv.Read())
                        {
                            Book record = csv.GetRecord<Book>();
                            results.Add(record);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return results;
        }

        private static List<Book> Read(string filePath, string delimiter)
        {
            List<Book> results = new List<Book>();
            try
            {
                using (StreamReader reader = File.OpenText(filePath))
                {
                    CsvReader csv = new CsvReader(reader);
                    csv.Configuration.Delimiter = delimiter;
                    csv.Configuration.RegisterClassMap<BookMap>();
                    IEnumerable<Book> records = csv.GetRecords<Book>();
                    results = records.ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return results;
        }

        private static List<Book> ReadTitles(string filePath, string delimiter)
        {
            List<Book> results = new List<Book>();
            try
            {
                List<string> titles = new List<string>();
                using (StreamReader reader = File.OpenText(filePath))
                {                    
                    CsvReader csv = new CsvReader(reader);
                    csv.Configuration.Delimiter = delimiter;
                    csv.Read();
                    // ReadHeader allows us access to each individual column from the file, by referencing the header
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        string field = csv["Title"];
                        titles.Add(field);
                    }
                }

                foreach (string t in titles)
                {
                    results.Add(new Book { Title = t, Author = "" });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return results;
        }

        private static void Write(List<Book> books, string filePath, string delimiter)
        {
            try
            {
                using (StreamWriter writer = File.CreateText(filePath))
                {
                    var csv = new CsvWriter(writer);
                    csv.Configuration.Delimiter = delimiter;
                    csv.WriteRecords(books);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        private static void Write(List<Book> books, string filePath, string delimiter, string numberOfRecords)
        {
            int count = 0;
            if (!int.TryParse(numberOfRecords, out count))
            {
                Console.WriteLine("Not a valid number");
                return;
            }

            using (StreamWriter writer = File.CreateText(filePath))
            {
                var csv = new CsvWriter(writer);
                csv.Configuration.Delimiter = delimiter;
                csv.WriteHeader<Book>();
                csv.NextRecord();

                for (int i = 0; i < count; i++)
                {
                    csv.WriteRecord(books[i]);
                    csv.NextRecord();
                    csv.Flush();
                }
            }
        }
    }
}
