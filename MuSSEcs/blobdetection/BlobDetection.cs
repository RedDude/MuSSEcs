using System;
using System.Collections.Generic;
using System.Drawing;
using MuSSEcs.spritesheet;

/*
 * Blob detection algorithm.
 * 
 * Blob detection algorithm is a machine-vision solution used
 * mainly to extract singular pieces of image of more complex
 * ones.
 * 
 * In MuSSE, Blob detection is used to extract individual Sprites (blobs)
 * from Sprite Sheets.
 * 
 */
namespace MuSSEcs.blobdetection
{
    class BlobDetection
    {
        /*
     * Collection of SpriteClips (blobs) for current SpriteSheet.
     */
        private List<Blob> _clips;

        /*
     * Connected-component Labeling algorithm.
     */
        // Table of Label Equivalence.
        private LabelEqTable _existingLabels;

        // Pixel ids for Connected-component Labeling.
        private int _nextId;

        /*
     * Threshold for connected-component method.
     * Smaller thresholds will result in a large number of blobs.
     * Higher thresholds will result in a smaller number of blobs.
     */
        private int _halfEdge;

        public BlobDetection()
        {
            _existingLabels = new LabelEqTable();
            _nextId = 1;
            _halfEdge = 8;
            _clips = new List<Blob>();
        }

        // Clean method to reset Clips (blobs) from SpriteSheet.
        public void Clean()
        {
            _clips.Clear();
            _existingLabels = new LabelEqTable();
            _nextId = 1;
        }

        // Find Clips (blobs) on current selection Box.
        public void FindClips(SpriteSheet sheet, Rectangle area)
        {
            _clips = GetSpriteClips(sheet, area);
            // Init clips anchor point
            foreach (Blob b in _clips)
            {
                b.Anchor.X = b.GetX();
                b.Anchor.Y = b.GetY();
            }
        }

        /*
     * The overall goal of connected-component is to label each pixel within
     * a blob with the same identifier.
     *
     * These identifiers are represented by label numbers. First stage is to
     * circle through all the pixels inside the selected area, verifying
     * corresponding label numbers from neighbor pixels.
     *
     */
        public List<Blob> GetSpriteClips(SpriteSheet sheet, Rectangle area)
        {
            var h = area.Height;
            var w = area.Width;
            var x = area.X;
            var y = area.Y;

            /*
         * Creates a list of labels for each pixel in selected area.
         * w -------->
         * h 0 0 0 0		Same image gets same label.
         * | 0 1 1 0		If a pixel get no nearest neighbor, a new label is created.
         * | 0 0 0 0
         * | 0 2 0 0
         * | 0 0 0 0
         * v
         *
         * All the labels are stored in a matrix of equal dimension as the original image.
         * This way, we can set one label entry per pixel in the image. This matrix starts
         * completely unlabeled and, as the algorithm iterate through the image, is filled
         * up by the 8-neighborhood connectivity method.
         *
         */
        var labelSheet = new Label[h, w];

            /*
         * Checks every pixel in selected area.
         */
            for (var row = 0; row < h; row++)
            {
                for (var col = 0; col < w; col++)
                {
                    // Gets current pixel's id.
                    var currentPixel = sheet.GetImage().GetPixel(x + col, y + row);

                    // Checks if pixel is foreground (true) or background (false)
                    if (!IsForeground(currentPixel, sheet.BackgroundValue)) continue;
                    
                    // Checks nearest neighbors for a valid Label or gets a new label
                    var returnedLabel = LabelFromNeighborhood(labelSheet, w, h, col, row, x, y);

                    // Saves pixel Label on sheet
                    labelSheet[row, col] = returnedLabel;

                    // Case null should throw an exception
                    // if (labelSheet[row, col] == null)
                        // Console.WriteLine($"Error null label (x,y) {col} {row} shouldn't be null.");
                }
            }

            // -- DEBUG --
            // Print labels on console.
            /*
          for (int row = 0; row < h; row++) {
            for (int col = 0; col < w; col++)
            {
               if(labelSheet[row][col] != null) System.out.print(existingLabels.getRep(labelSheet[row][col]).getId() + " "); // System.out.print(labelSheet[row][col].getId() + " ");
               else System.out.print("0 ");
            }
             System.out.println();
          }
        */

            /*
         * Reduce the labels to their equivalences.
         * Get pixels and bounding boxes.
         * Create SpriteClips (blobs).
         *
         */
            Dictionary<Label, Blob> clipRegistry = new();
            // Checks every Label in selected area.
            for (var row = 0; row < h; row++)
            {
                for (var col = 0; col < w; col++)
                {
                    // Gets current Label
                    var currentLabel = labelSheet[row, col];

                    // Check if pixel is Labeled
                    if (currentLabel != null)
                    {
                        // Get pixel's representative (origin/reduced Label)
                        Label rep = _existingLabels.GetRep(currentLabel);

                        // First time checking Label
                        if (!clipRegistry.ContainsKey(rep))
                        {
                            // Create a new SpriteClip (blob) for that Label
                            var sc = new Blob(sheet.GetImage(), y + row, x + col, rep.ToString());
                            clipRegistry.Add(rep, sc);
                        }
                        // Existing label
                        else
                        {
                            // Add Label to existing SpriteClip (blob) based on their representative (origin/reduced Label)
                            var sc = clipRegistry[rep];
                            sc.AddLocation(y + row, x + col);
                        }
                    }
                }
            }

            /*
         * Now we have all the SpriteClips (blobs) stored in a map.
         */
            List<Blob> returnList = new();
            returnList.AddRange(clipRegistry.Values);
            return returnList;
        }

