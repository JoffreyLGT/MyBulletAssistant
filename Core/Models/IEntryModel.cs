namespace Core.Models
{
    public interface IEntryModel
    {
        int Id { get; set; }
        string Journal { get; set; }
        string Pages { get; set; }
        string Title { get; set; }
    }
}