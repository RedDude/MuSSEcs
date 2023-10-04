/*
 * Load SpriteSheet image.
 */

using System.Drawing;
using System.IO;

namespace MuSSEcs.spritesheet
{
	class SpriteSheet {

		/* 
	 * Image files and configs.
	 */
		// System file
		private FileInfo file;
		// Image
		private Bitmap sheet;
		// Value for background (defines blank pixel/transparency), taken from 0,0 position on sheet
		public Color BackgroundValue;
		// Color Key
		Color colorkey;
		// Image path
		string imageName;

		/*
	 *  Loads SpriteSheet file
	 */
		public SpriteSheet(FileInfo src)
		{
			file = src;
			imageName = file.Name;
			sheet = (Bitmap?) Bitmap.FromStream(file.OpenRead());
			// Pixel color reference for background
			BackgroundValue = sheet.GetPixel(0, 0);
			colorkey = sheet.GetPixel(0, 0);
		}
		// This is a fix to properly load resource image inside jar
		public SpriteSheet(string src) {
			//file = src
			imageName = src;
			sheet = (Bitmap?) Bitmap.FromStream(file.OpenRead());
			// Pixel color reference for background
			BackgroundValue = sheet.GetPixel(0, 0);
			colorkey = sheet.GetPixel(0, 0);
		}

		/*
	 * Auxiliary methods (gets and sets).
	 */
		// public File GetFile()
		// {
		// return file; }

		public Bitmap GetImage()
		{
			return sheet; }

		public int GetWidth()
		{
			return sheet.Width; }

		public int GetHeight()
		{
			return sheet.Height; }

		public string GetName()
		{
			return imageName;
		}
	}
}
