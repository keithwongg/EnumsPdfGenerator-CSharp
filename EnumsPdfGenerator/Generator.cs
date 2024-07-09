
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
    public Generator()
    {
        
    }

    public async Task GenerateFromLocalEntites()
    {
        var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                where t.IsEnum
                select t;
        StringBuilder contentsBuilder = new StringBuilder();
        foreach (var t in q)
        {
            contentsBuilder.Append($"<div id=\"{t.Name.Trim().ToLower()}\">");
            Console.WriteLine($"Namespace: {t.Namespace}");
            Console.WriteLine($"Obj: {t}");
            contentsBuilder.Append($"<h1>{t.Namespace}</h1>");
            contentsBuilder.Append($"<h2>{t.Name}</h2>");

            var testEnum = Activator.CreateInstance(t);
            Type enumType = testEnum.GetType();
            var test = enumType.GetEnumValues();
            foreach (var v in test)
            {
                Console.WriteLine($"value: {v}");
                contentsBuilder.Append($"<p>{t}</p>");
            }
            contentsBuilder.Append("</div>");

        }
        var finalTemplate = GenerateTemplate( contentsBuilder.ToString() );
        //string directory = "C:\\Downloads";
        //using (StreamWriter outputFile = new StreamWriter(Path.Combine(directory, "test.html")))
        //{
        //    outputFile.Write(finalTemplate);
        //}
        Console.WriteLine(finalTemplate);
        
        
        
        // take the string html and generate pdf
        //Create a pdf document.
   
        SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
        SelectPdf.PdfDocument doc = converter.ConvertHtmlString(finalTemplate);
        doc.Save("C:\\Users\\User\\Downloads\\test.pdf");
        doc.Close();

    }



    private string GenerateTemplate(string contents)
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
                    {contents}
            </body>
            </html>
        ";
    }
}
