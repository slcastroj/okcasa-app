﻿using RestSharp;
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
using Solicitud = Aplicacion.Models.Solicitud;
using Inspeccion = Aplicacion.Models.Inspeccion;

namespace Aplicacion.Views
{
    /// <summary>
    /// Lógica de interacción para GestionUsuario.xaml
    /// </summary>
    public partial class GestionInspeccion : Page
    {
        public RestClient Client { get; }
        public GestionInspeccion(String idsolicitud = null)
        {
            InitializeComponent();
            Client = new RestClient(UrlUtils.BaseUrl);
            UpdateGrid();
            TxtPostSolicitud.Text = idsolicitud;
        }

        public void UpdateGrid()
        {
            var rq = new RestRequest("inspeccion", Method.GET);
            var rs = Client.Execute<List<Inspeccion.Get>>(rq);

            if (!rs.IsSuccessful)
            {
                MessageBox.Show("No se pudieron cargar los datos");
            }
            else
            {
                GridInspeccion.ItemsSource = rs.Data;
            }
        }

        private void BtnBack_Click(Object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Button_Click(Object sender, RoutedEventArgs e)
        {
            // Validaciones
            var idsolicitud = TxtPostSolicitud.Text;
            var monto = TxtPostMonto.Text;
            var montoint = -1;
            var fecha = PckPostFecha.SelectedDate;
            {
                if (String.IsNullOrWhiteSpace(idsolicitud))
                {
                    MessageBox.Show("Ingrese solicitud");
                    return;
                }

                if (String.IsNullOrWhiteSpace(monto))
                {
                    MessageBox.Show("Ingrese monto");
                    return;
                }

                if (!Int32.TryParse(monto, out montoint) || montoint < 0)
                {
                    MessageBox.Show("Monto debe ser un número positivo");
                    return;
                }

                if(fecha is null)
                {
                    MessageBox.Show("Ingrese fecha");
                    return;
                }
            }

            var rq = new RestRequest("inspeccion", Method.POST)
                .AddJsonBody(new
            {
                fecha_visita = fecha,
                observaciones = TxtPostObservaciones.Text,
                monto,
                id_solicitud = idsolicitud
            });

            var rs = Client.Execute(rq);
            if (rs.IsSuccessful)
            {
                MessageBox.Show("Registro actualizado");
                UpdateGrid();
            }
            else
            {
                MessageBox.Show($"Error {rs.StatusCode}");
            }
        }

        private void Button_Click_1(Object sender, RoutedEventArgs e)
        {
            // Validaciones
            var inspeccion = GridInspeccion.SelectedItem as Inspeccion.Get;
            var monto = TxtPutMonto.Text;
            var fecha = PckPutFecha.SelectedDate;
            var observaciones = TxtPutObservaciones.Text;
            {
                if(inspeccion is null)
                {
                    MessageBox.Show("Seleccione una fila");
                    return;
                }

                if(String.IsNullOrWhiteSpace(monto) && fecha is null && String.IsNullOrWhiteSpace(observaciones))
                {
                    MessageBox.Show("Ingrese algún campo a modificar");
                    return;
                }

                if (String.IsNullOrWhiteSpace(monto))
                {
                    monto = inspeccion.Monto.ToString();
                }

                if (fecha is null)
                {
                    fecha = inspeccion.Fecha_visita;
                }

                if(String.IsNullOrWhiteSpace(observaciones))
                {
                    observaciones = inspeccion.Observaciones;
                }
            }

            var rq = new RestRequest($"inspeccion/{inspeccion.Id_inspeccion}", Method.PUT)
                .AddJsonBody(new
                {
                    fecha_visita = fecha,
                    observaciones,
                    monto = monto,
                });

            var rs = Client.Execute(rq);
            if (rs.IsSuccessful)
            {
                MessageBox.Show("Registro actualizado");
                UpdateGrid();
            }
            else
            {
                MessageBox.Show($"Error {rs.StatusCode}");
            }
        }

        private void Button_Click_2(Object sender, RoutedEventArgs e)
        {
            var s = GridInspeccion.SelectedItem as Inspeccion.Get;
            // Validaciones
            {
                if (s is null)
                {
                    MessageBox.Show("Seleccione una fila");
                    return;
                }
            }

            var rq = new RestRequest($"inspeccion/{s.Id_inspeccion}", Method.DELETE);
            var rs = Client.Execute(rq);

            if (rs.IsSuccessful)
            {
                MessageBox.Show("Registro actualizado");
                UpdateGrid();
            }
            else
            {
                MessageBox.Show($"Error {rs.StatusCode}");
            }
        }
    }
}
