
using EnumsPdfGenerator.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EnumsPdfGenerator;

public class Generator
{

    private string _outputPath = string.Empty;
    private string _outputDir = $"C:\\Users\\User\\Downloads";
    private string _defaultPdfFileName = "EnumsCheatsheet.pdf";
    private string _defaultHtmlFileName = "EnumsCheatsheet.html";
    public Generator(string outputPath = "")
    {
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            _outputPath = Path.Combine(_outputDir, _defaultPdfFileName);
            return;
        }
        _outputPath = outputPath;
    }

    public void GenerateFromLocalEntites()
    {
        var entities = LoadEntitiesFromAssembly();
        var contents = GenerateHtmlBody(entities);
        var finalTemplate = GetFinalizedHtml(contents);
        SaveHtml(finalTemplate);
        SavePdf(finalTemplate);
    }

    private IEnumerable<Type>? LoadEntitiesFromAssembly()
    {
        return from item in Assembly.GetExecutingAssembly().GetTypes()
               where item.IsEnum
               select item;
    }

    private string GenerateHtmlBody(IEnumerable<Type>? entities)
    {
        StringBuilder contentsBuilder = new StringBuilder();
        foreach (var e in entities)
        {
            if (e == null)
            {
                throw new Exception("Entity not found.");
            }
            contentsBuilder.OpenDivContainer(e);
            contentsBuilder.AddEachEnumValue(e);
            contentsBuilder.CloseDivContainer();
        }
        return contentsBuilder.ToString();
    }

    private string GetFinalizedHtml(string contents)
    {
        return $@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Document</title>
            </head>
            <body>
                <div style=""display: flex; flex-direction: row; flex-wrap: wrap;"">
                    {contents}
                </div>
            </body>
            </html>
        ";
    }

    private void SaveHtml(string htmlTemplate)
    {
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(_outputDir, _defaultHtmlFileName)))
        {
            outputFile.Write(htmlTemplate);
        }
        Console.WriteLine(htmlTemplate);
    }

    private void SavePdf(string htmlTemplate)
    {
        /*
         * Uses SelectPdf community edition: https://selectpdf.com/community-edition/
         * Limit: 5 pages
         */
        SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
        SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlTemplate);
        doc.Save(_outputPath);
        doc.Close();
    }
}
