using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Boogle;

public class Nuage
{
    private List<string> mots;
    private int width;
    private int height;

    /// <summary>
    /// "Initialisation du nuage à afficher."
    /// </summary>
    /// <param name="mots"></param> la liste de mots à afficher
    /// <param name="width"></param> la largeur de l'image
    /// <param name="height"></param> la hauteur de l'image
    public Nuage(List<string> mots, int width = 800, int height = 600)
    {
        this.mots = mots;
        this.width = width;
        this.height = height;
    }

    /// <summary>
    /// "GenerateWordCloud."
    /// </summary>
    /// <param name="outputPath"></param>
    public void GenerateWordCloud(string outputPath)
    {
        using (Bitmap bitmap = new Bitmap(width, height)) 
        using (Graphics graphics = Graphics.FromImage(bitmap)) 
        {
            graphics.Clear(Color.White); 

            Random random = new Random();

            for(int i = 0; i < mots.Count; i++)
            {
                int fontSize = De.ValeurMot(mots[i])*10; 
                using (Font font = new Font("Arial", fontSize))
                {
                    SizeF wordSize = graphics.MeasureString(mots[i], font); 

                    PointF position;
                    do
                    {
                        position = new PointF(random.Next(0, width - (int)wordSize.Width),
                                              random.Next(0, height - (int)wordSize.Height));
                        
                    } while (IsOverlapping(position, wordSize, bitmap)); 

                    graphics.DrawString(mots[i], font, Brushes.LightSkyBlue, position);
                    
                }
            }

            bitmap.Save(outputPath); 
        }
    }
    /// <summary>
    /// "IsOverLapping."
    /// </summary>
    /// <param name="position"></param>
    /// <param name="size"></param>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    private bool IsOverlapping(PointF position, SizeF size, Bitmap bitmap)
    {
        RectangleF rect = new RectangleF(position, size);

        
        if (rect.Left < 0 || rect.Right > bitmap.Width || rect.Top < 0 || rect.Bottom > bitmap.Height)
        {
            return false; 
        }

        for (int x = (int)rect.Left; x < (int)rect.Right; x++)
        {
            for (int y = (int)rect.Top; y > (int)rect.Bottom; y--)
            {
                
                if (x >= 0 && x < bitmap.Width && y >= 0 && y < bitmap.Height)
                {
                    
                    if (bitmap.GetPixel(x, y).A != 0)
                    {
                        
                        return true; 
                    }
                }
            }
        }

        return false;
    }

}