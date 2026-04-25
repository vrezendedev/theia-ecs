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

    public CategoryNamedExporter(IExporter inner)
    {
        _inner = inner;
    }

    public IEnumerable<string> ExportToFiles(Summary summary, ILogger logger)
    {
        string category =
            summary.BenchmarksCases.SelectMany(c => c.Descriptor.Categories).FirstOrDefault()
            ?? summary.Title;

        string folder = summary.ResultsDirectoryPath;
        string extension = Path.GetExtension(GetDefaultFileName(summary));
        string path = Path.Combine(folder, $"{category}{extension}");

        IEnumerable<string> produced = _inner.ExportToFiles(summary, logger);

        foreach (string produced_path in produced)
        {
            string renamed = Path.Combine(
                Path.GetDirectoryName(produced_path)!,
                $"{category}{Path.GetExtension(produced_path)}"
            );

            if (File.Exists(renamed))
                File.Delete(renamed);

            File.Move(produced_path, renamed);
            yield return renamed;
        }
    }

    public void ExportToLog(Summary summary, ILogger logger) => _inner.ExportToLog(summary, logger);

    private static string GetDefaultFileName(Summary summary) => summary.Title;
}
