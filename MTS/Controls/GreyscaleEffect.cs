using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Effects;
using System.Windows;
using System.Windows.Media;
using System.IO;

namespace MTS.Controls
{
    public class GreyscaleEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(GreyscaleEffect), 0);

        private static PixelShader pixelShader;

        static GreyscaleEffect()
        {
            pixelShader = new PixelShader();

            string appPath = Properties.Settings.Default.AppDir;
            pixelShader.UriSource = new Uri(Path.Combine(appPath, "Greyscale.ps"));
                //Global.MakePackUri("Greyscale.ps");
            pixelShader.Freeze();
        }

        public GreyscaleEffect()
        {
            this.PixelShader = pixelShader;
            UpdateShaderValue(InputProperty);
        }

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }
    }

}
