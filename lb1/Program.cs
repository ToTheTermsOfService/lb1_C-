using System.Reflection.Metadata.Ecma335;

enum TimeFrame { Year, TwoYears, Long }

class Paper
{
    public string Title { get; init; }
    public Person Author { get; init; }
    public DateTime PublishDate { get; init; }

    public Paper(string title, Person author, DateTime publishDate)
    {
        Title = title;
        Author = author;
        PublishDate = publishDate;
    }
    public Paper() : this("Unknown", new Person(), DateTime.Now) { }

    public override string ToString() => $"{Title} by {Author}, published on {PublishDate:yyyy-MM-dd}";
}

class ResearchTeam
{
    private string _researchTopic = default!;
    private string _organization = default!;
    private int _registrationNumber;
    private TimeFrame _duration;
    private Paper[] _publications = default!;

    public string ResearchTopic { get => _researchTopic; init => _researchTopic = value; }
    public string Organization { get => _organization; init => _organization = value; }
    public int RegistrationNumber { get => _registrationNumber; init => _registrationNumber = value; }
    public TimeFrame Duration { get => _duration; init => _duration = value; }
    public Paper[] Publications
    {
        get => _publications ?? [];
        private set => _publications = (value != null && value.Length != 0) ? value : [];
    }

    public ResearchTeam(string topic, string org, int regNum, TimeFrame timeFrame)
    {
        ResearchTopic = topic;
        Organization = org;
        RegistrationNumber = regNum;
        Duration = timeFrame;
        Publications = [];
    }
    public ResearchTeam() : this("Unknown Topic", "Unknown Organization", 0, TimeFrame.Year) { }

    public Paper? LatestPublication => Publications.Length == 0 ? null : Publications.OrderBy(p => p.PublishDate).Last();

    public bool this[TimeFrame time] => Duration == time;

    public void AddPapers(params Paper[] newPapers)
    {
        if (newPapers == null || newPapers.Length != 0)
        {
            return;
        }
        Publications = [.. Publications, .. newPapers];
    }

    public override string ToString()
    {
        var papersString = Publications.Length > 0 ? string.Join("; ", Publications.Select(p => p.ToString())) : "No publications";
        return $"Topic: {ResearchTopic}, Organization: {Organization}, Reg#: {RegistrationNumber}, Duration: {Duration}, Publications: {papersString}";
    }

    public virtual string ToShortString() => $"Topic: {ResearchTopic}, Organization: {Organization}, Reg#: {RegistrationNumber}, Duration: {Duration}";
}

class Person
{
    public string Name { get; init; }
    public string Surname { get; init; }
    public DateTime BirthDate { get; init; }

    public Person(string name, string surname, DateTime birthDate)
    {
        Name = name;
        Surname = surname;
        BirthDate = birthDate;
    }
    public Person() : this("Unknown", "Unknown", DateTime.Now) { }

    public override string ToString() => $"{Name} {Surname} ({BirthDate:yyyy-MM-dd})";

    public string ToShortString() => $"{Name} {Surname}";
}

class Program
{
    static void Main()
    {
        ResearchTeam team = new ResearchTeam("AI Research", "TechCorp", 12345, TimeFrame.TwoYears);
        Console.WriteLine(team.ToShortString());

        Console.WriteLine($"Year duration: {team[TimeFrame.Year]}");
        Console.WriteLine($"Two Years duration: {team[TimeFrame.TwoYears]}");
        Console.WriteLine($"Long duration: {team[TimeFrame.Long]}");

        team.AddPapers(new Paper("Neural Networks", new Person("Alice", "Smith", new DateTime(1985, 6, 15)), new DateTime(2023, 5, 20)));
        Console.WriteLine(team);
        Console.WriteLine("Latest publication: " + (team.LatestPublication?.ToString() ?? "No publications"));

        Console.WriteLine("Enter the number of rows and columns separated by a space, comma, or semicolon:");

        int nRows, nColumns;
        string? input;
        string[] parts;

        do
        {
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty. Try again:");
                continue;
            }

            parts = input.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out nRows) ||
                !int.TryParse(parts[1], out nColumns) ||
                nRows <= 0 ||
                nColumns <= 0)
            {
                Console.WriteLine("Invalid input. Please enter two positive integers separated by space, comma, or semicolon.");
                continue;
            }

            break;

        } while (true);

        int totalElements = nRows * nColumns;

        Paper[] oneDimArray = new Paper[totalElements];
        Paper[,] twoDimArray = new Paper[nRows, nColumns];
        Paper[][] jaggedArray = new Paper[nRows][];
        Paper[][] increasingJaggedArray = new Paper[nRows][];

        for (int i = 0; i < nRows; i++)
            jaggedArray[i] = new Paper[nColumns];

        int assignedElements = 0;
        for (int i = 0; i < nRows; i++)
        {
            int remaining = totalElements - assignedElements;
            int rowSize = Math.Min(nColumns, remaining);
            increasingJaggedArray[i] = new Paper[rowSize];
            assignedElements += rowSize;
        }
        CompareArrayPerformance(oneDimArray, twoDimArray, jaggedArray, increasingJaggedArray, nRows, nColumns);
    }

    static void CompareArrayPerformance(Paper[] oneDimArray, Paper[,] twoDimArray, Paper[][] jaggedArray, Paper[][] increasingJaggedArray, int nRows, int nColumns)
    {
        var start = DateTime.Now;
        for (int i = 0; i < oneDimArray.Length; i++)
            oneDimArray[i] = new Paper();
        Console.WriteLine("One-dimensional array time: " + (DateTime.Now - start).TotalMilliseconds);

        start = DateTime.Now;
        for (int i = 0; i < nRows; i++)
            for (int j = 0; j < nColumns; j++)
                twoDimArray[i, j] = new Paper();
        Console.WriteLine("Two-dimensional rectangular array time: " + (DateTime.Now - start).TotalMilliseconds);

        start = DateTime.Now;
        for (int i = 0; i < nRows; i++)
            for (int j = 0; j < nColumns; j++)
                jaggedArray[i][j] = new Paper();
        Console.WriteLine("Jagged array time (equal row size): " + (DateTime.Now - start).TotalMilliseconds);

        
        int assignedElements = 0;
        int size = 1;
        int row = 0;
        int totalElements = nRows * nColumns;

        start = DateTime.Now;
        while (row < nRows && assignedElements < totalElements)
        {
            int remaining = totalElements - assignedElements;
            int rowSize = Math.Min(size, remaining); 
            increasingJaggedArray[row] = new Paper[rowSize];

            assignedElements += rowSize;
            size++;
            row++;
        }
        Console.WriteLine("Jagged array time (increasing row size): " + (DateTime.Now - start).TotalMilliseconds);
    }
}
