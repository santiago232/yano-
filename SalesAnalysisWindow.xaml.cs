using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ProyectoFinal2
{
    public partial class SalesAnalysisWindow : Window
    {
        public ObservableCollection<Venta> Ventas { get; set; }

        public SalesAnalysisWindow()
        {
            InitializeComponent();
            Ventas = new ObservableCollection<Venta>();
            dgVentas.ItemsSource = Ventas;
        }

        private void GenerarReporte_Click(object sender, RoutedEventArgs e)
        {
            // Código para generar reporte de ventas
        }
    }

    public class Venta
    {
        public DateTime Fecha { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Monto { get; set; }
    }
}
