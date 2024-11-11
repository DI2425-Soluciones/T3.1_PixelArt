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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace T3._1_PixelArt
{
    public partial class MainWindow : Window
    {
        private Brush color = Brushes.White;

        public MainWindow()
        {
            InitializeComponent();                              // Inicializa los componentes
            CrearPanel(25);                                     // Panel inicial
            CambioColor(colorPersonalizadoTextBox.Text, true);  // Color inicial
        }

        // Crea un panel de tamaño size x size utilizando elementos "Border"
        // dentro de un panel "UniformGrid".
        private void CrearPanel(int size)
        {
            pixelPanelGrid.Children.Clear(); // Limpia el panel de píxeles (controles border).
            
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Border bd = new Border();
                    bd.Style = (Style)this.Resources["BorderPixel"];

                    TextBlock tb = new TextBlock();     // Añadimos un TextBlock al borde creado
                    bd.Child = tb;                      // con el fin de que contenga un objeto
                                                        // que facilita el control de los eventos
                                                        // "MouseLeftButtonDown" y "MouseEnter".

                    pixelPanelGrid.Children.Add(bd);    // Añade al panel el píxel (borde) creado.
                }
            }
        }

        // Para comprobar si el panel está vacío para cuando
        // se cambia de tamaño y se pierde el dibujo.
        private bool isPanelEmpty()
        {
            foreach (Border b in pixelPanelGrid.Children)
            {
                if (b.Background != Brushes.White) return false; // Si hay algún píxel pintado distinto de BLANCO.
            }
            return true;
        }

        // Cambiar el tamaño del panel de píxeles.
        private void ResizePanel(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;

            int size = int.Parse(b.Tag.ToString());

            if (!isPanelEmpty())
            {
                if (MessageBox.Show("¿Seguro que quieres perder tu dibujo?","Nuevo dibujo",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    CrearPanel(size);
                }
            }
            else
            {
                CrearPanel(size);
            }
        }

        // ................ COLOR con el que trabajar.
        //
        // Gestión de los colores desde los RadioButton y TextBox.
        // Se envia al método "CambioColor" el color seleccionado no personalizado.
        private void ColorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Tag.ToString() == "Personalizado") colorPersonalizadoTextBox.IsEnabled = true;
            else CambioColor(rb.Tag.ToString(), false);
        }

        // Para deshabilitar el TextBox de color personalizado.
        private void PersonalizadoRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            colorPersonalizadoTextBox.IsEnabled = false;
            colorPersonalizadoRadioButton.Foreground = Brushes.Black;
        }

        // Gestión del color personalizado introducido por teclado.
        private void ColorPersonalizadoTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) CambioColor(((TextBox)sender).Text, true);
        }

        // Cambia el color seleccionado.
        private void CambioColor(String colorNuevo, bool isPersonalized)
        {
            BrushConverter bc = new BrushConverter();
            try
            {   // Cambia el color seleccionado.
                color = (Brush)bc.ConvertFrom(colorNuevo);

                // Cambia el color del texto del RadioButton personalizado.
                if (isPersonalized) colorPersonalizadoRadioButton.Foreground = (Brush)bc.ConvertFrom(colorNuevo);
            }
            catch (FormatException)
            {
                MessageBox.Show($"Código de color {colorNuevo} no válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ........... PARA PINTAR EN EL PANEL DE PÍXELES.
        //
        // Evento para modificar el color de los píxeles por pulsación.
        private void Border_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Border b = (Border)sender;
            b.Background = color;
        }

        // Evento para modificar el color de los píxeles por arrastre.
        private void Border_MouseEnter(object sender, RoutedEventArgs e)
        {
            Border b = (Border)sender;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                b.Background = color;
        }

        // Rellena todos los píxeles del panel con el color seleccionado.
        private void RellenarButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Border b in pixelPanelGrid.Children)
            {
                b.Background = color;
            }
        }

        // ........... PARA EL PANEL DE PÍXELES PERSONALIZADO.
        //
        private void PanelPersonalizadoTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    int size = int.Parse(panelPersonalizadoTextBox.Text);

                    if ((size < 1 || size > 100) && errorNumero.Visibility == Visibility.Collapsed)
                    {
                        errorNumero.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        errorNumero.Visibility = Visibility.Collapsed;

                        if (!isPanelEmpty())
                        {
                            if (MessageBox.Show("¿Seguro que quieres perder tu dibujo?","Nuevo dibujo",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                CrearPanel(int.Parse(panelPersonalizadoTextBox.Text));
                            }
                        }
                        else
                        {
                            CrearPanel(int.Parse(panelPersonalizadoTextBox.Text));
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show($"Número '{panelPersonalizadoTextBox.Text}' no válido.\n Introduce un número entero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PanelPersonalizadoTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            panelPersonalizadoTextBox.Foreground = Brushes.Black;
            panelPersonalizadoTextBox.Text = "";
        }

        private void PanelPersonalizadoTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            panelPersonalizadoTextBox.Foreground = Brushes.LightGray;

            if (String.IsNullOrEmpty(panelPersonalizadoTextBox.Text))
                panelPersonalizadoTextBox.Text = "ej. 60";
        }


        // Evento para añadir o quitar borde al panel de píxeles.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            if (b.ToolTip.ToString() == "Borde")
                borderPixelPanelGrid.BorderThickness = new Thickness(3);  // Añade borde.
            else borderPixelPanelGrid.BorderThickness = new Thickness(0);  // Quita borde.
        }
    }
}