        /*
     * Check if valid pixel.
     * Return true  in case foreground (not background).
     * Return false in case background.
     */
        private bool IsForeground(Color pixel, Color backgroundValue)
        {
            return pixel != backgroundValue;
        }

        /*
     * Apply 8-adjacent search path, likely: 8-neighborhood connectivity,
     * in order to Label a pixel based on its neighborhood.
     *
     */
        private Label? LabelFromNeighborhood(Label?[,] labelSheet,
            int width, // Selection Box width
            int height, // Selection Box height
            int colPixel, // Pixel Label on sheet
            int rowPixel, // Pixel Label on sheet
            int xImg, // Selection Box start pos X
            int yImg) // Selection Box start pos Y
        {
            var colStart = Math.Max(0, colPixel - _halfEdge); // Starting X 	(on Label list)
            var rowStart = Math.Max(0, rowPixel - _halfEdge); // Starting Y 	(on Label list)
            var colEnd = Math.Min(width - 1, colPixel + _halfEdge); // Ending X 	(on Label list)
            var rowEnd = Math.Min(height - 1, rowPixel + _halfEdge); // Ending Y 	(on Label list)

            Label? assignedLabel = null;

            // -- DEBUG --
            // System.out.println("Clip start at (x,y): " + colStart + " " + rowStart);
            // System.out.println("Clip ends  at (x,y): " + colEnd + " " + rowEnd);

            /* 8-neighborhood connectivity
         *
         * L1 L2 L3
         * L4 C
         *
         * C: Center Pixel
         * Ln: Neighbors 1 to 4
         *
         * Identify Labels of neighborhood pixels L1, L2, L3 and L4,
         * and apply to given position C.
         *
         */
            // Check nearest neighbors for a valid Label.
            for (var row = rowStart; row <= rowEnd; row++)
            {
                for (var col = colStart; col <= colEnd; col++)
                {
                    // If there's no valid label on neighborhood
                    if (row == rowPixel && col == colPixel)
                    {
                        // Gets a new Label
                        if (assignedLabel == null)
                        {
                            assignedLabel = NextLabel();
                            _existingLabels.AddLabel(assignedLabel);
                        }

                        // No need to check additional neighbors.
                        break;
                    }

                    // Get neighbor's Label, if any
                    var neighbor = labelSheet[row, col];
                    if (neighbor != null &&
                        assignedLabel == null)
                    {
                        assignedLabel = neighbor;
                    }

                    // Check neighbors and mark equivalences.
                    if (neighbor != null && neighbor != assignedLabel) {
                        if (!_existingLabels.HasLabel(assignedLabel))
                        {
                            _existingLabels.AddLabel(assignedLabel);
                        }

                        _existingLabels.SetComembers(neighbor, assignedLabel);
                    }
                    
                    // if (neighbor == null || neighbor == assignedLabel) continue;
                    // if (!_existingLabels.HasLabel(assignedLabel))
                    // {
                    //     _existingLabels.AddLabel(assignedLabel);
                    // }
                    //
                    // _existingLabels.SetComembers(neighbor, assignedLabel);
                }
            }

            return assignedLabel;
        }

        /*
     * Returns next valid Label.
     */
        private Label NextLabel()
        {
            return new Label(_nextId++);
        }


        /*
     * Auxiliary methods (gets and sets).
     */
        public List<Blob> GetClips()
        {
            return _clips;
        }

        public void SetThreshold(int t)
        {
            this._halfEdge = t;
        }
    }
}