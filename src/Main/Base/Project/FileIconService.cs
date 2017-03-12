using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ICSharpCode.SharpDevelop
{
	public static class FileIconService
	{
		private static Dictionary<string, Bitmap> bitmapCache = new Dictionary<string, Bitmap>();

		public static Bitmap GetBitmap(string text)
		{
			Bitmap bitmap = null;
			if (text.ToUpper().StartsWith("FILE:"))
			{
				lock (FileIconService.bitmapCache)
				{
					if (FileIconService.bitmapCache.TryGetValue(text, out bitmap))
					{
						return bitmap;
					}
					string filename = StringParser.Parse(text.Substring(5, text.Length - 5));
					bitmap = (Bitmap)Image.FromFile(filename);
					FileIconService.bitmapCache[text] = bitmap;
				}
				return bitmap;
			}
			return bitmap;
		}
	}
}
