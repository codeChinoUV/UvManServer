using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
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
using GameChatService;
using GameChatService.Servicio;

namespace Pacman
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceHost cuentaHost;
        private ServiceHost chatHost;
        private int  CONEXIONES_MAXIMAS_PERMITIDAS = 5;
        private int TIEMPO_ESPERA_INACTIVIDAD = 30;
        private int TIEMPO_ESPERA_PAQUETE = 2;
        private int LLAMADAS_SIMULTANEAS_PERMITIDAS = 10;
        private Boolean CONEXION_CONFIABLE = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BIniciar_Click(object sender, RoutedEventArgs e)
        {
            bIniciar.IsEnabled = false;
            //String direccion = "localhost";
            //String puertoTcp = "8192";
            //String direccionChat = "/CuentaService";
            //String direccionMetadatos = "/WPFHost/mex";
            //String puertoHttp = "8182";
            //Uri direccionTpc = new Uri("net.tcp://" + direccion + ":" + puertoTcp + direccionChat);
            //Uri direccionHttp = new Uri("http://" + direccion + ":" + puertoHttp + direccionChat);

            //Uri[] baseAdresses = { direccionTpc, direccionHttp };
            Uri[] uris = new Uri[2];
            String direccion = "net.tcp://localhost:8192/CuentaService";
            String direccionHttp = "http://localhost:8182/CuentaService";
            uris[0] = new Uri(direccion);
            uris[1] = new Uri(direccionHttp);
            IChatService servicio = new ChatService();
            cuentaHost = new ServiceHost(typeof(CuentaService.Servicio.CuentaService));
            chatHost = new ServiceHost(servicio, uris);
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            chatHost.AddServiceEndpoint(typeof(IChatService), binding, String.Empty);
            //NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);

            //tcpBinding.MaxConnections = CONEXIONES_MAXIMAS_PERMITIDAS;
            //tcpBinding.CloseTimeout = new TimeSpan(0, 0, TIEMPO_ESPERA_INACTIVIDAD);
            //tcpBinding.ReceiveTimeout = new TimeSpan(0, 0, TIEMPO_ESPERA_PAQUETE);
            //tcpBinding.ReliableSession.Enabled = CONEXION_CONFIABLE;
            //tcpBinding.ReliableSession.InactivityTimeout = new TimeSpan(0, 0, 10);

            //ServiceThrottlingBehavior comportamientoDeconcurrenciaDelServicio = chatHost.Description.Behaviors.Find<ServiceThrottlingBehavior>();

            //if (comportamientoDeconcurrenciaDelServicio == null)
            //{
            //    comportamientoDeconcurrenciaDelServicio = new ServiceThrottlingBehavior();
            //    comportamientoDeconcurrenciaDelServicio.MaxConcurrentCalls = LLAMADAS_SIMULTANEAS_PERMITIDAS;
            //    comportamientoDeconcurrenciaDelServicio.MaxConcurrentSessions = CONEXIONES_MAXIMAS_PERMITIDAS;
            //    chatHost.Description.Behaviors.Add(comportamientoDeconcurrenciaDelServicio);
            //}

            //chatHost.AddServiceEndpoint(typeof(GameChatService.IChatService), tcpBinding, "tcp");

            ServiceMetadataBehavior comportamientoDelaMetadata = new ServiceMetadataBehavior();
            comportamientoDelaMetadata.HttpGetEnabled = true;
            chatHost.Description.Behaviors.Add(comportamientoDelaMetadata);

            //chatHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "net.tcp://" + direccionHttp
            //    + ":" + puertoHttp + direccionMetadatos);

            try
            {
                cuentaHost.Closed += hostCuentaOnClosed;
                cuentaHost.Open();
                chatHost.Closed += hostChatOnClosed;
                chatHost.Open();
            }
            catch (Exception excepcion)
            {
                lEstado.Content = excepcion.Message;
            }
            finally
            {
                if (cuentaHost.State == CommunicationState.Opened && chatHost.State == CommunicationState.Opened)
                {
                    lEstado.Content = "Activo";
                    bDetener.IsEnabled = true;
                }
            }


        }

        private void hostCuentaOnClosed(Object sender, EventArgs e)
        {
            lEstado.Content += " Cerrada cuenta";
        }

        private void hostChatOnClosed(Object sender, EventArgs e)
        {
            lEstado.Content += ", Cerrada chat";
        }

        private void BDetener_Click(object sender, RoutedEventArgs e)
        {
            if (cuentaHost != null && chatHost != null )
            {
                try
                {
                    cuentaHost.Close();
                    chatHost.Close();
                }
                catch (Exception excepcion)
                {
                    lEstado.Content = excepcion.Message;
                }
                finally
                {
                    if (cuentaHost.State == CommunicationState.Closed && chatHost.State == CommunicationState.Closed)
                    {
                        lEstado.Content = "Cerrada";
                        bIniciar.IsEnabled = true;
                        bDetener.IsEnabled = false;
                    }
                }
            }
        }
    }
}
