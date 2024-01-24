namespace Boiler.Server;

public class PageContainer<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
}
