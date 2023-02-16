using System.Data.Entity;

namespace PTMK2.Model
{
    public class PersonContext : DbContext
    {
        public PersonContext()
            : base("DBConnection")
        {
        }

        public DbSet<Person> Persons { get; set; }
    }
}