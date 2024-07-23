using System.Drawing;
using System.IO;

namespace Facebook.Utility {
    internal sealed class ImageHelper {
        private ImageHelper() {
        }

        internal static Image ConvertBytesToImage(byte[] imageBytes) {
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length)) {
                ms.Write(imageBytes, 0, imageBytes.Length);
                return new Bitmap(ms);
            }
        }
    }
}