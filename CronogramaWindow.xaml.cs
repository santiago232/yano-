using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ProyectoFinal2
{
    public partial class CronogramaWindow : Window
    {
        private ObservableCollection<Evento> eventos = new ObservableCollection<Evento>();

        public CronogramaWindow()
        {
            InitializeComponent();
            dgEventos.ItemsSource = eventos;
        }

        private void AgregarEvento_Click(object sender, RoutedEventArgs e)
        {
            DateTime fechaEvento = dpFechaEvento.SelectedDate ?? DateTime.Now;
            string descripcionEvento = txtDescripcionEvento.Text;

            eventos.Add(new Evento { Descripcion = descripcionEvento, Fecha = fechaEvento });

            txtDescripcionEvento.Clear();
            dpFechaEvento.SelectedDate = null;
        }

        private void EliminarEvento_Click(object sender, RoutedEventArgs e)
        {
            Evento eventoSeleccionado = dgEventos.SelectedItem as Evento;
            if (eventoSeleccionado != null)
            {
                eventos.Remove(eventoSeleccionado);
            }
        }
    }

    public class Evento
    {
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
