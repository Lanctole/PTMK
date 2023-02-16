using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PTMK2.Model;

namespace PTMK2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            using (PersonContext db = new PersonContext())
            {
                db.Database.CreateIfNotExists();
                Console.WriteLine(db.Database.Exists() ? "Database Exists" : "Error");
                switch (args[0])
                {
                    case "1":
                        CreateTable(db);
                        Console.WriteLine("Table created");
                        break;

                    case "2":
                        try
                        {
                            AddPerson(db,args);
                            Console.WriteLine("Done");
                        }
                        catch(Exception ex) { Console.WriteLine(ex.ToString()); }
                        break;

                    case "3":
                        ShowUniquePeoples(db);
                        Console.WriteLine("Done");
                        break;

                    case "4":
                        TableFilling(db, random);
                       Console.WriteLine("Done");
                        break;

                    case "5":
                        Stopwatch timer = new Stopwatch();
                        timer.Start();
                        var selectedPeoples = CreateSelection(db);
                        timer.Stop();
                        Console.WriteLine(timer.Elapsed);
                        break;
                    default: 
                        break;
                }
            }
        }

        static void CreateTable(PersonContext db)
        {
            var initTablePerson = new Person
                {
                    FIO = "A",
                    BirthDate = DateTime.ParseExact("01.01.2000", "dd.MM.yyyy",
                        System.Globalization.CultureInfo.InvariantCulture),
                    Gender = "Male"
                };
                db.Persons.Add(initTablePerson);
                db.SaveChanges();
                db.Persons.Remove(initTablePerson);
                db.SaveChanges();
        }

        static void AddPerson(PersonContext db, string[] args)
        {
            if (args.Length != 4)
                throw new Exception("Incorrect input");

            if (args[3].ToUpper() != "MALE" && args[3].ToUpper() != "FEMALE")
                throw new Exception("There were a mistake in gender");

            Regex regex = new Regex(@"\d{2}\u002E\d{2}\u002E\d{4}");
            if(!regex.IsMatch(args[2]))
                throw new Exception("Mistake in input. Correct date format is dd.mm.yyyy");

            db.Persons.Add(new Person
                {
                    FIO = args[1],
                    BirthDate = DateTime.ParseExact(args[2], "dd.MM.yyyy",
                        System.Globalization.CultureInfo.InvariantCulture),
                    Gender = args[3]
                });
            db.SaveChanges();
            
        }

        static void ShowUniquePeoples(PersonContext db)
        {
            var today = DateTime.Today;
            var peoplesFromDB = (from p in db.Persons select p).ToList();
            var unique = peoplesFromDB
                .OrderBy(m=>m.FIO)
                .Select(i => new Person {FIO = i.FIO, BirthDate = i.BirthDate})
                .Distinct()
                .ToList();
            
            foreach (var person in unique)
            {
                var age = today.Year - person.BirthDate.Year;
                if (person.BirthDate.Date > today.AddYears(-age)) age--;
                Console.WriteLine("FIO = " + person.FIO + ", BirthDate = " + person.BirthDate + ", Age = " + age);
            }
        }

        static void TableFilling(PersonContext db, Random random)
        {
            int i = 0;
            for (i = 0; i < 100; i++)
            {
                db.Persons.Add(new Person { FIO = GenerateRandomName(random,true), BirthDate = Convert.ToDateTime(GetRandomDateTime(random)), Gender = "Male" });
            }
            db.SaveChanges();
            for (; i < 1000000; i++)
            {
                db.Persons.Add(new Person { FIO = GenerateRandomName(random,false), BirthDate = Convert.ToDateTime(GetRandomDateTime(random)), Gender = i % 2==1? "Male":"Female" });
                
                if (i % 10000 == 0)
                {
                    db.SaveChanges();
                }
            }
            db.SaveChanges();
        }

        static string GenerateRandomName(Random r, bool NeedF)
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            StringBuilder name = new StringBuilder();
            
            for (int i = 0; i < 3; i++)
            {
                if(name.Length==0 && NeedF)
                     name.Append("F");
                else
                    name.Append(alphabet.ToUpper()[r.Next(alphabet.Length)]);
                
                for (int j = 0; j < r.Next(5,10); j++)
                    name.Append(alphabet[r.Next(alphabet.Length)]);
            }
            return name.ToString();
        }

        static DateTime GetRandomDateTime(Random r)
        {
            DateTime d = DateTime.ParseExact("01.01.1980", "dd.MM.yyyy",
                System.Globalization.CultureInfo.InvariantCulture);
            return d.AddDays(r.Next(10000));
        }

        static List<Person> CreateSelection(PersonContext db)
        {
            return (from p in db.Persons where p.Gender.ToUpper()=="MALE" && p.FIO.StartsWith("F") select p).ToList();
        }
    }
}
