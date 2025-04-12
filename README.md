svg-to-png is a console app, created with .NET, for bulk converting SVG (Scalable Vector Graphics) to PNG (Portable Network Graphics).

The following options are available:
<br>--input, -i     Path to the .svg file, or a folder containing .svg files. Defaults to ./
<br>--output, -o    Folder to output the .png file(s). Defaults to ./output-png/
<br>--color, -c     Color to use for 'currentColor' in the SVG. Can be a named color like 'white' or a hex value like > '#FFFFFF'. Defaults to 'black' 
<br>--scale, -s     Scale factor for the output PNG. Use values greater than 1 for larger images, less than 1 for > smaller. Defaults to 1.0
<br>--help, -h      Displays instructions on how the app works

Usage:
1. Single file conversion:
   svg-to-png -i logo.svg -o exported -c blue -s 2.0

2. Bulk conversion (all SVGs in a folder):
   svg-to-png -i ./icons -o ./exported-icons

The app will create the output directory if it doesn't exist. When processing a single file, it must have a .svg extension. When processing a directory, all .svg files in that directory (non-recursive) will be converted to PNG format. The output files will have the same name as their input files but with .png extension.

Features:
- Supports 'currentColor' replacement in SVGs
- High-quality rendering using SkiaSharp
- Configurable output scaling
- Transparent background support
- Batch processing capability