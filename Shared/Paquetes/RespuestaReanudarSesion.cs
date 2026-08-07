using MessagePack;

namespace Shared.Paquetes
{
    [MessagePackObject]
    public class PaqueteRespuestaReanudarSesion : IPaquete
    { 
        [Key(0)]
        public bool Exitoso {get; set;}
        [Key(1)]
        public string? Token {get; set;}= "";
        [Key(2)]
        public string? NombreUsuario { get; set; }
        [Key(3)]
        public int? IdUsuario { get; set; }
        [Key(4)]
        public string? MensajeError { get; set;}
        [IgnoreMember]
        public TipoPaquete Tipo => TipoPaquete.RespuestaReanudarSesion;
    }    
}
