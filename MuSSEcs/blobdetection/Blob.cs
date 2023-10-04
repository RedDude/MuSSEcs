using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

/*
 * Blob.
 * 
 * A blob is a region of an image that contains approximately close 
 * characteristics, such as brightness or color. A blob is defined 
 * as a region of connected pixels. Blob detection is used to identify 
 * these regions in images.
 * 
 * These blobs are used in MuSSE to save meta-data for Sprites inside a
 * SpriteSheet, i.e.:
 * 		offset_x, offset_y (position); and
 * 		width, height (size).
 * 
 */
namespace MuSSEcs.blobdetection
{
    class Blob
    {
        // Blob identification.
        private string _name;

        // Blob boundingBox containing position and size.
        private Rectangle? _boundingBox;

        // Set of Points (pixels) contained in the boundingBox
        private HashSet<Point> _points = new();

        // Anchor point for the Blob image
        public Point Anchor;

        // Related SpriteSheet.
        private Bitmap _parentImage;

        /*
     *  Create SpriteClip (blob) for current SpriteSheet.
     */
        public Blob(Bitmap parentImage, int row, int col, string name)
        {
            _parentImage = parentImage;
            _name = name;
            AddLocation(row, col);
        }

        // Add Point (pixel) to current SpriteClip (blob).
        public void AddLocation(int row, int col)
        {
            var p = new Point(col, row);
            _points.Add(p);
            if (_boundingBox == null)
            {
                _boundingBox = new Rectangle(col, row, 1, 1);
                Anchor = new Point(col, row);
                return;
            }

            _boundingBox.Value.Inflate(p.X, p.Y);
        }

        // Save Blob Image to File
        // TODO - Testar.
        /*
     * la em MuSSE fazer um botao "extrair sprites individuais da selecao"
     * foreach blob
     * b.saveTo("blob1", "jpg")
     * 
     */
        // NYI.
        public void SaveTo(FileInfo directory, string format)
        {
            // throws IOException {
            var outfile = new FileInfo(directory + _name + "." + format);
            //ImageIO.write(makeCutout(null, null), format, outfile);
        }

        /*
     * Auxiliary methods (get's).
     */
        public Point GetAnchorPoint()
        {
            return Anchor;
        }

        public Rectangle? GetBoundingBox()
        {
            return _boundingBox;
        }

        public int GetX()
        {
            return _boundingBox!.Value.X;
        }

        public int GetY()
        {
            return _boundingBox!.Value.Y;
        }

        // Width of the bounding box is not the same as the number of pixels
        public int GetWidth()
        {
            return _boundingBox.Value.Width + 1;
        }

        // Height of the bounding box is not the same as the number of pixels
        public int GetHeight()
        {
            return _boundingBox.Value.Height + 1;
        }

        public string GetName()
        {
            return _name;
        }

        /*
     * Return String in Plain txt format.
     * Create String description for the Sprite (Blob) with name and Bounding Box data (x, y, w, h) with points (opt).
     * 
     */
        public override string ToString()
        {
            var s = new StringBuilder(
                "Data dump for clip " + _name + "\n" +
                "Bounding box x: " + _boundingBox.Value.X +
                ", y: " + _boundingBox.Value.Y +
                ", w: " + _boundingBox.Value.Width +
                ", h: " + _boundingBox.Value.Height + "\n" +
                "Anchor point x: " + Anchor.X +
                ", y: " + Anchor.Y + "\n");
            /* 
            s.append("Points:\n")
            for (Point curPoint : points) {
                s.append("\t x: " + curPoint.x + ", y: " + curPoint.y + "\n")
            } 
        */
            s.Append("Clip W: " + GetWidth() + ", H: " + GetHeight() + "\n");
            return s.ToString();
        }

        /*
     * Return String in XML format.
     * Create XML description for the Sprite (Blob) with name and Bounding Box data (x, y, w, h).
     * 
     */
        public string ToXml()
        {
            var s = new StringBuilder(
                "		<sprite name=\"" + _name + "\">\n" +
                "			<offset_x>" + _boundingBox.Value.X + "</offset_x>\n" +
                "			<offset_y>" + _boundingBox.Value.Y + "</offset_y>\n" +
                "			<width>" + _boundingBox.Value.Width + "</width>\n" +
                "			<height>" + _boundingBox.Value.Height + "</height>\n" +
                "			<anchor_x>" + (Anchor.X - _boundingBox.Value.X) + "</anchor_x>\n" +
                "			<anchor_y>" + (Anchor.Y - _boundingBox.Value.Y) + "</anchor_y>\n" +
                "		</sprite>\n");
            return s.ToString();
        }
    }
}