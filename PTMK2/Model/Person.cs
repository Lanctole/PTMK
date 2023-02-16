using System;

namespace PTMK2.Model
{
    public class Person
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set;}

        public override bool Equals(object obj)
        {
            if (obj is Person person) return FIO == person.FIO && BirthDate==person.BirthDate;
            return false;
        }
        public override int GetHashCode() => FIO.GetHashCode()+BirthDate.GetHashCode();

    }
}
