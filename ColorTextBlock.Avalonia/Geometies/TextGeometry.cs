﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace ColorTextBlock.Avalonia.Geometries
{
    public class TextGeometry : CGeometry
    {
        public string Text { get; }

        public IBrush Foreground { get; set; }
        public IBrush Background { get; set; }
        public bool IsUnderline { get; set; }
        public bool IsStrikethrough { get; set; }

        private FormattedText Format;

        public TextGeometry(
            double width, double height, bool linebreak,
            IBrush foreground, IBrush background,
            bool isUnderline, bool isStrikethrough,
            string text, FormattedText format) : base(width, height, linebreak)
        {
            this.Text = text;
            this.Format = format;

            this.Foreground = foreground;
            this.Background = background;
            this.IsUnderline = isUnderline;
            this.IsStrikethrough = isStrikethrough;
        }


        public static TextGeometry NewLine(FormattedText format)
        {
            return new TextGeometry(
                0, format.Bounds.Height, true,
                null, null,
                false, false,
                "", format);
        }

        public static IEnumerable<TextGeometry> CreateFrom(
            FormattedText format,
            IBrush foreground, IBrush background,
            bool isUnderline, bool isStrikethrough)
        {
            var lines = new List<TextGeometry>();

            string text = format.Text;
            double width = format.Bounds.Width;
            int lineOffset = 0;

            FormattedTextLine[] ftlines = format.GetLines().ToArray();

            for (int idx = 0; idx < ftlines.Length; ++idx)
            {
                FormattedTextLine line = ftlines[idx];
                bool isLast = idx == ftlines.Length - 1;

                string chip = text.Substring(lineOffset, line.Length);
                var geometry = new TextGeometry(
                                        width, line.Height, !isLast,
                                        foreground, background,
                                        isUnderline, isStrikethrough,
                                        chip, format);
                lines.Add(geometry);

                lineOffset += line.Length;
            }

            return lines;
        }

        public override void Render(DrawingContext ctx)
        {
            var background = Background;
            if (background != null)
            {
                ctx.FillRectangle(background, new Rect(Left, Top, Width, Height));
            }

            var pen = new Pen(Foreground);


            Format.Text = Text;
            ctx.DrawText(Foreground, new Point(Left, Top), Format);

            if (IsUnderline)
            {
                ctx.DrawLine(pen,
                    new Point(Left, Top + Height),
                    new Point(Left + Width, Top + Height));
            }

            if (IsStrikethrough)
            {
                ctx.DrawLine(pen,
                    new Point(Left, Top + Height / 2),
                    new Point(Left + Width, Top + Height / 2));
            }
        }
    }
}