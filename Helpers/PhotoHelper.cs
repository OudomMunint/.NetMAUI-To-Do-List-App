using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Models;
using ToDoListApp.Data;

namespace ToDoListApp.Helpers
{
    public class PhotoResult
    {
        public byte[] ImageData { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
    }

    public static class PhotoHelper
    {
        public static async Task<PhotoResult> TakePhotoAsync(bool shouldProcessAttachment)
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        // save to local storage
                        string localFilePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, photo.FileName);

                        using (Stream sourceStream = await photo.OpenReadAsync())
                        {
                            using (FileStream localFileStream = File.OpenWrite(localFilePath))
                            {
                                await sourceStream.CopyToAsync(localFileStream);
                            }
                        }

                        // subsample attached
                        byte[] originalBytes = File.ReadAllBytes(localFilePath);
                        byte[] processedBytes;

                        if (shouldProcessAttachment)
                        {
                            processedBytes = SubSampleImageToByteArray(originalBytes, 700);
                        }
                        else
                        {
                            processedBytes = originalBytes;
                        }

                        return new PhotoResult
                        {
                            ImageData = processedBytes,
                            Success = true
                        };
                    }
                }
                
                return new PhotoResult
                {
                    Success = false,
                    ErrorMessage = "No photo captured"
                };
            }
            catch (Exception exception)
            {
                return new PhotoResult
                {
                    Success = false,
                    ErrorMessage = exception.Message
                };
            }
        }

        public static async Task<PhotoResult> UploadPhotoAsync(bool shouldProcessAttachment)
        {
            try
            {
                FileResult uploadedFile = await MediaPicker.Default.PickPhotoAsync();

                if (uploadedFile != null)
                {
                    // save to local storage
                    string localFilePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, uploadedFile.FileName);

                    using (Stream sourceStream = await uploadedFile.OpenReadAsync())
                    {
                        using (FileStream localFileStream = File.OpenWrite(localFilePath))
                        {
                            await sourceStream.CopyToAsync(localFileStream);
                        }
                    }

                    // subsample attached
                    byte[] originalBytes = File.ReadAllBytes(localFilePath);
                    byte[] processedBytes;

                    if (shouldProcessAttachment)
                    {
                        processedBytes = SubSampleImageToByteArray(originalBytes, 700);
                    }
                    else
                    {
                        processedBytes = originalBytes;
                    }

                    return new PhotoResult
                    {
                        ImageData = processedBytes,
                        Success = true
                    };
                }

                return new PhotoResult
                {
                    Success = false,
                    ErrorMessage = "No photo selected"
                };
            }
            catch (Exception exception)
            {
                return new PhotoResult
                {
                    Success = false,
                    ErrorMessage = exception.Message
                };
            }
        }

        public static byte[] SubSampleImageToByteArray(byte[] originalBytes, int maxWidth)
        {
            using var inputStream = new MemoryStream(originalBytes);
            using var codec = SKCodec.Create(inputStream);
            if (codec == null)
                return originalBytes;

            // Get the EXIF orientation
            var orientation = codec.EncodedOrigin;

            // Decode to bitmap
            using var original = SKBitmap.Decode(codec);
            if (original == null)
                return originalBytes;

            // OG dimensions
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            // Maintaine aspect ratio
            float ratio = (float)maxWidth / originalWidth;
            int newWidth = maxWidth;
            int newHeight = (int)(originalHeight * ratio);

            var samplingOptions = GetSamplingOptions(SamplingQuality.High);
            using var resized = original.Resize(new SKImageInfo(newWidth, newHeight), samplingOptions);
            if (resized == null)
                return originalBytes;

            SKBitmap rotated;

            // Handle rotation
            switch (orientation)
            {
                case SKEncodedOrigin.RightTop: // 90°
                    rotated = new SKBitmap(resized.Height, resized.Width);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.Translate(rotated.Width, 0);
                        canvas.RotateDegrees(90);
                        canvas.DrawBitmap(resized, 0, 0);
                    }
                    break;

                case SKEncodedOrigin.BottomRight: // 180°
                    rotated = new SKBitmap(resized.Width, resized.Height);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.Translate(rotated.Width, rotated.Height);
                        canvas.RotateDegrees(180);
                        canvas.DrawBitmap(resized, 0, 0);
                    }
                    break;

                case SKEncodedOrigin.LeftBottom: // 270°
                    rotated = new SKBitmap(resized.Height, resized.Width);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.Translate(0, rotated.Height);
                        canvas.RotateDegrees(270);
                        canvas.DrawBitmap(resized, 0, 0);
                    }
                    break;

                default:
                    rotated = resized;
                    break;
            }

            // Bitmap => image
            using var image = SKImage.FromBitmap(rotated);
            using var output = new MemoryStream();
            // 70 = compression quality (0-100), lower = smaller file size but more compression artifacts
            // Note: if using quality < 70, consider using better resampling via SamplingQuality.Mitchell or SamplingQuality.CatmullRom
            image.Encode(SKEncodedImageFormat.Jpeg, 70).SaveTo(output);

            return output.ToArray();
        }

        private static SKSamplingOptions GetSamplingOptions(SamplingQuality quality)
        {
            return quality switch
            {
                SamplingQuality.Low => new SKSamplingOptions(SKFilterMode.Nearest),
                SamplingQuality.Medium => new SKSamplingOptions(SKFilterMode.Linear),
                SamplingQuality.High => new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear),
                SamplingQuality.Mitchell => new SKSamplingOptions(SKCubicResampler.Mitchell),
                SamplingQuality.CatmullRom => new SKSamplingOptions(SKCubicResampler.CatmullRom),
                _ => new SKSamplingOptions(SKFilterMode.Linear)
            };
        }
    }

    public enum SamplingQuality
    {
        Low,
        Medium,
        High,
        Mitchell,
        CatmullRom
    }
}