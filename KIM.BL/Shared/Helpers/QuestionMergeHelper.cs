namespace KIM.BL.Shared.Helpers;

public static class QuestionMergeHelper
{
    public static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    public static string? MergeAuthors(string? existing, string? incoming)
    {
        var items = new List<string>();

        Add(items, existing);
        Add(items, incoming);

        return items.Count == 0 ? null : string.Join("; ", items);
    }

    public static string? MergeComment(string? existing, string? incoming)
    {
        return string.IsNullOrWhiteSpace(incoming) ? existing : incoming.Trim();
    }

    private static void Add(List<string> items, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        var segments = value
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => !string.IsNullOrWhiteSpace(s));

        foreach (var segment in segments)
        {
            if (items.All(x => !x.Equals(segment, StringComparison.OrdinalIgnoreCase)))
            {
                items.Add(segment);
            }
        }
    }
}