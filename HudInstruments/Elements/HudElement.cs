using System;
using System.Text;
using System.Drawing;

using ARDrone.Hud.Utils;

namespace ARDrone.Hud.Elements
{
    public abstract class HudElement
    {
        protected const float hudFontSize = 8.0f;

        protected HudConstants constants;

        protected Pen hudPen;
        protected Font hudFont;
        protected Brush hudBrush;

        protected int currentWidth;
        protected int currentHeight;

        public HudElement(HudConstants constants)
        {
            this.constants = constants;

            hudBrush = new SolidBrush(Color.LightGreen);
            hudPen = new Pen(hudBrush, 1.0f);
            hudFont = new Font("Courier New", hudFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        public abstract Bitmap DrawToImage(Bitmap bitmap, HudState currentState);

        public virtual void InitBaseVariables(Bitmap bitmap, HudState currentState)
        {
            currentWidth = bitmap.Width;
            currentHeight = bitmap.Height;
        }

    }
}
