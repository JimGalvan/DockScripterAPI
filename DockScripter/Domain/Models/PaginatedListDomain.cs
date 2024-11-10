namespace DockScripter.Domain.Models;

public class PaginatedListDomain<T> where T : class
{
    public int TotalCount { get; set; }
    public IEnumerable<T> Items { get; set; } = default!;
}