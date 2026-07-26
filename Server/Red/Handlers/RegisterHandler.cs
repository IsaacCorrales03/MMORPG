using Server.Servicios;
using Shared.Paquetes;

namespace Server.Red.Handlers
{
    public class RegisterHandler
    {
        private static readonly ServicioAutenticacion _servicio = new ServicioAutenticacion();

        public static async Task<PaqueteRespuestaRegistro> Manejar(PaquetePeticionRegistro paquete)
        {
            PaqueteRespuestaRegistro respuesta = new PaqueteRespuestaRegistro();

            bool requisito_clave_al_menos_un_numero = paquete.Clave.Any(char.IsDigit);
            bool requisito_clave_longitud = paquete.Clave.Length >= 8;
            bool usuarioVacio = string.IsNullOrWhiteSpace(paquete.Usuario);
            bool claveVacia = string.IsNullOrWhiteSpace(paquete.Clave);
            bool requisito_usuario_longitud = paquete.Usuario.Length is >= 3 and <= 20;
            bool requisito_usuario_caracteres_validos = paquete.Usuario.All(c => char.IsLetterOrDigit(c) || c == '_');
            bool requisito_clave_mayuscula = paquete.Clave.Any(char.IsUpper);
            bool requisito_clave_minuscula = paquete.Clave.Any(char.IsLower);
            bool requisito_clave_longitud_maxima = paquete.Clave.Length <= 64;
            respuesta.Exitoso = true;
            switch (true)
            {
                case var _ when usuarioVacio:
                    respuesta.Exitoso = false;
                    respuesta.MensajeError = "El usuario es obligatorio.";
                    break;

                case var _ when claveVacia:
                    respuesta.Exitoso = false;
                    respuesta.MensajeError = "La clave es obligatoria.";
                    break;

                case var _ when !requisito_usuario_longitud:
                    respuesta.Exitoso = false;
                    respuesta.MensajeError = "El usuario debe tener entre 3 y 20 caracteres.";
                    break;

                case var _ when !requisito_usuario_caracteres_validos:
                    respuesta.Exitoso = false;
                    respuesta.MensajeError = "El usuario solo puede contener letras, números y guion bajo.";
                    break;

                case var _ when !requisito_clave_longitud:
                    respuesta.Exitoso = false;
                    respuesta.MensajeError = "La clave debe tener al menos 8 caracteres.";
                    break;

                case var _ when !requisito_clave_longitud_maxima:
                    respuesta.Exitoso = false;
                    respuesta.MensajeError = "La clave no puede superar los 64 caracteres.";
                    break;

                case var _ when !requisito_clave_al_menos_un_numero:
                    respuesta.Exitoso = false;
                    respuesta.MensajeError = "La clave debe contener al menos un número.";
                    break;

                case var _ when !requisito_clave_mayuscula:
                    respuesta.Exitoso = false;
                    respuesta.MensajeError = "La clave debe contener al menos una mayúscula.";
                    break;

                case var _ when !requisito_clave_minuscula:
                    respuesta.Exitoso = false;
                    respuesta.MensajeError = "La clave debe contener al menos una minúscula.";
                    break;

                default:
                    respuesta.Exitoso = true;
                    respuesta.MensajeError = "";
                    break;
            }

            if (!respuesta.Exitoso)
                return respuesta;
            await _servicio.registrar_jugador(paquete.Email, paquete.Usuario, paquete.Clave, respuesta);
            return respuesta;
        }
    }
}
