using System;
using System.Net.Mail;
using System.Text;
using System.Net;
using System.Diagnostics;

namespace LogicaDelNegocio.Util
{
    /// <summary>
    /// Se encarga de enivar correo electronicos
    /// </summary>
    public class ClienteCorreo
    {
        private SmtpClient Cliente = new SmtpClient();
        private const String CORREO = "UvManGame@gmail.com";
        private const String PASSWORD = "FacultadEstadisticaEInformaticaLIS5";
        private const int PUERTO = 587;
        private const String HOSTSMTP = "smtp.gmail.com";

        public ClienteCorreo()
        {
            Cliente.Port = PUERTO;
            Cliente.EnableSsl = true;
            Cliente.Host = HOSTSMTP;
            Cliente.UseDefaultCredentials = false;
            Cliente.Credentials = new NetworkCredential(CORREO, PASSWORD);
        } 

        /// <summary>
        /// Envia un correo electronico en formato HTML
        /// </summary>
        /// <param name="Destinatario">String</param>
        /// <param name="Asunto">String</param>
        /// <param name="Contenido">String</param>
        /// <returns>true si el correo se envio correctamente o false si no</returns>
        public Boolean EnviarCorreoHtml(String Destinatario, String Asunto, String Contenido)
        {
            MailMessage Mensaje = new MailMessage();
            Mensaje.To.Add(Destinatario);
            Mensaje.Subject = Asunto;
            Mensaje.SubjectEncoding = Encoding.UTF8;
            Mensaje.Body = Contenido;
            Mensaje.BodyEncoding = Encoding.UTF8;
            Mensaje.IsBodyHtml = true;
            Mensaje.From = new MailAddress(CORREO);
            try
            {
                Cliente.Send(Mensaje);
                return true;
            }
            catch (SmtpException ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Genera una cadena html que contiene el texto necesario para enviar el mensaje
        /// </summary>
        /// <param name="Code"></param>
        /// <returns>Una cadena que contiene el texto del mensaje</returns>
        public static String GenerarContenidoVerificacion(String Code)
        {
            return "<h1>Estas a un paso de jugar UvMan</h1><h2>Tu codigo de verificación es: </h2><h3>" + Code
                + "</h3>";
        }
    }
}
