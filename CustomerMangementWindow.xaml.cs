using System.Collections.ObjectModel;
using System.Windows;

namespace ProyectoFinal2
{
    public partial class CustomerManagementWindow : Window
    {
        public ObservableCollection<Cliente> Clientes { get; set; }

        public CustomerManagementWindow()
        {
            InitializeComponent();
            Clientes = new ObservableCollection<Cliente>();
            dgClientes.ItemsSource = Clientes;
        }

        private void AgregarCliente_Click(object sender, RoutedEventArgs e)
        {
            // Código para agregar un cliente
        }

        private void EditarCliente_Click(object sender, RoutedEventArgs e)
        {
            // Código para editar un cliente
        }

        private void EliminarCliente_Click(object sender, RoutedEventArgs e)
        {
            // Código para eliminar un cliente
        }
    }

    public class Cliente
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }
}
