using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Contactos.View
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        public Page2()
        {
            InitializeComponent();
        }
    }

    public class ConversorImagem : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String caminho = System.Environment.CurrentDirectory;
            caminho = caminho.Substring(0, caminho.IndexOf("bin")) + @"Images\";

            if(value!=null && !String.IsNullOrEmpty(value.ToString()))
            {
                caminho = caminho + value;
            }
            else
            {
                caminho = caminho + "noFoto.jpg";
            }

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.CreateOptions = BitmapCreateOptions.DelayCreation;
            img.UriSource = new Uri(caminho, UriKind.Absolute);
            img.EndInit();

            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
