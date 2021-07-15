using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using TouchTracking;
using SkiaSharpFormsDemos.Bitmaps;

namespace SkiaSharpFormsDemos.Basics
{
    public class SimpleCirclePage : ContentPage
    {
        SKBitmap monkeyBitmap;

        public SimpleCirclePage()
        {
            Title = "Simple Circle";

            monkeyBitmap = BitmapExtensions.LoadBitmapResource ( GetType () ,
                "SkiaSharpFormsDemos.Media.MonkeyFace.png" );

            // Create canvas based on bitmap
            using ( SKCanvas canvas = new SKCanvas ( monkeyBitmap ) )
            {
                using ( SKPaint paint = new SKPaint () )
                {
                    paint.Style = SKPaintStyle.Stroke;
                    paint.Color = SKColors.Black;
                    paint.StrokeWidth = 24;
                    paint.StrokeCap = SKStrokeCap.Round;

                    using ( SKPath path = new SKPath () )
                    {
                        path.MoveTo ( 380 , 390 );
                        path.CubicTo ( 560 , 390 , 560 , 280 , 500 , 280 );

                        path.MoveTo ( 320 , 390 );
                        path.CubicTo ( 140 , 390 , 140 , 280 , 200 , 280 );

                        canvas.DrawPath ( path , paint );

                        canvas.DrawCircle ( 5 , 5 , 5 , paint );
                    }
                }
            }

            //SKCanvasView canvasView = new SKCanvasView();
            //canvasView.PaintSurface += OnCanvasViewPaintSurface;
            //Content = canvasView;

            CustomSKCanvas customSKCanvas = new CustomSKCanvas();

            //touchHandler = new TouchHandler () { UseTouchSlop = true };
            



            customSKCanvas.PaintSurface += OnCanvasViewPaintSurface;
            Content = customSKCanvas;
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            //SKImageInfo info = args.Info;
            //SKSurface surface = args.Surface;
            //SKCanvas canvas = surface.Canvas;

            //canvas.Clear();

            //SKPaint paint = new SKPaint
            //{
            //    Style = SKPaintStyle.Stroke,
            //    Color = Color.Red.ToSKColor(),
            //    StrokeWidth = 25
            //};
            //canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, paint);

            //paint.Style = SKPaintStyle.Fill;
            //paint.Color = SKColors.Blue;
            //canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, paint);
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear ();
            canvas.DrawBitmap ( monkeyBitmap , info.Rect , BitmapStretch.None );
        }
    }
}
