# EnumsPdfGenerator-CSharp

Generate HTML and PDF given a folder of C# enums.

## Why?

Enums can be troublesome to search for especially when they are stored as int values in the database. Having a cheatsheet to get an overall picture will help.

# Usage

> Note: you should have dotnet installed

Clone the repository, copy and paste c# enums classes and place it in the `EnumsPdfGenerator\Entities` folder.

Change directory to `EnumsPdfGenerator-CSharp\EnumsPdfGenerator` and run `dotnet run .\EnumsPdfGenerator.csproj`.

PDF will be saved to the desinated folder.


# Generated Samples

In `EnumsPdfGenerator`, there are some sample enums, inclusive of directory nesting. The generated HTML and PDF output are stored in `Sample`.

Custom Attribute `Name` is currently hardcoded for usage of `DisplayName` attribute in the enums object.


# Dependencies

```
Dotnet version: net6.0
Select.HtmlToPdf: 24.1.0
Select.HtmlToPdf.NetCore 24.1.0
```

Currently using SelectPdf's [community edition](https://selectpdf.com/community-edition/) which has a limit of 5 pages. If you have a lot of entities, the generated HTML will be more applicable.

