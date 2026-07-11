using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace LastChaos_ToolBoxNG
{
	internal static class TexImageLoader
	{
		public static Bitmap? Load(string filePath)
		{
			try
			{
				using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				using BinaryReader reader = new(fileStream);

				reader.ReadBytes(4);
				reader.ReadInt32();
				reader.ReadBytes(4);

				uint width = reader.ReadUInt32() ^ 303316286U;
				uint shift = reader.ReadUInt32() ^ 1431797889U;
				uint height = reader.ReadUInt32() ^ 2560279492U;
				uint mipMap = reader.ReadUInt32() ^ 3688695303U;
				uint bits = reader.ReadUInt32() ^ 505432394U;
				reader.ReadUInt32();
				string format = Encoding.ASCII.GetString(reader.ReadBytes(4));
				reader.ReadInt32();

				width >>= (int)shift;
				height >>= (int)shift;

				if (format != "FRMS" || width == 0 || height == 0 || width > int.MaxValue || height > int.MaxValue)
					return null;

				int bytesPerPixel = bits == 0 || bits == 2 ? 3 : 4;
				long imageSize = (long)width * height * bytesPerPixel;

				if (imageSize <= 0 || imageSize > int.MaxValue || fileStream.Length - fileStream.Position < imageSize)
					return null;

				byte[] imageData = reader.ReadBytes((int)imageSize);

				return bytesPerPixel == 3
					? MakeRgb(imageData, (int)width, (int)height)
					: MakeArgb(imageData, (int)width, (int)height);
			}
			catch
			{
				return null;
			}
		}

		private static Bitmap MakeArgb(byte[] imageData, int width, int height)
		{
			byte[] destination = new byte[width * height * 4];

			for (int sourceIndex = 0, destinationIndex = 0; sourceIndex + 3 < imageData.Length; sourceIndex += 4, destinationIndex += 4)
			{
				destination[destinationIndex] = imageData[sourceIndex + 2];
				destination[destinationIndex + 1] = imageData[sourceIndex + 1];
				destination[destinationIndex + 2] = imageData[sourceIndex];
				destination[destinationIndex + 3] = imageData[sourceIndex + 3];
			}

			Bitmap bitmap = new(width, height, PixelFormat.Format32bppArgb);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
			Marshal.Copy(destination, 0, bitmapData.Scan0, destination.Length);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		private static Bitmap MakeRgb(byte[] imageData, int width, int height)
		{
			byte[] destination = new byte[width * height * 3];

			for (int sourceIndex = 0, destinationIndex = 0; sourceIndex + 2 < imageData.Length; sourceIndex += 3, destinationIndex += 3)
			{
				destination[destinationIndex] = imageData[sourceIndex + 2];
				destination[destinationIndex + 1] = imageData[sourceIndex + 1];
				destination[destinationIndex + 2] = imageData[sourceIndex];
			}

			Bitmap bitmap = new(width, height, PixelFormat.Format24bppRgb);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
			Marshal.Copy(destination, 0, bitmapData.Scan0, destination.Length);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}
	}
}
