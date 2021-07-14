using System;
using System.Collections.Generic;

using Xamarin.Forms;

using TouchTracking;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaSharpFormsDemos.Basics;


namespace SkiaSharpFormsDemos.Bitmaps
{
    public partial class FingerPaintSavePage : ContentPage
    {
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();

        SKBitmap myBitmap;

        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        SKBitmap saveBitmap;

        public FingerPaintSavePage ()
        {
            myBitmap = BitmapExtensions.LoadBitmapResource ( GetType () ,
                "SkiaSharpFormsDemos.Media.Pinceszinti_alaprajz.png" );

            InitializeComponent ();
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            /*// Create bitmap the size of the display surface
            if (saveBitmap == null)
            {
                saveBitmap = new SKBitmap(info.Width, info.Height);
            }
            // Or create new bitmap for a new size of display surface
            else if (saveBitmap.Width < info.Width || saveBitmap.Height < info.Height)
            {
                SKBitmap newBitmap = new SKBitmap(Math.Max(saveBitmap.Width, info.Width),
                                                  Math.Max(saveBitmap.Height, info.Height));

                using (SKCanvas newCanvas = new SKCanvas(newBitmap))
                {
                    newCanvas.Clear();
                    newCanvas.DrawBitmap(saveBitmap, 0, 0);
                }

                saveBitmap = newBitmap;
            }



            */


            


            // Render the bitmap
            canvas.Clear();
            //CustomSKCanvas customSKCanvas

            //canvas.DrawBitmap(saveBitmap, 0, 0);
            canvas.DrawBitmap ( myBitmap , /*info.Rect*/SKRect.Create ( 1500 , 1500 ) , BitmapStretch.None );

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

        void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (!inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = new SKPath();
                        path.MoveTo(ConvertToPixel(args.Location));
                        inProgressPaths.Add(args.Id, path);
                        UpdateBitmap();
                    }
                    break;

                case TouchActionType.Moved:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = inProgressPaths[args.Id];
                        path.LineTo(ConvertToPixel(args.Location));
                        UpdateBitmap();
                    }
                    break;

                case TouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        completedPaths.Add(inProgressPaths[args.Id]);
                        inProgressPaths.Remove(args.Id);
                        UpdateBitmap();
                    }
                    break;

                case TouchActionType.Cancelled:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
                        UpdateBitmap();
                    }
                    break;
            }
        }

        SKPoint ConvertToPixel(Point pt)
        {
            return new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                               (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }

        void UpdateBitmap()
        {
            //using ( SKCanvas saveBitmapCanvas = new SKCanvas ( monkeyBitmap ) )
            //{
            //    saveBitmapCanvas.Clear ();

            //    foreach ( SKPath path in completedPaths )
            //    {
            //        saveBitmapCanvas.DrawPath ( path , paint );
            //    }

            //    foreach ( SKPath path in inProgressPaths.Values )
            //    {
            //        saveBitmapCanvas.DrawPath ( path , paint );
            //    }
            //}

            //canvasView.InvalidateSurface ();
        }

        void OnClearButtonClicked(object sender, EventArgs args)
        {
            completedPaths.Clear();
            inProgressPaths.Clear();
            UpdateBitmap();
            canvasView.InvalidateSurface();
        }

        async void OnSaveButtonClicked(object sender, EventArgs args)
        {
            using (SKImage image = SKImage.FromBitmap(saveBitmap))
            {
                SKData data = image.Encode();
                DateTime dt = DateTime.Now;
                string filename = String.Format("FingerPaint-{0:D4}{1:D2}{2:D2}-{3:D2}{4:D2}{5:D2}{6:D3}.png",
                                                dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

                IPhotoLibrary photoLibrary = DependencyService.Get<IPhotoLibrary>();
                bool result = await photoLibrary.SavePhotoAsync(data.ToArray(), "FingerPaint", filename);

                if (!result)
                {
                    await DisplayAlert("FingerPaint", "Artwork could not be saved. Sorry!", "OK");
                }
            }
        }
                
    }
}