
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
        //SavePdf(finalTemplate);
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
        var style = @"
        .overall-container{
            display: flex;
            flex-direction: row;
            flex-wrap: wrap;
        }
        .enum-container{
            border-style: solid;
            padding: 8px;
            margin: 4px;
        }
        .copy-alert {
            display: none;
            color: green
        }
        .radio-container {
            display: flex;
            flex-direction: row;
            align-items: center;
        }
        ";
        var script = @"
        function copySqlScriptToClipboard(id) {
            let itemId = id.split('-')[0]
            let enumsToGenerate = getMappedObjFromId(itemId)
            let textToCopy = formatTextToCopy(itemId, enumsToGenerate)
            navigator.clipboard.writeText(textToCopy)
            console.log(textToCopy)
            hideIdAlert(itemId, 5000)
        }

        function getMappedObjFromId(id)
        {
            console.log(id)
            let pElements = [...document.getElementById(`${id}-box`).getElementsByTagName(""p"")]
            let mappedObj = new Map()
            pElements.forEach((e) => {
                let displayName = getTextWithinRoundedBrackets(e)
                let enumNumber = getTextAfterEqualSign(e)
                if (hasValue(displayName))
                {
                    mappedObj.set(displayName, enumNumber)
                }
            })
            return mappedObj
        }

        function getTextWithinRoundedBrackets(element)
        {
            return element.innerText.match(/\(([^)]+)\)/)[1]
        }

        function getTextAfterEqualSign(element)
        {
            return element.innerText.split('=')[1].trim()
        }

        function hasValue(displayName)
        {
            return displayName.trim() !== '-'
        }

        function formatTextToCopy(id, enumsToGenerate)
        {
            let placeholderName = getSqlColumnNameFromId(id)
            let text = ""case ""
            for (let [key, value] of enumsToGenerate){
                text += `when (${placeholderName} = ${value}) then '${key}' \n \t`
            }
            let finalText = text.slice(0, -2) // don't want the last line to be indented
            finalText += `else 'not a value' end as ${placeholderName.split('.')[1]}`
            return finalText
        }

        function getSqlColumnNameFromId(id)
        {
            return document.getElementById(`${id}-input`).value.trim() || ""sqlColumnName""
        }

        function hideIdAlert(id, duration)
        {
            document.getElementById(`${id}-alert`).style.display = 'block';
            setTimeout(function(){
                document.getElementById(`${id}-alert`).style.display = 'none';
            }, duration)
        }
        function copySqlScriptToClipboardOnEnter(event)
        {
            if (event.key == ""Enter"")
            {
                copySqlScriptToClipboard(event.target.id)
            }
        }
        ";
        return $@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Document</title>
                <style>{style}</style>
                <script>{script}</script>
            </head>
            <body>
                <p>If you want to copy enums out as SQL Script, insert the col name and click on the 'get' button. E.g aa.Name</p>
                <div class=""radio-container"">
                    <p>Single = Copy 1 at a time.     Chained = Add to current copy on click. </p>
                    <input type=""radio"" id=""single-radio"">
                    <label for=""single-radio"">Single</label><br>
                    <input type=""radio"" id=""chained-radio"">
                    <label for=""chained-radio"">Chained</label><br>
                </div>
                <div class=""overall-container"">
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
