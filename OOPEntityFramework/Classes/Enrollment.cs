namespace OOPEntityFramework.Classes;

public class Enrollment
{
    public int Id { get; set; }
    public Person Student { get; set; }
    public Course Course { get; set; }
}