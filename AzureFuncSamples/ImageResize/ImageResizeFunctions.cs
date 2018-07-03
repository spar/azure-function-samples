using System.IO;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;
using SixLabors.Shapes;

namespace AzureFuncSamples.ImageResize
{
    public static class ImageResizeFunctions
    {
        public static byte[] GenerateThumbnailByPercentage(Stream imageStream, int resizeByPercentage)
        {
            using (var image = Image.Load(imageStream))
            {
                image.Mutate(x =>
                    x.Resize((image.Width * resizeByPercentage) / 100, (image.Height * resizeByPercentage) / 100));
                using (var returnImageStream = new MemoryStream())
                {
                    image.Save(returnImageStream, new JpegEncoder());
                    return returnImageStream.ToArray();
                }
            }
        }

        public static byte[] GenerateThumbnailByWidthHeight(Image<Rgba32> image, int w, int h)
        {
            image.Mutate(x => x.Resize(w, h));
            using (var returnImageStream = new MemoryStream())
            {
                image.Save(returnImageStream, new JpegEncoder());
                return returnImageStream.ToArray();
            }
        }

        public static byte[] GenerateAvatar(Image<Rgba32> image, int w, int h, float radius)
        {
            var avtr = image.Clone(x => x.ConvertToAvatar(new Size(w, h), radius));
            using (var returnImageStream = new MemoryStream())
            {
                avtr.Save(returnImageStream, new PngEncoder());
                return returnImageStream.ToArray();
            }
        }

        public static IImageProcessingContext<Rgba32> ConvertToAvatar(this IImageProcessingContext<Rgba32> processingContext, Size size, float cornerRadius)
        {
            return processingContext.Resize(new ResizeOptions
            {
                Size = size,
                Mode = ResizeMode.Crop
            }).Apply(i => ApplyRoundedCorners(i, cornerRadius));
        }

        public static void ApplyRoundedCorners(Image<Rgba32> img, float cornerRadius)
        {
            IPathCollection corners = BuildCorners(img.Width, img.Height, cornerRadius);

            var graphicOptions = new GraphicsOptions(true)
            {
                BlenderMode = PixelBlenderMode.Src // enforces that any part of this shape that has color is punched out of the background
            };
            // mutating in here as we already have a cloned original
            img.Mutate(x => x.Fill(graphicOptions, Rgba32.Transparent, corners));
        }

        public static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            // first create a square
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            // then cut out of the square a circle so we are left with a corner
            IPath cornerToptLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            // corner is now a corner shape positions top left
            //lets make 3 more positioned correctly, we can do that by translating the orgional artound the center of the image
            var center = new Vector2(imageWidth / 2F, imageHeight / 2F);

            float rightPos = imageWidth - cornerToptLeft.Bounds.Width + 1;
            float bottomPos = imageHeight - cornerToptLeft.Bounds.Height + 1;

            // move it across the widthof the image - the width of the shape
            IPath cornerTopRight = cornerToptLeft.RotateDegree(90).Translate(rightPos, 0);
            IPath cornerBottomLeft = cornerToptLeft.RotateDegree(-90).Translate(0, bottomPos);
            IPath cornerBottomRight = cornerToptLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerToptLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }
    }
}