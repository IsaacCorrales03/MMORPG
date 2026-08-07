    using MessagePack;

    namespace Shared.Paquetes
    {
        [MessagePackObject]
        public class PaqueteRespuestaRegistro : IPaquete
        {
            [Key(0)]
            public bool Exitoso {get; set;} = false;
            [Key(1)]
            public string MensajeError {get; set;} = "";
            [Key(2)]
            public string Token {get; set;} = "";
            [IgnoreMember]
            public TipoPaquete Tipo => TipoPaquete.RespuestaRegistro;
        }
    }