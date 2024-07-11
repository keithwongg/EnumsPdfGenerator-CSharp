
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
        html{
            border-style: solid;
            border-width: 4px;
            border-color: transparent;
        }
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
        var CHAINED_SCRIPT_LIST = [];
        function copySqlScriptToClipboard(id) {
            let itemId = id.split('-')[0]
            let enumsToGenerate = getMappedObjFromId(itemId)
            let textToCopy = formatTextToCopy(itemId, enumsToGenerate)
            copyTextToClipBoard(textToCopy)
            console.log(textToCopy)
            hideIdAlert(itemId, 5000)
        }

        function getMappedObjFromId(id)
        {
            console.log(id)
            let pElements = [...document.getElementById(`${id}-box`).getElementsByTagName(""p"")]
            let mappedObj = new Map()
            pElements.forEach((e) => {
                let displayName = getTextWithinBrackets(e)
                let enumNumber = getTextAfterEqualSign(e)
                mappedObj.set(displayName, enumNumber)
            })
            return mappedObj
        }

        function getTextWithinBrackets(element)
        {
            let text = element.innerText
            let captureWithinSquareBrackets = text.match(/(?<=\[)(.*?)(?=\])/)
            return (captureWithinSquareBrackets === null)
                ? text.split('=')[0].trim()
                : captureWithinSquareBrackets[1];
        }

        function getTextAfterEqualSign(element)
        {
            return element.innerText.split('=')[1].trim()
        }

        function formatTextToCopy(id, enumsToGenerate)
        {
            let placeholderName = getSqlColumnNameFromId(id)
            let text = ""case ""
            for (let [key, value] of enumsToGenerate){
                text += `when (${placeholderName} = ${value}) then '${key}'\n\t\t`
            }
            let newColName = placeholderName.split('.')[1] || getTitleDisplayName(id)
            text += `else '-' end as ${newColName}`
            return text
        }

        function getSqlColumnNameFromId(id)
        {
            return document.getElementById(`${id}-input`).value.trim() || ""sqlColumnName""
        }
        
        function copyTextToClipBoard(textToCopy)
        {
            if (isChained())
            {
                CHAINED_SCRIPT_LIST.push(textToCopy);
                navigator.clipboard.writeText(CHAINED_SCRIPT_LIST.join(`\n\t,`))
                return
            }
            navigator.clipboard.writeText(textToCopy)
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

        // toggle radio buttons
        function toggleRadio(event)
        {
            if (event.target.id === ""single-radio"")
            {
                document.getElementById(""chained-radio"").checked = """"
                CHAINED_SCRIPT_LIST = [];
                document.getElementsByTagName(""html"")[0].style.setProperty(""border-color"", ""transparent"")
                return
            }
            if (event.target.id === ""chained-radio"")
            {
                document.getElementById(""single-radio"").checked = """"
                document.getElementsByTagName(""html"")[0].style.setProperty(""border-color"", ""red"")
                return
            }
        }
        
        function isChained()
        {
            return document.getElementById(""chained-radio"").checked
        }
        
        function getTitleDisplayName(id)
        {
            return document.getElementById(`${id}-box`).getElementsByTagName(""h3"")[0].innerText
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
                <div class=""radio-container"" id=""rad-container"">
                    <p>Single = Copy 1 at a time.     Chained = Add to current copy on click. </p>
                    <label for=""single-radio"">
                        <input type=""radio"" name=""rad"" id=""single-radio"" checked=""checked"" onclick=toggleRadio(event)>
                        Single
                    </label>
                    <label for=""chained-radio"">
                        <input type=""radio"" name=""rad"" id=""chained-radio"" onclick=toggleRadio(event)>
                        Chained
                    </label><br>
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
