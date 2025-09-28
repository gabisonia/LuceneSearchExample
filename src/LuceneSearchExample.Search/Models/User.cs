namespace LuceneSearchExample.Search.Models;

public sealed class User
{
    public User(int userId, string firstName, string lastName, int age)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
        }

        if (age <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(age), "Age must be positive.");
        }

        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    public int UserId { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public int Age { get; }
}
