using System;
using System.Collections.Generic;

using Xamarin.Forms;

using TouchTracking;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaSharpFormsDemos.Basics;
using SkiaSharpFormsDemos.Transforms;

namespace SkiaSharpFormsDemos.Bitmaps
{
    public partial class FingerPaintSavePage : ContentPage
    {
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();

        Dictionary<long, SKPoint> touchDictionary = new Dictionary<long, SKPoint>();

        SKCanvas canvas;
        TouchManipulationBitmap tmBitmap;
        SKBitmap skBitmap;
        SKMatrix matrix = SKMatrix.MakeIdentity();
        SKMatrix canvasMatrix;
        List<long> touchIds = new List<long>();
        MatrixDisplay matrixDisplay = new MatrixDisplay();
        SKPoint previousPoint;

        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        public FingerPaintSavePage ()
        {
            skBitmap = BitmapExtensions.LoadBitmapResource ( GetType () ,
                "SkiaSharpFormsDemos.Media.Pinceszinti_alaprajz.png" );

            var height = skBitmap.Height; //2572
            var width = skBitmap.Width; //3640

            tmBitmap = new TouchManipulationBitmap ( skBitmap );
            tmBitmap.TouchManager.Mode = TouchManipulationMode.IsotropicScale;

            InitializeComponent ();
        }

        void OnCanvasViewPaintSurface ( object sender , SKPaintSurfaceEventArgs args )
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            canvas = surface.Canvas;

            canvas.Clear ();

            //FOR SOLUTION 1
            tmBitmap.Paint( canvas );
            //END FOR SOLUTION 1

            //FOR SOLUTION 2 & 3
            //canvas.DrawBitmap ( skBitmap , new SKPoint () );

            //using ( SKPaint paint = new SKPaint () )
            //{
            //    paint.Style = SKPaintStyle.Stroke;
            //    paint.Color = SKColors.Black;
            //    paint.StrokeWidth = 24;
            //    paint.StrokeCap = SKStrokeCap.Round;

            //    using ( SKPath path = new SKPath () )
            //    {
            //        path.MoveTo ( 380 , 390 );
            //        path.CubicTo ( 560 , 390 , 560 , 280 , 500 , 280 );

            //        path.MoveTo ( 320 , 390 );
            //        path.CubicTo ( 140 , 390 , 140 , 280 , 200 , 280 );

            //        canvas.DrawPath ( path , paint );

            //        canvas.DrawCircle ( 5 , 5 , 5 , paint );
            //    }
            //}
            //END FOR SOLUTION 2 & 3
        }

        /// SOLUTION 1
        /// With using TouchManipulationBitmap
        /// It can handle the moving and zooming of the bitmap, but not the canvas, and the drawings on the canvas
        void OnTouchEffectAction ( object sender , TouchActionEventArgs args )
        {
            Point pt = args.Location;
            SKPoint point =
                new SKPoint( (float)( canvasView.CanvasSize.Width * pt.X / canvasView.Width ),
                            (float)( canvasView.CanvasSize.Height * pt.Y / canvasView.Height ) );

            switch ( args.Type )
            {
                case TouchActionType.Pressed:
                if ( tmBitmap.HitTest ( point ) )
                {
                    touchIds.Add ( args.Id );
                    tmBitmap.ProcessTouchEvent ( args.Id , args.Type , point );
                    break;
                }
                break;

                case TouchActionType.Moved:
                if ( touchIds.Contains ( args.Id ) )
                {
                    tmBitmap.ProcessTouchEvent ( args.Id , args.Type , point );
                    canvasView.InvalidateSurface ();
                }
                break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                if ( touchIds.Contains ( args.Id ) )
                {
                    tmBitmap.ProcessTouchEvent ( args.Id , args.Type , point );
                    touchIds.Remove ( args.Id );
                    canvasView.InvalidateSurface ();
                }
                break;
            }

        }
        ///END SOLUTION 1

        ///SOLUTION 2
        ///We can reach the coordinates of the tap, but we can't move the bitmap
        //void OnTouchEffectAction ( object sender , TouchActionEventArgs args )
        //{
        //    switch ( args.Type )
        //    {
        //        case TouchActionType.Pressed:
        //        if ( !inProgressPaths.ContainsKey ( args.Id ) )
        //        {
        //            SKPath path = new SKPath();
        //            path.MoveTo ( ConvertToPixel ( args.Location ) );
        //            inProgressPaths.Add ( args.Id , path );
        //            UpdateBitmap ();
        //        }
        //        break;

        //        case TouchActionType.Moved:
        //        if ( inProgressPaths.ContainsKey ( args.Id ) )
        //        {
        //            SKPath path = inProgressPaths[args.Id];
        //            path.LineTo ( ConvertToPixel ( args.Location ) );
        //            UpdateBitmap ();
        //        }
        //        break;

        //        case TouchActionType.Released:
        //        if ( inProgressPaths.ContainsKey ( args.Id ) )
        //        {
        //            completedPaths.Add ( inProgressPaths [ args.Id ] );
        //            inProgressPaths.Remove ( args.Id );
        //            UpdateBitmap ();
        //        }
        //        break;

        //        case TouchActionType.Cancelled:
        //        if ( inProgressPaths.ContainsKey ( args.Id ) )
        //        {
        //            inProgressPaths.Remove ( args.Id );
        //            UpdateBitmap ();
        //        }
        //        break;
        //    }
        //}

