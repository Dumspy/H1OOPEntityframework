using Microsoft.EntityFrameworkCore;
using OOPEntityFramework.Classes;

void DrawMenu(string[] items, int currentItem)
{
    var output = new System.Text.StringBuilder();
    for (int i = 0; i < items.Length; i++)
    {
        output.Append(i == currentItem ? "[" : "");
        output.Append(items[i]);
        output.Append(i == currentItem ? "]" : " ");
        if (i < items.Length - 1)
        {
            output.Append(" ");
        }
    }
    Console.SetCursorPosition((Console.WindowWidth - output.Length) / 2, Console.CursorTop);
    Console.WriteLine(output.ToString());
}

int MenuHandler(string[] items)
{
    bool canExit = false;
    int currentItem = 0;
    while (!canExit)
    {
        DrawMenu(items, currentItem);
        switch (Console.ReadKey().Key)
        {
            case ConsoleKey.LeftArrow:
                currentItem = currentItem == 0 ? items.Length-1 : currentItem - 1;
                break;
            case ConsoleKey.RightArrow:
                currentItem = currentItem == items.Length-1 ? 0 : currentItem + 1;
                break;
            case ConsoleKey.Enter:
                canExit = true;
                break;
        }
        Console.Clear();
    }

    return currentItem;
}

void SearchLogic()
{
    string[] searchTerms = Enum.GetNames(typeof(SearchTerms)).Skip(1).ToArray();
    SearchTerms selectedItem = (SearchTerms)Enum.Parse(typeof(SearchTerms), searchTerms[MenuHandler(searchTerms)]);;

    Console.Clear();
    Console.Write("Søg efter: ");
    string searchTerm;
    try
    {
        searchTerm = Console.ReadLine()?.ToLower()!;
    }
    catch(Exception e)
    {
        searchTerm = string.Empty;
    }
    
    
    Console.Clear();
    Console.WriteLine($"Søger efter {searchTerm} i {selectedItem}");
    using var db = new DBContext();

    switch (selectedItem)
    {   
        case SearchTerms.Elev:
            var student = db.People.FirstOrDefault(p => p.Type == PersonType.Student && p.firstName.ToLower().Contains(searchTerm) || p.lastName.ToLower().Contains(searchTerm)) ?? null;    
            
            if (student == null)
            {
                Console.WriteLine("Ingen elever fundet");
                break;
            }

            var enrollments = db.Enrollments
                .Include(e => e.Course)
                .ThenInclude(c => c.Teacher)
                .Where(e => e.Student.Id == student.Id)
                .ToList();

            Console.WriteLine($"Elev: {student}");
            Console.WriteLine($"Tilmeldt {enrollments.Count} fag");
            
            foreach (var enrollment in enrollments)
            {
                Console.WriteLine(enrollment.Course);
                Console.WriteLine($"    Lære: {enrollment.Course.Teacher}");
            }
            
            break;
        case SearchTerms.Lære:
            var teacher = db.People.FirstOrDefault(p => p.Type == PersonType.Teacher && p.firstName.ToLower().Contains(searchTerm) || p.lastName.ToLower().Contains(searchTerm)) ?? null;    
            
            if (teacher == null)
            {
                Console.WriteLine("Ingen lære fundet");
                break;
            }
            Console.Clear();
            Console.WriteLine($"{teacher}");
            Console.WriteLine("Fag : Antal Elever");
            var courses = db.Enrollments.Where(e=> e.Course.Teacher == teacher).Include(e=> e.Student).GroupBy(e => e.Course);
            foreach (var item in courses)
            {
                Console.WriteLine($"{item.Key.Name} : {item.Count()}");
                foreach (var enrollment in item)
                {
                    Console.WriteLine($"    {enrollment.Student}");
                }
            }
            
            break;
        case SearchTerms.Fag:
            var course = db.Courses.Include(c => c.Teacher).First(c => c.Name.ToLower().Contains(searchTerm));
            var courseEnrollments = db.Enrollments.Include(e=> e.Student).Where(e => e.Course.Id == course.Id);

            Console.WriteLine($"{course} - {course.Teacher} - {courseEnrollments.Count()}");
            foreach (var item in courseEnrollments)
            {
                Console.WriteLine($"    {item.Student}");
            }
            break;
    }

    Console.ReadKey();
}

void CUDLogic()
{
    const string title = "Hvad ønsker du at oprette";
    switch (MenuHandler(new[] { "Create", "Update", "Delete" }))
    {
        case 0:
            //switch (MenuHandler(new []{"Fag","Person","Tilmelding"})) // TODO
            Console.Clear();
            Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, Console.CursorTop);
            Console.WriteLine(title);
            switch (MenuHandler(new[] { "Fag" }))
            {

                case 0:
                {
                    using var db = new DBContext();
                    string courseName;
                    Console.WriteLine("Indtast navn på fag: ");
                    try
                    {
                        courseName = Console.ReadLine()?.ToLower()!;
                    }
                    catch
                    {
                        courseName = string.Empty;
                    }

                    string teacherName;
                    Console.WriteLine("Indtast navn på læren: ");
                    try
                    {
                        teacherName = Console.ReadLine()?.ToLower()!;
                    }
                    catch
                    {
                        teacherName = string.Empty;
                    }

                    var teacher = db.People.FirstOrDefault(p =>
                        (p.Type == PersonType.Teacher && p.firstName.ToLower().Contains(teacherName) ||
                         p.lastName.ToLower().Contains(teacherName))) ?? null;
                    if (teacher == null)
                    {
                        Console.WriteLine($"Fandt ingen lære ved navn: {teacherName}");
                        Console.ReadKey();
                        break;
                    }

                    db.Courses.Add(new Course() { Name = courseName, Teacher = teacher });
                    db.SaveChanges();
                    Console.WriteLine(
                        $"Nyt fag opret med navn: {courseName}, lære: {teacher}");
                    Console.ReadKey();

                    break;
                }
                case 1:
                    break;
                case 2:
                    break;
            }
            break;
        case 1:
            break;
        case 2:
            Console.Clear();
            Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, Console.CursorTop);
            Console.WriteLine(title);
            //switch (MenuHandler(new []{"Fag","Person","Tilmelding"})) // TODO
            switch (MenuHandler(new []{"Fag"}))
            {
                case 0:
                {
                    Console.Write("Indtast navn på fag: ");
                    string searchTerm;
                    try
                    {
                        searchTerm = Console.ReadLine()?.ToLower()!;
                    }
                    catch (Exception e)
                    {
                        searchTerm = string.Empty;
                    }
                    using var db = new DBContext();
                    var course = db.Courses.First(c => c.Name.ToLower().Contains(searchTerm));
                    Console.WriteLine($"Fandt fag ved navn: {course}");
                    Console.WriteLine("Er du sikker på du vil slette");
                    switch (MenuHandler(new []{"Ja", "Nej"}))
                    {
                        case 0:
                            db.Remove(course);
                            db.SaveChanges();
                            Console.WriteLine($"{course} er nu sletted");
                            Console.ReadKey();
                            break;
                        case 1:
                            break;
                    }
                    break;
                }
                case 1:
                    break;
                case 2:
                    break;
            }
            break;
    }
}

while (true)
{
    int selectedItem = MenuHandler(new[] { "Søg", "CUD" });

    switch (selectedItem)
    {
        case 0:
            SearchLogic();
            break;
        case 1:
            CUDLogic();
            break;
    }
    Console.Clear();
}

enum SearchTerms
{
    None,
    Lære,
    Elev,
    Fag
}