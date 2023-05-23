namespace OOPEntityFramework.Classes;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Person Teacher { get; set; }
}