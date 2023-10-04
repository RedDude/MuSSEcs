/*
 * Define Labels for the SpriteClip (blob).
 * 
 */

namespace MuSSEcs.blobdetection
{
    class Label
    {
        public int id;

        public Label(int _id)
        {
            id = _id;
        }

        public override string ToString()
        {
            return "Label" + id;
        }

        public int GetId()
        {
            return this.id;
        }
    }
}