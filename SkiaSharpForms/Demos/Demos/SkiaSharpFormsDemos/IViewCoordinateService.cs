using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaSharpFormsDemos
{
    public interface IViewCoordinateService
    {
        System.Drawing.PointF GetCoordinates ( global::Xamarin.Forms.VisualElement view );
    }
}
