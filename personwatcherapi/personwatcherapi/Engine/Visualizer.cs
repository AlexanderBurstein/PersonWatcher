using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using personwatcherapi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel;
using System;
using System.Reflection.Metadata;
using System.Drawing.Text;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace personwatcherapi.Engine
{
    public class Visualizer
    {
        const int CIRCLE_SIZE = 540;
        const int TEXT_ROW_HEIGHT = 25;
        const int PADDING_LINE = 12;
        private int[] SIGN_GLYPHS = { 97, 115, 100, 102, 103, 104, 106, 107, 108, 122, 120, 99 };
        private int[] PLANET_GLYPHS = { 81, 87, 69, 82, 84, 89, 85, 73, 79, 80, 60 };
        private double[] _transits;    
        private double[] _houses;
        private IEnumerable<PersonDetailsForVisualizer> _persons;

        public Visualizer(double[] transits, double[] houses, IEnumerable<PersonDetailsForVisualizer> persons)
        {
            _transits = transits;
            _houses = houses;
            _persons = persons;
        }
        public string CreateImage(string fontsPath)
        {
            var result = "";
            if (_persons != null && _persons.Count() > 0) 
            {
                result = _persons.First().Id.ToString() + ".jpg";
            }
            var chart = new Bitmap(2 * (CIRCLE_SIZE + PADDING_LINE), _persons.Count() / 2 * (CIRCLE_SIZE + TEXT_ROW_HEIGHT + PADDING_LINE));
            // specify the colors
            var almostWhiteBrush = new SolidBrush(Color.FromArgb(0, 254, 254, 254));
            var redPen = new Pen(Brushes.Red);
            var bluePen = new Pen(Brushes.Blue);
            var greenPen = new Pen(Brushes.Green);
            var anotherGreenBrush = new SolidBrush(Color.FromArgb(255, 0, 128, 0));
            var greyPen = new Pen(Color.FromArgb(255, 153, 153, 153));
            var tColorBrush = new SolidBrush(Color.FromArgb(255, 153, 51, 255));
            var anotherBlueBrush = new SolidBrush(Color.FromArgb(255, 212, 232, 242));

            // specific colors
            var plaNatBrush = Brushes.Black;
            var plaTransBrush = tColorBrush;
            //Geometry Stuff
            var rectSize = CIRCLE_SIZE;    // size of rectangle in which to draw the wheel
            var diameter = CIRCLE_SIZE * 13 / 16;   // diameter of circle drawn
            var outerOuterDiameter = CIRCLE_SIZE * 15 / 16;      // diameter of circle drawn
            var outerDiameterDistance = (outerOuterDiameter - diameter) / 2; // distance between outer-outer diameter and diameter
            var innerDiameterOffset = CIRCLE_SIZE * 13 / 128;     // diameter of inner circle drawn
            var innerDiameterOffset2 = CIRCLE_SIZE * 9 / 128;   // diameter of nextmost inner circle drawn
            var distFromDiameter1 = CIRCLE_SIZE / 20;      // distance inner planet glyph is from circumference of wheel
            var radius = diameter / 2;        // radius of circle drawn
            var middleRadius = CIRCLE_SIZE * 277 / 640;   //the radius for the middle of the two outer circles

            var courierFont = new Font(FontFamily.GenericMonospace, 12);
            PrivateFontCollection collection = new PrivateFontCollection();
            var fontFilePath = Path.Combine(fontsPath, "HamburgSymbols.ttf");
            collection.AddFontFile(fontFilePath);
            FontFamily fontFamily = new FontFamily("HamburgSymbols", collection);
            var astroFont = new Font(fontFamily, 12);

            using (var gr = Graphics.FromImage(chart))
            {
                gr.CompositingQuality = CompositingQuality.HighSpeed;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.CompositingMode = CompositingMode.SourceOver;
                for (int personIndex = 0; personIndex < _persons.Count(); personIndex++)
                {
                    var person = _persons.ToArray()[personIndex];
                    var leftX = (personIndex % 2) * (CIRCLE_SIZE + PADDING_LINE);
                    var topY = (personIndex / 2) * (CIRCLE_SIZE + TEXT_ROW_HEIGHT + PADDING_LINE);
                    var centerPtX = leftX + rectSize / 2;       // center of circle
                    var centerPtY = topY + TEXT_ROW_HEIGHT + (rectSize / 2);   // center of circle
                    gr.FillRectangle(almostWhiteBrush, leftX, topY, rectSize, rectSize + TEXT_ROW_HEIGHT);
                    gr.DrawString(person.Name, courierFont, Brushes.Black, leftX, topY);
                    gr.FillEllipse(anotherBlueBrush, leftX, topY + TEXT_ROW_HEIGHT, rectSize, rectSize);
                    gr.FillEllipse(Brushes.LightYellow, centerPtX - outerOuterDiameter / 2, centerPtY - outerOuterDiameter / 2,
                        outerOuterDiameter, outerOuterDiameter);
                    gr.DrawEllipse(greyPen, centerPtX - outerOuterDiameter / 2, centerPtY - outerOuterDiameter / 2,
                        outerOuterDiameter, outerOuterDiameter);
                    gr.FillEllipse(Brushes.White, centerPtX - diameter / 2, centerPtY - diameter / 2, diameter, diameter);
                    gr.DrawEllipse(greyPen, centerPtX - diameter / 2, centerPtY - diameter / 2, diameter, diameter);
                    var ascPos = _houses.First();
                    for (int i = 0; i < 12; ++i)
                    {
                        var angle = i * 30 + (int)ascPos % 30;
                        gr.FillPie(i % 2 == 0 ? anotherBlueBrush : Brushes.White,
                            centerPtX - diameter / 2 + 1, centerPtY - diameter / 2 + 1, diameter - 2, diameter - 2,
                            angle, 30);
                    }
                    var innerChartDiameter = diameter - 2 * innerDiameterOffset;
                    var innerChartDiameter2 = diameter - 2 * innerDiameterOffset2;
                    gr.FillEllipse(Brushes.PaleGreen, centerPtX - innerChartDiameter2 / 2, centerPtY - innerChartDiameter2 / 2, innerChartDiameter2, innerChartDiameter2);
                    gr.DrawEllipse(greyPen, centerPtX - innerChartDiameter2 / 2, centerPtY - innerChartDiameter2 / 2, innerChartDiameter2, innerChartDiameter2);
                    gr.FillEllipse(Brushes.White, centerPtX - innerChartDiameter / 2, centerPtY - innerChartDiameter / 2, innerChartDiameter, innerChartDiameter);
                    gr.DrawEllipse(greyPen, centerPtX - innerChartDiameter / 2, centerPtY - innerChartDiameter / 2, innerChartDiameter, innerChartDiameter);
                    var arrowedLineRadius = radius - innerDiameterOffset;
                    gr.DrawLine(greyPen, centerPtX - arrowedLineRadius, centerPtY, centerPtX - radius, centerPtY);
                    gr.DrawLine(greyPen, centerPtX - radius, centerPtY, centerPtX - radius + 12, centerPtY - (int)(12 * Math.Sin(Math.PI / 12)));
                    gr.DrawLine(greyPen, centerPtX - radius, centerPtY, centerPtX - radius + 12, centerPtY + (int)(12 * Math.Sin(Math.PI / 12)));
                     // draw the near-vertical line for the MC
                    var disMcAsc = ascPos - _houses[9];
                    if (disMcAsc < 0) {
                        disMcAsc += 360;
                    }
                    var value = 90 - disMcAsc;
                    var angle1 = 65 - value;
                    var angle2 = 65 + value;
                    var x1 = -(radius - innerDiameterOffset) * Math.Cos(Math.PI * disMcAsc / 180);
                    var y1 = -(radius - innerDiameterOffset) * Math.Sin(Math.PI * disMcAsc / 180);
                    var x2 = -radius * Math.Cos(Math.PI * disMcAsc / 180);
                    var y2 = -radius * Math.Sin(Math.PI * disMcAsc / 180);
                    gr.DrawLine(greyPen, centerPtX + (int)x1, centerPtY + (int)y1, centerPtX + (int)x2, centerPtY + (int)y2);
                    x1 = x2 + 15 * Math.Cos(Math.PI * angle1 / 180);
                    y1 = y2 + 15 * Math.Sin(Math.PI * angle1 / 180);
                    gr.DrawLine(greyPen, centerPtX + (int)x1, centerPtY + (int)y1, centerPtX + (int)x2, centerPtY + (int)y2);
                    x1 = x2 - 15 * Math.Cos(Math.PI * angle2 / 180);
                    y1 = y2 + 15 * Math.Sin(Math.PI * angle2 / 180);
                    gr.DrawLine(greyPen, centerPtX + (int)x1, centerPtY + (int)y1, centerPtX + (int)x2, centerPtY + (int)y2);
                    for (int i = 0; i < 12; ++i)
                    {
                        var angle = i * 30 + (15 - ascPos % 30) + 1;
                        var signPos = (int)ascPos / 30 + i;
                        if (signPos > 11) signPos -= 12;
                        Brush brushToUse;
                        switch (signPos % 4)
                        {
                            case 0:
                                brushToUse = Brushes.Red; break;
                            case 1:
                                brushToUse = anotherGreenBrush; break;
                            case 2:
                                brushToUse = Brushes.Orange; break;
                            default:
                                brushToUse = Brushes.Blue; break;
                        }
                        var (x, y) = displayAstroPoint(angle, middleRadius);
                        gr.DrawString(((char)SIGN_GLYPHS[signPos]).ToString(), astroFont, brushToUse, centerPtX + x, centerPtY + y);
                        if (i > 0 && i != 9)
                        {
                            angle = ascPos - _houses[i];
                            x1 = -radius * Math.Cos(Math.PI * angle / 180);
                            y1 = -radius * Math.Sin(Math.PI * angle / 180);
                            x2 = -(radius - innerDiameterOffset) * Math.Cos(Math.PI * angle / 180);
                            y2 = -(radius - innerDiameterOffset) * Math.Sin(Math.PI * angle / 180);
                            gr.DrawLine(greyPen, centerPtX + (int)x1, centerPtY + (int)y1, centerPtX + (int)x2, centerPtY + (int)y2);
                        }
                        angle = i * 30 + (int)ascPos % 30;
                        x1 = -(CIRCLE_SIZE / 2) * Math.Cos(Math.PI * angle / 180);
                        y1 = -(CIRCLE_SIZE / 2) * Math.Sin(Math.PI * angle / 180);
                        x2 = -(radius + outerDiameterDistance) * Math.Cos(Math.PI * angle / 180);
                        y2 = -(radius + outerDiameterDistance) * Math.Sin(Math.PI * angle / 180);
                        gr.DrawLine(greyPen, centerPtX + (int)x1, centerPtY + (int)y1, centerPtX + (int)x2, centerPtY + (int)y2);
                    }
                    for (int pl = person.natPlanets.Count() - 1; pl >= 0; pl--)
                    {
                        var angle = person.natPlanets.ToArray()[pl] - ascPos;
                        var (x, y) = displayAstroPoint(angle, radius - distFromDiameter1);
                        gr.DrawString(((char)PLANET_GLYPHS[pl]).ToString(), astroFont, plaNatBrush, centerPtX + x, centerPtY + y);
                    }
                    for (int pl = _transits.Count() - 1; pl >= -2; --pl)
                    {
                        var thePos = pl < 0 ? pl == -1 ? _houses[12]: ascPos : _transits[pl];
                        if (pl >= -1)
                        {
                            var angle = thePos - ascPos;
                            var (x, y) = displayAstroPoint(angle, radius - distFromDiameter1);
                            gr.DrawString(((char)PLANET_GLYPHS[pl < 0 ? 10 : pl]).ToString(), astroFont, plaTransBrush, centerPtX + x, centerPtY + y);
                        }
                        for (int nat =  _transits.Count() + person.natPlanets.Count() - 1; nat > pl; --nat)
                        {
                            angle1 = ascPos - thePos;
                            var otherPos = nat >= _transits.Count() ? person.natPlanets.ToArray()[nat - _transits.Count()] :
                                (nat < 0 ? _houses[12] : _transits[nat]);
                            angle2 = ascPos - otherPos;
                            var distance = thePos - otherPos;
                            if (distance < 0) distance += 360;
                            if ((distance > 57 && distance < 63) || (distance > 87 && distance < 93) ||
                                (distance > 117 && distance < 123) || (distance > 177 && distance < 183) ||
                                (distance > 237 && distance < 243) || (distance > 267 && distance < 273) ||
                                (distance > 297 && distance < 303))
                            {
                                x1 = -(radius - innerDiameterOffset) * Math.Cos(Math.PI * angle1 / 180);
                                y1 = -(radius - innerDiameterOffset) * Math.Sin(Math.PI * angle1 / 180);
                                x2 = -(radius - innerDiameterOffset) * Math.Cos(Math.PI * angle2 / 180);
                                y2 = -(radius - innerDiameterOffset) * Math.Sin(Math.PI * angle2 / 180);
                                var penToUse = greenPen;
                                if ((distance > 87 && distance < 93) || (distance > 177 && distance < 183) ||
                                    (distance > 267 && distance < 273))
                                {
                                    if (nat >= _transits.Count())
                                        penToUse = bluePen;
                                    else penToUse = redPen;
                                }
                                if (pl < 2 && penToUse == bluePen && distance % 30 < 2)
                                {
                                    if (distance % 30 < 1) penToUse.Width = 4;
                                    else penToUse.Width = 2;
                                }
                                gr.DrawLine(penToUse, centerPtX + (int)x1, centerPtY + (int)y1, centerPtX + (int)x2, centerPtY + (int)y2);
                            }
                        }
                    }
                    chart.Save(Path.Combine("Public", "images", result), ImageFormat.Png);
                }
            }
            return result;
        }
        public (int, int) displayAstroPoint(double angle, int radii)
        {
            var result1 = -8 - (int)(radii * Math.Cos(Math.PI * angle / 180));
            var result2 = -7 + (int)(radii * Math.Sin(Math.PI * angle / 180));

            return (result1, result2);
        }
    }
    public class PersonDetailsForVisualizer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<double> natPlanets { get; set; }
    }
}