        /// SOLUTION 3
        /// We can move the bitmap and the canvas with the graphics (not the whole picture),
        /// but we can't get the tap positions
        /// With using the CustomSKCanvas 
        /// It's necessary to put Solution 1 & 2 into comment to check Solution 3



        SKPoint ConvertToPixel ( Point pt )
        {
            return new SKPoint ( ( float ) ( canvasView.CanvasSize.Width * pt.X / canvasView.Width ) ,
                               ( float ) ( canvasView.CanvasSize.Height * pt.Y / canvasView.Height ) );
        }

        void UpdateBitmap ()
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

    }

    public class CustomSKCanvas : SKCanvasView
    {
        private const double MIN_SCALE = 1;
        private const double MAX_SCALE = 4;
        private const double OVERSHOOT = 0.15;
        private double StartScale, LastScale;
        private double StartX, StartY;

        public CustomSKCanvas ()
        {
            var pinch = new PinchGestureRecognizer();
            pinch.PinchUpdated += OnPinchUpdated;
            GestureRecognizers.Add ( pinch );

            var pan = new PanGestureRecognizer();
            pan.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add ( pan );

            var tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            tap.Tapped += OnTapped;
            GestureRecognizers.Add ( tap );

            Scale = MIN_SCALE;
            TranslationX = TranslationY = 0;
            AnchorX = AnchorY = 0;
        }

        protected override SizeRequest OnMeasure ( double widthConstraint , double heightConstraint )
        {
            Scale = MIN_SCALE;
            TranslationX = TranslationY = 0;
            AnchorX = AnchorY = 0;
            return base.OnMeasure ( widthConstraint , heightConstraint );
        }

        private void OnTapped ( object sender , EventArgs e )
        {
            if ( Scale > MIN_SCALE )
            {
                this.ScaleTo ( MIN_SCALE , 250 , Easing.CubicInOut );
                this.TranslateTo ( 0 , 0 , 250 , Easing.CubicInOut );
            }
            else
            {
                var ve = (Xamarin.Forms.VisualElement) sender;
                var tappedPos = DependencyService.Get<IViewCoordinateService>().GetCoordinates(ve);

                AnchorX = AnchorY = 0.3; //TODO tapped position
                this.ScaleTo ( MAX_SCALE , 250 , Easing.CubicInOut );
            }
        }

        private void OnPanUpdated ( object sender , PanUpdatedEventArgs e )
        {
            switch ( e.StatusType )
            {
                case GestureStatus.Started:
                StartX = ( 1 - AnchorX ) * Width;
                StartY = ( 1 - AnchorY ) * Height;
                break;
                case GestureStatus.Running:
                AnchorX = Clamp ( 1 - ( StartX + e.TotalX ) / Width , 0 , 1 );
                AnchorY = Clamp ( 1 - ( StartY + e.TotalY ) / Height , 0 , 1 );
                break;
            }
        }

        private void OnPinchUpdated ( object sender , PinchGestureUpdatedEventArgs e )
        {
            switch ( e.Status )
            {
                case GestureStatus.Started:
                LastScale = e.Scale;
                StartScale = Scale;
                AnchorX = e.ScaleOrigin.X;
                AnchorY = e.ScaleOrigin.Y;
                break;
                case GestureStatus.Running:
                if ( e.Scale < 0 || Math.Abs ( LastScale - e.Scale ) > ( LastScale * 1.3 ) - LastScale )
                { return; }
                LastScale = e.Scale;
                var current = Scale + (e.Scale - 1) * StartScale;
                Scale = Clamp ( current , MIN_SCALE * ( 1 - OVERSHOOT ) , MAX_SCALE * ( 1 + OVERSHOOT ) );
                break;
                case GestureStatus.Completed:
                if ( Scale > MAX_SCALE )
                    this.ScaleTo ( MAX_SCALE , 250 , Easing.SpringOut );
                else if ( Scale < MIN_SCALE )
                    this.ScaleTo ( MIN_SCALE , 250 , Easing.SpringOut );
                break;
            }
        }

        private T Clamp<T> ( T value , T minimum , T maximum ) where T : IComparable
        {
            if ( value.CompareTo ( minimum ) < 0 )
                return minimum;
            else if ( value.CompareTo ( maximum ) > 0 )
                return maximum;
            else
                return value;
        }

        void OnTouchEffectAction ( object sender , TouchActionEventArgs args )
        {
            switch ( args.Type )
            {
                case TouchActionType.Pressed:
                //if ( !inProgressPaths.ContainsKey ( args.Id ) )
                //{
                //    SKPath path = new SKPath();
                //    path.MoveTo ( ConvertToPixel ( args.Location ) );
                //    inProgressPaths.Add ( args.Id , path );
                //    UpdateBitmap ();
                //}
                break;

                //case TouchActionType.Moved:
                //if ( inProgressPaths.ContainsKey ( args.Id ) )
                //{
                //    SKPath path = inProgressPaths[args.Id];
                //    path.LineTo ( ConvertToPixel ( args.Location ) );
                //    UpdateBitmap ();
                //}
                //break;

                //case TouchActionType.Released:
                //if ( inProgressPaths.ContainsKey ( args.Id ) )
                //{
                //    completedPaths.Add ( inProgressPaths [ args.Id ] );
                //    inProgressPaths.Remove ( args.Id );
                //    UpdateBitmap ();
                //}
                //break;

                //case TouchActionType.Cancelled:
                //if ( inProgressPaths.ContainsKey ( args.Id ) )
                //{
                //    inProgressPaths.Remove ( args.Id );
                //    UpdateBitmap ();
                //}
                //break;
            }
        }
    }
}