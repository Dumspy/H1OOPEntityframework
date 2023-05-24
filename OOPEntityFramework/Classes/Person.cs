namespace OOPEntityFramework.Classes;

public enum PersonType
{
    Student,
    Teacher
}

public class Person
{
    public int Id { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public PersonType Type { get; set; }

    public override string ToString()
    {
        return $"{firstName} {lastName}";
    }
}