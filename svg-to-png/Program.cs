using Svg.Skia;
using SkiaSharp;
using System.CommandLine;

var inputOption = new Option<string>(
    aliases: new[] { "--input", "-i" },
    description: "Path to the .svg file, or a folder containing .svg files.",
    getDefaultValue: () => "./");

var outputOption = new Option<string>(
    aliases: new[] { "--output", "-o" },
    description: "Folder to output the .png file(s).",
    getDefaultValue: () => "./output-png/");

var colorOption = new Option<string>(
    aliases: new[] { "--color", "-c" },
    description: "Color to use for 'currentColor' in the SVG. Can be a named color like 'white' or a hex value like '#FFFFFF'.",
    getDefaultValue: () => "black");

var scaleOption = new Option<float>(
    aliases: new[] { "--scale", "-s" },
    description: "Scale factor.",
    getDefaultValue: () => 1.0f);

var rootCommand = new RootCommand("Bulk convert SVG (Scalable Vector Graphics) to PNG (Portable Network Graphics).");
rootCommand.AddOption(inputOption);
rootCommand.AddOption(outputOption);
rootCommand.AddOption(colorOption);
rootCommand.AddOption(scaleOption);

rootCommand.SetHandler(async (string input, string output, string color, float scale) =>
{
    try
    {
        // Ensure output directory exists
        Directory.CreateDirectory(output);

        if (File.Exists(input))
        {
            // Single file conversion
            if (Path.GetExtension(input).ToLower() != ".svg")
            {
                Console.WriteLine($"Error: Input file '{input}' is not an SVG file.");
                return;
            }

            await ConvertSvgToPng(input, Path.Combine(output, Path.GetFileNameWithoutExtension(input) + ".png"), color, scale);
        }
        else if (Directory.Exists(input))
        {
            // Bulk conversion
            var svgFiles = Directory.GetFiles(input, "*.svg", SearchOption.TopDirectoryOnly);
            if (svgFiles.Length == 0)
            {
                Console.WriteLine($"No SVG files found in directory '{input}'");
                return;
            }

            foreach (var svgFile in svgFiles)
            {
                var pngFileName = Path.GetFileNameWithoutExtension(svgFile) + ".png";
                var outputPath = Path.Combine(output, pngFileName);
                await ConvertSvgToPng(svgFile, outputPath, color, scale);
            }
        }
        else
        {
            Console.WriteLine($"Error: Input path '{input}' does not exist.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}, inputOption, outputOption, colorOption, scaleOption);

async Task ConvertSvgToPng(string inputPath, string outputPath, string color, float scale)
{
    try
    {
        using var svg = new SKSvg();
        // Load the SVG content first
        string svgContent = await File.ReadAllTextAsync(inputPath);
        // Replace currentColor with the specified color
        svgContent = svgContent.Replace("currentColor", color);
        
        // Load the modified SVG content
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(svgContent));
        if (await Task.Run(() => svg.Load(stream)) is { } && svg.Picture is { } picture)
        {
            using var bitmap = picture.ToBitmap(
                SKColors.Transparent,  // background color
                scale,              // scale X
                scale,              // scale Y
                SKColorType.Rgba8888,
                SKAlphaType.Premul,
                SKColorSpace.CreateSrgb());
            
            if (bitmap is { })
            {
                using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
                if (data is { })
                {
                    using var outStream = File.OpenWrite(outputPath);
                    await Task.Run(() => data.SaveTo(outStream));
                    Console.WriteLine($"Converted: {inputPath} -> {outputPath}");
                    return;
                }
            }
        }
        Console.WriteLine($"Failed to convert: {inputPath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error converting {inputPath}: {ex.Message}");
    }
}

return await rootCommand.InvokeAsync(args);
