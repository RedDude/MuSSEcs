using System.Drawing;
using System.IO;
using MuSSEcs.blobdetection;
using MuSSEcs.spritesheet;

namespace MuSSEcs
{
    public class Program {

        public static void Main()
        {
            var ss = new SpriteSheet(new FileInfo("E:\\Projects\\MuSSEcs\\MuSSEcs\\img\\examples\\test2.png"));
            var bd = new BlobDetection();
        
            var clips = bd.GetSpriteClips(ss, new Rectangle(0, 0, ss.GetWidth(), ss.GetHeight()));
        
        }

    }
}