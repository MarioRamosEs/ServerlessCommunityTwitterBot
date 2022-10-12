namespace Domain.Models;

public class Sentence {
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime LastUse { get; set; }
    public DateTime Created { get; set; }
    public int TimesUsed { get; set; }
}