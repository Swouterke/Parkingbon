using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ParkingBon
{
    /// <summary>
    /// Interaction logic for Afdrukvoorbeeld.xaml
    /// </summary>
    public partial class Afdrukvoorbeeld : Window
    {
        public Afdrukvoorbeeld()
        {
            InitializeComponent();
        }
        public IDocumentPaginatorSource AfdrukDocument
        {
            get { return printPreview.Document; }
            set { printPreview.Document = value; }
        }
    }
}
