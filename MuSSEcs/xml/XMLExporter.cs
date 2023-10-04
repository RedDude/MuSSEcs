
/*
 * Class to append data to the XML file.
 * 
 * Clean when user changes SpriteSheet.
 * 
 */

using System.Drawing;
using System.Text;

namespace MuSSEcs.xml
{
	class XmlExporter {

		// String for the XML
		private StringBuilder _xmlString;
	
		public XmlExporter()
		{
			// Init xml
			_xmlString = new StringBuilder();
		}

		public void Clean()
		{
			// Restart xml
			_xmlString = new StringBuilder();
		}

		/*
	 * Return closed XML to be print in file with image path and color key.
	 */
		public string GetXml(string sheetName, int width, int height, Color sheetCk)
		{ 
			// Open xml
			StringBuilder finalXml = new StringBuilder(
				"<?xml version=\"1.0\"?>\n" +
				"<spritesheet "
				+ "image_name=\"" + sheetName + "\""
				+ " width=\"" + width + "\" height=\"" + height
				+ "\" ck_r=\"" + sheetCk.R + "\" ck_g=\"" + sheetCk.G + "\" ck_b=\"" + sheetCk.B
				+ "\" >\n");

			// Add all Animations saved in xmlString
			finalXml.Append(_xmlString.ToString());
			
			// Close xml
			finalXml.Append("</spritesheet>");

			return finalXml.ToString();
		}
	
		/* 
	 * Every time user adds a new animation,
	 * SpritePanel calls:
	 * 
	 * 		openAnimation("name")
	 * 		add Sprites for that Animation
	 * 		closeAnimation()
	 * 
	 */
		public void OpenAnimation(string s)
		{
			_xmlString.Append("	<animation name=\"" + s + "\">\n");
		}
		public void CloseAnimation(string s)
		{
			_xmlString.Append("	</animation>\n");
		}
	
		/*
	 * Adds individual Sprites (Blobs) to the XML
	 * 
	 */
		public void AddSprite(string s)
		{
			_xmlString.Append(s);
		}
	}
}
