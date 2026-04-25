using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

public sealed class CategoryNamedExporter : IExporter
{
    private readonly IExporter _inner;

    public string Name => _inner.Name;

    public CategoryNamedExporter(IExporter inner) => _inner = inner;

    public IEnumerable<string> ExportToFiles(Summary summary, ILogger logger)
    {
        List<string> allCategories = summary
            .BenchmarksCases.Select(c => c.Descriptor.Categories.First())
            .Distinct()
            .ToList();

        string category = allCategories.Count > 1 ? summary.Title : allCategories[0];

        foreach (string producedPath in _inner.ExportToFiles(summary, logger))
        {
            string renamed = Path.Combine(
                Path.GetDirectoryName(producedPath)!,
                $"{category}{Path.GetExtension(producedPath)}"
            );

            if (File.Exists(renamed))
                File.Delete(renamed);

            File.Move(producedPath, renamed);
            yield return renamed;
        }
    }

    public void ExportToLog(Summary summary, ILogger logger) => _inner.ExportToLog(summary, logger);

    private static string GetDefaultFileName(Summary summary) => summary.Title;
}
