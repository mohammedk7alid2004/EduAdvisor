using EduAdvisor.Domain.Entities.Base;

namespace EduAdvisor.Domain.Entities.Semesters;

public sealed class Semester : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsRegistrationOpen { get; private set; }

    public int StandardSemesterNumber { get; private set; }

    private Semester() { }

    public Semester(string name, int year, DateTime startDate, DateTime endDate, int standardSemesterNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Semester name cannot be empty");
        if (year < 2000 || year > 2100)
            throw new ArgumentException("Invalid year");
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date");
        if (standardSemesterNumber < 1 || standardSemesterNumber > 2)
            throw new ArgumentException("Standard semester number must be 1 (First Term) or 2 (Second Term).");

        Name = name.Trim();
        Year = year;
        StartDate = startDate;
        EndDate = endDate;
        StandardSemesterNumber = standardSemesterNumber;
        IsActive = true;
        IsRegistrationOpen = false;
    }

    public void OpenRegistration()
    {
        IsRegistrationOpen = true;
        UpdateTimestamp();
    }

    public void CloseRegistration()
    {
        IsRegistrationOpen = false;
        UpdateTimestamp();
    }

    public void Activate()
    {
        IsActive = true;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        IsRegistrationOpen = false;
        UpdateTimestamp();
    }

    public bool IsCurrentDateInSemester()
    {
        var now = DateTime.UtcNow;
        return now >= StartDate && now <= EndDate;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Semester name cannot be empty.");
        Name = name.Trim();
        UpdateTimestamp();
    }

    public void UpdateYear(int year)
    {
        if (year < 2000 || year > 2100)
            throw new ArgumentException("Invalid year.");
        Year = year;
        UpdateTimestamp();
    }

    public void UpdateStandardSemesterNumber(int number)
    {
        if (number < 1 || number > 2)
            throw new ArgumentException("Standard semester number must be 1 or 2.");
        StandardSemesterNumber = number;
        UpdateTimestamp();
    }

    public void UpdateStartDate(DateTime startDate)
    {
        if (startDate >= EndDate)
            throw new ArgumentException("Start date must be before end date.");
        StartDate = startDate;
        UpdateTimestamp();
    }

    public void UpdateEndDate(DateTime endDate)
    {
        if (endDate <= StartDate)
            throw new ArgumentException("End date must be after start date.");
        EndDate = endDate;
        UpdateTimestamp();
    }

    public string GetDisplayName() => $"{Name} {Year}";
}