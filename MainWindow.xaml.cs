using ProyectoFinal2;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Configuration;


namespace ProyectoFinal2
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<MovimientoCaja> movimientosCaja = new ObservableCollection<MovimientoCaja>();
        private ObservableCollection<Producto> inventario = new ObservableCollection<Producto>();

        public MainWindow()
        {
            InitializeComponent();

            dgInventario.ItemsSource = inventario;
            dgMovimientos.ItemsSource = movimientosCaja;

            cmbTipoTransaccion.SelectionChanged += cmbTipoTransaccion_SelectionChanged;


        }

        private void GuardarYCerrar_Click(object sender, RoutedEventArgs e)
        {
            ConexionBD.CloseConnection();
            this.Close();
        }


        private void cmbTipoTransaccion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = cmbTipoTransaccion.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string tipoTransaccion = selectedItem.Content.ToString();
                bool isCompra = tipoTransaccion == "Compra";
                bool isVenta = tipoTransaccion == "Venta";

                lblCodigo.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                txtCodigo.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                lblDescripcion.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                txtDescripcion.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                lblCantidad.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                txtCantidad.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                lblPrecioIngreso.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                txtPrecioIngreso.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                lblPrecioVenta.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                txtPrecioVenta.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                lblFechaIngreso.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                dpFechaIngreso.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;

                lblCantidadVenta.Visibility = isVenta ? Visibility.Visible : Visibility.Collapsed;
                txtCantidadVenta.Visibility = isVenta ? Visibility.Visible : Visibility.Collapsed;
                lblDescripcionVenta.Visibility = isVenta ? Visibility.Visible : Visibility.Collapsed;
                txtDescripcionVenta.Visibility = isVenta ? Visibility.Visible : Visibility.Collapsed;
                lblMontoVenta.Visibility = isVenta ? Visibility.Visible : Visibility.Collapsed;
                txtMontoVenta.Visibility = isVenta ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void txtBuscarProducto_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscarProducto.Text))
            {
                dgInventario.ItemsSource = inventario;
            }
            else
            {
                string textoBusqueda = txtBuscarProducto.Text.ToLower();
                var productosEncontrados = inventario.Where(p => p.Descripcion.ToLower().Contains(textoBusqueda) || p.Codigo.ToLower() == textoBusqueda).ToList();
                dgInventario.ItemsSource = productosEncontrados;
            }
        }

        private void ActualizarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (dgInventario.SelectedItem != null)
            {
                Producto productoSeleccionado = dgInventario.SelectedItem as Producto;
            }
            else
            {
                MessageBox.Show("Seleccione un producto para actualizar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarProducto_Click(object sender, RoutedEventArgs e)
        {
            Producto productoSeleccionado = dgInventario.SelectedItem as Producto;
            if (productoSeleccionado != null)
            {
                MessageBoxResult result = MessageBox.Show("¿Está seguro de que desea eliminar este producto?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    inventario.Remove(productoSeleccionado);
                    MessageBox.Show("Producto eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Seleccione un producto para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BuscarProducto_Click(object sender, RoutedEventArgs e)
        {
            string textoBusqueda = txtBuscarProducto.Text.ToLower();
            var productosEncontrados = inventario.Where(p => p.Descripcion.ToLower().Contains(textoBusqueda) || p.Codigo.ToLower() == textoBusqueda).ToList();
            dgInventario.ItemsSource = productosEncontrados;
        }

        private void GenerarReportes_Click(object sender, RoutedEventArgs e)
{
    int totalProductos = inventario.Count;
    int totalProductosEnStock = inventario.Where(p => p.Cantidad > 0).Count();
    decimal valorTotalInventario = inventario.Sum(p => p.PrecioIngreso * p.Cantidad);

    string mensaje = $"Total de productos: {totalProductos}\n" +
                     $"Productos en stock: {totalProductosEnStock}\n" +
                     $"Valor total del inventario: ${valorTotalInventario:F2}";

    MessageBox.Show(mensaje, "Estadísticas del Inventario", MessageBoxButton.OK, MessageBoxImage.Information);
}

        private void RegistrarTransaccion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tipo = (cmbTipoTransaccion.SelectedItem as ComboBoxItem)?.Content.ToString();
                string connectionString = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;

                    if (tipo == "Compra")
                    {
                        string codigo = txtCodigo.Text;
                        string descripcion = txtDescripcion.Text;

                        if (!int.TryParse(txtCantidad.Text, out int cantidad))
                        {
                            MessageBox.Show("Por favor, ingrese una cantidad válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (!decimal.TryParse(txtPrecioIngreso.Text, out decimal precioIngreso))
                        {
                            MessageBox.Show("Por favor, ingrese un precio de ingreso válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (!decimal.TryParse(txtPrecioVenta.Text, out decimal precioVenta))
                        {
                            MessageBox.Show("Por favor, ingrese un precio de venta válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        DateTime fechaIngreso = dpFechaIngreso.SelectedDate.HasValue ? dpFechaIngreso.SelectedDate.Value : DateTime.Now;

                        bool productoExistente = inventario.Any(p => p.Codigo == codigo);
                        if (productoExistente)
                        {
                            command.CommandText = "UPDATE Productos SET Cantidad = Cantidad + @Cantidad, PrecioIngreso = @PrecioIngreso WHERE Codigo = @Codigo";
                        }
                        else
                        {
                            command.CommandText = "INSERT INTO Productos (Codigo, Descripcion, Cantidad, PrecioIngreso, PrecioVenta, FechaIngreso) VALUES (@Codigo, @Descripcion, @Cantidad, @PrecioIngreso, @PrecioVenta, @FechaIngreso)";
                        }

                        command.Parameters.AddWithValue("@Codigo", codigo);
                        command.Parameters.AddWithValue("@Descripcion", descripcion);
                        command.Parameters.AddWithValue("@Cantidad", cantidad);
                        command.Parameters.AddWithValue("@PrecioIngreso", precioIngreso);
                        command.Parameters.AddWithValue("@PrecioVenta", precioVenta);
                        command.Parameters.AddWithValue("@FechaIngreso", fechaIngreso);
                        command.ExecuteNonQuery();

                        if (productoExistente)
                        {
                            Producto productoEncontrado = inventario.FirstOrDefault(p => p.Codigo == codigo);
                            productoEncontrado.Cantidad += cantidad;
                        }
                        else
                        {
                            Producto nuevoProducto = new Producto
                            {
                                Codigo = codigo,
                                Descripcion = descripcion,
                                Cantidad = cantidad,
                                PrecioIngreso = precioIngreso,
                                PrecioVenta = precioVenta,
                                FechaIngreso = fechaIngreso
                            };

                            inventario.Add(nuevoProducto);
                        }


                        command.CommandText = "INSERT INTO MovimientosCaja (TipoMovimiento, Cantidad, Descripcion, Monto, Fecha) VALUES (@TipoMovimiento, @Cantidad, @Descripcion, @Monto, @Fecha)";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@TipoMovimiento", "Compra");
                        command.Parameters.AddWithValue("@Cantidad", cantidad);
                        command.Parameters.AddWithValue("@Descripcion", descripcion);
                        command.Parameters.AddWithValue("@Monto", precioIngreso);
                        command.Parameters.AddWithValue("@Fecha", fechaIngreso);
                        command.ExecuteNonQuery();

                        movimientosCaja.Add(new MovimientoCaja
                        {
                            TipoMovimiento = "Compra",
                            Cantidad = cantidad,
                            Descripcion = descripcion,
                            Monto = precioIngreso,
                            Fecha = fechaIngreso
                        });

                        MessageBox.Show("Compra registrada con éxito en la base de datos.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    else if (tipo == "Venta")
                    {
                        string descripcionVenta = txtDescripcionVenta.Text;

                        if (!int.TryParse(txtCantidadVenta.Text, out int cantidadVenta))
                        {
                            MessageBox.Show("Por favor, ingrese una cantidad válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        command.CommandText = "SELECT * FROM Productos WHERE Descripcion = @Descripcion";
                        command.Parameters.AddWithValue("@Descripcion", descripcionVenta);
                        SqlDataReader reader = command.ExecuteReader();

                        if (!reader.HasRows)
                        {
                            MessageBox.Show("Producto no encontrado en el inventario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            reader.Close();
                            return;
                        }

                        reader.Read();
                        int cantidadActual = (int)reader["Cantidad"];
                        decimal precioVenta = (decimal)reader["PrecioVenta"];
                        reader.Close();

                        if (cantidadActual < cantidadVenta)
                        {
                            MessageBox.Show("No hay suficiente cantidad en el inventario para realizar la venta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        decimal montoVenta = cantidadVenta * precioVenta;
                        txtMontoVenta.Text = montoVenta.ToString("F2");

                        command.CommandText = "UPDATE Productos SET Cantidad = Cantidad - @CantidadVenta WHERE Descripcion = @Descripcion";
                        command.Parameters.AddWithValue("@CantidadVenta", cantidadVenta);
                        command.ExecuteNonQuery();

                        Producto producto = inventario.FirstOrDefault(p => p.Descripcion == descripcionVenta);
                        if (producto != null)
                        {
                            producto.Cantidad -= cantidadVenta;
                        }

                        movimientosCaja.Add(new MovimientoCaja
                        {
                            TipoMovimiento = "Venta",
                            Cantidad = cantidadVenta,
                            Descripcion = descripcionVenta,
                            Monto = montoVenta,
                            Fecha = DateTime.Now
                        });

                        MessageBox.Show("Venta registrada con éxito en la base de datos.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar transacción: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void AbrirCronograma_Click(object sender, RoutedEventArgs e)
        {
            CronogramaWindow cronogramaWindow = new CronogramaWindow();
            cronogramaWindow.Show();
        }

        private void AbrirGestionClientes_Click(object sender, RoutedEventArgs e)
        {
            CustomerManagementWindow customerManagementWindow = new CustomerManagementWindow();
            customerManagementWindow.Show();
        }

        private void AbrirAnalisisVentas_Click(object sender, RoutedEventArgs e)
        {
            SalesAnalysisWindow salesAnalysisWindow = new SalesAnalysisWindow();
            salesAnalysisWindow.Show();
        }

        private void CargarInventarioDesdeBaseDeDatos()
        {
            using (SqlConnection connection = ConexionBD.GetConnection())
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM Productos", connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        inventario.Add(new Producto
                        {
                            Codigo = row["Codigo"].ToString(),
                            Descripcion = row["Descripcion"].ToString(),
                            Cantidad = (int)row["Cantidad"],
                            PrecioIngreso = (decimal)row["PrecioIngreso"],
                            PrecioVenta = (decimal)row["PrecioVenta"],
                            FechaIngreso = (DateTime)row["FechaIngreso"]
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar inventario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
