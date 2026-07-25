using Tipos;

namespace Entidades
{
    public class Entidad
    {
        public string Nombre {get;set;}
        public int Vida {get;set;}
        public int Vida_maxina {get;set;}
        public Vector2 Posicion {get;set;}

        public Entidad(string nombre, int vida, int vida_maxina)
        {
            Nombre = nombre;
            Vida = vida;
            Vida_maxina = vida_maxina;
            Posicion = new Vector2(0,0);
        }

        public void Mover(Vector2 desplazamiento)
        {
         Posicion = Posicion + desplazamiento;
        }
        
    }
}