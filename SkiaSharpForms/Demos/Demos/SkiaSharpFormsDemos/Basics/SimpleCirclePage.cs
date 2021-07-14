using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using TouchTracking;

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
