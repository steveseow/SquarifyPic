using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SquarifyPic.Controllers
{
    public class PicHelperController : ApiController
    {
        [Route("Squarify")]
        [HttpPost]
        public string Squarify(string imageUri)
        {
            return ProcessSquarify(imageUri);
        }

        private string ProcessSquarify(string imageUri)
        {
            var request = WebRequest.Create(imageUri);
            try
            {
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    Image image = Image.FromStream(stream);
                    int h = image.Height;
                    int w = image.Width;
                    int s = Math.Min(h, w);
                    s = Math.Min(s, 100); //100 px or smaller
                    Image imageSquared = ResizeImage(image, new Size(s, s));
                }
            }
            catch (Exception)
            {

            }
        }


        public static Image ResizeImage(Image imgToResize, Size destinationSize)
        { 
            var originalWidth = imgToResize.Width;
            var originalHeight = imgToResize.Height;

            //how many units are there to make the original length
            var hRatio = (float)originalHeight / destinationSize.Height;
            var wRatio = (float)originalWidth / destinationSize.Width;

            //get the shorter side
            var ratio = Math.Min(hRatio, wRatio);

            var hScale = Convert.ToInt32(destinationSize.Height * ratio);
            var wScale = Convert.ToInt32(destinationSize.Width * ratio);

            //start cropping from the center
            var startX = (originalWidth - wScale) / 2;
            var startY = (originalHeight - hScale) / 2;

            //crop the image from the specified location and size
            var sourceRectangle = new Rectangle(startX, startY, wScale, hScale);

            //the future size of the image
            var bitmap = new Bitmap(destinationSize.Width, destinationSize.Height);

            //fill-in the whole bitmap
            var destinationRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            //generate the new image
            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }

            return bitmap;

        }


    }
}

