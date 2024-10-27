using System;
using System.IO;
using Android.Graphics;
using Enigma;
using Enigma.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;


[assembly: ExportRenderer(typeof(ImageCircle), typeof(ImageCircleRender))]
namespace Enigma.Droid
{
    public class ImageCircleRender : ImageRenderer
    {
        public ImageCircleRender()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {

                if ((int)Android.OS.Build.VERSION.SdkInt < 18)
                    SetLayerType(LayerType.Software, null);
            }
        }

        protected override bool DrawChild(Canvas canvas, global::Android.Views.View child, long drawingTime)
        {
            try
            {
                var radius = System.Math.Min(Width, Height) / 2;
                var strokeWidth = 10;
                radius -= strokeWidth / 2;

                //Create path to clip
                var path = new Android.Graphics.Path();
                path.AddCircle(Width / 2, Height / 2, radius, Android.Graphics.Path.Direction.Ccw);
                canvas.Save();
                canvas.ClipPath(path);

                var result = base.DrawChild(canvas, child, drawingTime);

                canvas.Restore();

                // Create path for circle border
                path = new Android.Graphics.Path();
                path.AddCircle(Width / 2, Height / 2, radius, Android.Graphics.Path.Direction.Ccw);

                var paint = new Paint();
                paint.AntiAlias = true;
                paint.StrokeWidth = 5;
                paint.SetStyle(Paint.Style.Stroke);
                paint.Color = global::Android.Graphics.Color.White;

                canvas.DrawPath(path, paint);

                //Properly dispose
                paint.Dispose();
                path.Dispose();
                return result;
            }
            catch 
            {
            }

            return base.DrawChild(canvas, child, drawingTime);
        }



    }
}
