using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PerlinNoiseGenerator
{
  public class NoiseForm : Form
  {
    // Para la ventana.
    private readonly PictureBox _box;
    private int _counter = 1;

    // Instancio la libreria externa que genera el ruido.
    private readonly FastNoiseLite _noise;

    private readonly Random _random = new Random();
    private readonly int _canvasWidth = 512;
    private readonly int _canvasHeight = 512;

    public NoiseForm()
    {
      this.Text = "Generador de ruido";

      // Creacion de objeto.
      _noise = new FastNoiseLite();

      /* EXPLICACION:
       * El ruido de Perlin es demasiado uniforme para crear terrenos, por lo que debemos hacer otra serie de
       * operacones para obtener el mapa. En primer lugar, el FractalType(FBm) activa el modo Fractal Brownian Motion.
       * Esto significa que la libreria que uso, en lugar de hacer una sola llamada para genrar ruido, va a hacer
       * varias. Este numero se define en SetFractalOcataves. Dichas octavas son, por asi decirlo, las capas del
       * ruido. SetFranctalLacunarity define cuanto aumenta la frecuencia en cada octava (a mayor frecuencia, mas
       * pequeñas y densas son las implementaciones. La frecuencia define cuanta distancia hay entre cada onda.
       * Por otro lado, FractalGain regula la amplitud de cada octava consecutiva. Es decir, las octavas de alta
       * frecuencia (micro-detalles) deben influir menos que las de baja frecuencia (estructura general).
       * 
         octava 1 (amplitud 1.0,  frecuencia 1x) → forma general del terreno
           octava 2 (amplitud 0.5,  frecuencia 2x) → colinas sobre esa forma
           octava 3 (amplitud 0.25, frecuencia 4x) → irregularidades medianas
           octava 4 (amplitud 0.125,frecuencia 8x) → detalles pequeños
           octava 5 (amplitud 0.06, frecuencia 16x) → micro-detalles
           octava 6 (amplitud 0.03, frecuencia 32x) → textura fina
       */

      _noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
      _noise.SetFractalType(FastNoiseLite.FractalType.FBm);
      _noise.SetFractalOctaves(6);
      _noise.SetFractalLacunarity(2.0f);
      _noise.SetFractalGain(0.5f);

      // Boirlerplate de la ventana.
      _box = new PictureBox { Dock = DockStyle.Fill };
      Controls.Add(_box);
      ClientSize = new Size(_canvasWidth, _canvasHeight);

      // Pintar en la ventana.
      GenerateBitmap();

      // Al pulsar una tecla, se genera un nuevo BitMap.
      KeyDown += (s, e) => GenerateBitmap();
    }

    private void GenerateBitmap()
    {
      // Semilla para el ruido.
      _noise.SetSeed(_random.Next());

      // Dimensiones.
      int width = 512, height = 512;
      var bitmap = new Bitmap(width, height);
      List<double> vals = new List<double>();

      // Por cada pixel del bitmap.
      for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
          // Generamos un valor de ruido.
          double value = (_noise.GetNoise(x, y) + 1.0) / 2.0;
          // Lo elevamos.
          value = Math.Pow(value, 2.0);
          // En funcion del valor, pintamos de un color u otro.
          Color c = GetCaveColor(value);

          vals.Add(value);


          // Pintamos el color "c" en las coordenadas x e y.
          bitmap.SetPixel(x, y, c);
        }

      string fileName = $"terrain_{_counter}.csv";
      _counter++;

      string baseDir = AppContext.BaseDirectory;
      string fullPath = Path.Combine(baseDir, "Exports" , fileName);
      
      this.ExportToCSV(vals, 512, 512, fullPath);

      // Guardo el bitmap como un png.
      // Le asignamos el bitmap nuevo a la ventana.
      //bitmap.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);

      _box.Image = bitmap;
    }

    // Prototipo cueva:
    private Color GetCaveColor(double value)
    {
      if (value < 0.4) return Color.White;
      if (value < 0.8) return Color.Black;
      return Color.White;
    }

    // Para mapas complejos.
    private Color GetTerraincolor(double value)
    {
      if (value < 0.3) return Color.Blue;
      if (value < 0.4) return Color.SandyBrown;
      if (value < 0.6) return Color.Green;
      if (value < 0.8) return Color.Gray;
      return Color.White;
    }

    private void ExportToCSV(List<double> values, int width, int height, string fullPath)
    {
      double[] vals = values.ToArray();
      var rows = new List<string>();
      int index = 0;

      for (int y = 0; y < height; y++)
      {
        var cells = new List<int>();
        for (int x = 0; x < width; x++)
        {
          cells.Add(vals[index] > 0.5 ? 1 : 0);
          index++;
        }
        rows.Add(string.Join(",", cells));
      }

      File.WriteAllLines(fullPath, rows);
    }
  }
}