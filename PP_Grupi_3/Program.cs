using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace PP_Grupi_3
{
    class Program
    {
        private static readonly string DebugPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        private static readonly string LocalPath = Path.GetDirectoryName(Path.GetDirectoryName(DebugPath));
        private static readonly ParallelOptions ParallelOptions = new ParallelOptions();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Shtypni numrin e test: ");
                var testImage = Convert.ToInt32(Console.ReadLine());
                var bitmap = new Bitmap($@"{LocalPath}\Images\test{testImage}.jpg");
                Console.WriteLine("Shtypni numrin e cores: ");
                ParallelOptions.MaxDegreeOfParallelism = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Shtypni numrin e ndryshimit të fotografisë: ");
                var factor = Convert.ToDouble(Console.ReadLine());
                var sw = new Stopwatch();
                sw.Start();
                ChangeColorByFactor(bitmap, factor);
                bitmap.Save($@"{LocalPath}\Images\test{testImage}-c.{ParallelOptions.MaxDegreeOfParallelism}-f.{factor}.jpg");
                Console.WriteLine($"Procesimi zgjati: {sw.ElapsedMilliseconds} ms");
            }
        }

        private static unsafe void ChangeColorByFactor(Bitmap bitmap, double factor)
        {
            var bitmapMemory = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            Parallel.For(0, bitmapMemory.Height, ParallelOptions, i =>
            {
                var rgb = (byte*)bitmapMemory.Scan0 + i * bitmapMemory.Stride;
                for (var x = 0; x < bitmapMemory.Width * bytesPerPixel; x = x + bytesPerPixel)
                {
                    rgb[x] = (byte)(rgb[x] * factor);
                    rgb[x + 1] = (byte)(rgb[x + 1] * factor);
                    rgb[x + 2] = (byte)(rgb[x + 2] * factor);
                }
            });
            bitmap.UnlockBits(bitmapMemory);
        }
    }
}
