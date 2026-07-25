using Entidades;
using Mundo;
using Tipos;

class Programa
{
    public static void Main()
    {
        Mapa mapa = new Mapa(20,30);
        Entidad Player = new Entidad("carlos", 10, 12);
        Vector2 desplazamiento = new Vector2(0,0);
        while (true)
        {
            Console.Clear();
            mover_dibujar(Player, desplazamiento, mapa);

            char key = Console.ReadKey(true).KeyChar;
            if (key == 'w')
            {
                // arriba
                desplazamiento = new Vector2(-1,0);
            }
            else if (key == 's')
            {
                // abajo
                desplazamiento = new Vector2(1,0);
            }
            else if (key == 'd')
            {
                // derecha
                desplazamiento = new Vector2(0,1);
            }
            else if (key == 'a')
            {
                // izquierda
                desplazamiento = new Vector2(0,-1);
            }
            else
            {
                desplazamiento = new Vector2(0,0);
            }
        }

    }
    public static void mover_dibujar(Entidad jugador, Vector2 desplazamiento, Mapa mapa)
    {
        //ocupo, mover al jugador, cambiar un tile normal por el jugador, y finalmente
        // intercambiar el tile actual del jugador por un tile normal
        Vector2 nueva_pos = jugador.Posicion + desplazamiento;
        if (mapa.esPosicionValida(nueva_pos))
        {
            mapa.redibujar(jugador.Posicion);
            jugador.Mover(desplazamiento);
            mapa.plantar_jugador(jugador.Posicion);
            Console.Write(mapa.dibujar());

        }
        else
        {
            Console.WriteLine("La posición dada excede los limites del mapa");

        }


    }

}