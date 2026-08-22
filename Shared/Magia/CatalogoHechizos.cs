namespace Shared.Magia
{
    public static class CatalogoHechizos
    {
        // ============================================================
        // RUNAS ELEMENTALES
        // ============================================================

        public static readonly RunaElemental Fuego = new(
            new List<Tecla> {
                Tecla.W,
                Tecla.A,
                Tecla.S
            }
        );

        public static readonly RunaElemental Agua = new(
            new List<Tecla> {
                Tecla.A,
                Tecla.S,
                Tecla.D
            }
        );

        public static readonly RunaElemental Viento = new(
            new List<Tecla> {
                Tecla.S,
                Tecla.D,
                Tecla.R
            }
        );

        public static readonly RunaElemental Tierra = new(
            new List<Tecla> {
                Tecla.D,
                Tecla.R,
                Tecla.E
            }
        );

        public static readonly RunaElemental Hielo = new(
            new List<Tecla> {
                Tecla.R,
                Tecla.E,
                Tecla.Q
            }
        );

        public static readonly RunaElemental Rayo = new(
            new List<Tecla> {
                Tecla.W,
                Tecla.R,
                Tecla.Q
            }
        );

        public static readonly RunaElemental Sagrado = new(
            new List<Tecla> {
                Tecla.W,
                Tecla.D,
                Tecla.E
            }
        );

        public static readonly RunaElemental Oscuro = new(
            new List<Tecla> {
                Tecla.A,
                Tecla.R,
                Tecla.Q
            }
        );


        // ============================================================
        // COMBINACIONES
        // ============================================================

        public static readonly CombinacionTeclas IS = new(
            new List<Tecla> { Tecla.A },
            new List<Tecla> { Tecla.A, Tecla.D },
            new List<Tecla> { Tecla.A, Tecla.D, Tecla.S }
        );

        public static readonly CombinacionTeclas IA =
            CombinacionTeclas.Concatenar(IS, Tecla.W);

        public static readonly CombinacionTeclas ARA =
            CombinacionTeclas.Concatenar(IA, Tecla.R);

        public static readonly CombinacionTeclas ARAION =
            CombinacionTeclas.Concatenar(ARA, Tecla.Q);

        public static readonly CombinacionTeclas AERONIS =
            CombinacionTeclas.Concatenar(ARAION, Tecla.S);

        public static readonly CombinacionTeclas AERAVON =
            CombinacionTeclas.Concatenar(AERONIS, Tecla.D);


        // ============================================================
        // FUEGO - IGN
        // ============================================================

        public static readonly Hechizo Ignis =
            new("Ignis", 1, 10,
                "Una pequeña llama nacida de la esencia elemental del fuego.",
                new List<Elemento> { Elemento.Fuego }, Fuego, IS, 2f, 4f, 8f, "fuego_1");

        public static readonly Hechizo Ignia =
            new("Ignia", 2, 20,
                "Una llamarada más intensa capaz de abrasar aquello que encuentra a su paso.",
                new List<Elemento> { Elemento.Fuego }, Fuego, IA, 3f, 6f, 16f, "fuego_2");

        public static readonly Hechizo Ignara =
            new("Ignara", 3, 30,
                "Una poderosa corriente de fuego que envuelve al objetivo en llamas.",
                new List<Elemento> { Elemento.Fuego }, Fuego, ARA, 5f, 10f, 32f, "fuego_3");

        public static readonly Hechizo Ignaraion =
            new("Ignaraion", 4, 40,
                "Una violenta explosión ígnea que convierte el terreno en un mar de fuego.",
                new List<Elemento> { Elemento.Fuego }, Fuego, ARAION, 7f, 14f, 64f, "fuego_4");

        public static readonly Hechizo Ignaeronis =
            new("Ignaeronis", 5, 50,
                "El fuego elemental se desata con tal intensidad que consume todo cuanto toca.",
                new List<Elemento> { Elemento.Fuego }, Fuego, AERONIS, 10f, 20f, 128f, "fuego_5");

        public static readonly Hechizo Ignaeravon =
            new("Ignaeravon", 6, 60,
                "Una devastadora manifestación del fuego capaz de reducir a cenizas incluso a los enemigos más resistentes.",
                new List<Elemento> { Elemento.Fuego }, Fuego, AERAVON, 12f, 24f, 256f, "fuego_6");


        // ============================================================
        // AGUA - AQU
        // ============================================================

        public static readonly Hechizo Aquis =
            new("Aquis", 1, 10,
                "Una pequeña corriente de agua formada a partir de la esencia elemental.",
                new List<Elemento> { Elemento.Agua }, Agua, IS, 1f, 2f, 8f, "agua_1");

        public static readonly Hechizo Aquia =
            new("Aquia", 2, 20,
                "Un torrente de agua que golpea con suficiente fuerza para hacer retroceder a sus enemigos.",
                new List<Elemento> { Elemento.Agua }, Agua, IA, 3f, 6f, 16f, "agua_2");

        public static readonly Hechizo Aquara =
            new("Aquara", 3, 30,
                "Una poderosa masa de agua que se precipita sobre el objetivo con fuerza implacable.",
                new List<Elemento> { Elemento.Agua }, Agua, ARA, 5f, 10f, 32f, "agua_3");

        public static readonly Hechizo Aquaraion =
            new("Aquaraion", 4, 40,
                "Una violenta corriente elemental que arrasa todo lo que encuentra en su trayectoria.",
                new List<Elemento> { Elemento.Agua }, Agua, ARAION, 7f, 14f, 64f, "agua_4");

        public static readonly Hechizo Aquaeronis =
            new("Aquaeronis", 5, 50,
                "El poder del agua se desborda, formando una devastadora oleada capaz de aplastar a sus enemigos.",
                new List<Elemento> { Elemento.Agua }, Agua, AERONIS, 10f, 20f, 128f, "agua_5");

        public static readonly Hechizo Aquaeravon =
            new("Aquaeravon", 6, 60,
                "Una manifestación colosal del agua elemental que arrasa el campo de batalla como una inundación imparable.",
                new List<Elemento> { Elemento.Agua }, Agua, AERAVON, 12f, 24f, 256f, "agua_6");


        // ============================================================
        // VIENTO - AER
        // ============================================================

        public static readonly Hechizo Aeris =
            new("Aeris", 1, 10,
                "Una ráfaga ligera que concentra la esencia elemental del viento.",
                new List<Elemento> { Elemento.Viento }, Viento, IS, 1f, 2f, 8f, "viento_1");

        public static readonly Hechizo Aeria =
            new("Aeria", 2, 20,
                "Una corriente de aire afilada que atraviesa al enemigo con gran velocidad.",
                new List<Elemento> { Elemento.Viento }, Viento, IA, 3f, 6f, 16f, "viento_2");

        public static readonly Hechizo Aerara =
            new("Aerara", 3, 30,
                "Un poderoso corte de viento capaz de desgarrar aquello que encuentra en su camino.",
                new List<Elemento> { Elemento.Viento }, Viento, ARA, 5f, 10f, 32f, "viento_3");

        public static readonly Hechizo Aeraraion =
            new("Aeraraion", 4, 40,
                "Una tormenta de corrientes cortantes que envuelve al objetivo desde todas direcciones.",
                new List<Elemento> { Elemento.Viento }, Viento, ARAION, 7f, 14f, 64f, "viento_4");

        public static readonly Hechizo Aeraeronis =
            new("Aeraeronis", 5, 50,
                "El viento alcanza una velocidad aterradora, convirtiendo el aire en incontables hojas invisibles.",
                new List<Elemento> { Elemento.Viento }, Viento, AERONIS, 10f, 20f, 128f, "viento_5");

        public static readonly Hechizo Aeraeravon =
            new("Aeraeravon", 6, 60,
                "Una tempestad elemental que domina el campo de batalla y despedaza todo cuanto queda atrapado en ella.",
                new List<Elemento> { Elemento.Viento }, Viento, AERAVON, 12f, 24f, 256f, "viento_6");


        // ============================================================
        // TIERRA - TER
        // ============================================================

        public static readonly Hechizo Teris =
            new("Teris", 1, 10,
                "Una pequeña concentración de energía terrestre que hace brotar la fuerza de la tierra.",
                new List<Elemento> { Elemento.Tierra }, Tierra, IS, 1f, 2f, 8f, "tierra_1");

        public static readonly Hechizo Teria =
            new("Teria", 2, 20,
                "Un fragmento de roca elemental que golpea al enemigo con un peso considerable.",
                new List<Elemento> { Elemento.Tierra }, Tierra, IA, 3f, 6f, 16f, "tierra_2");

        public static readonly Hechizo Terara =
            new("Terara", 3, 30,
                "La tierra se alza con violencia, lanzando una masa rocosa contra el objetivo.",
                new List<Elemento> { Elemento.Tierra }, Tierra, ARA, 5f, 10f, 32f, "tierra_3");

        public static readonly Hechizo Teraraion =
            new("Teraraion", 4, 40,
                "Una poderosa ruptura terrestre que hace emerger enormes fragmentos de roca desde el suelo.",
                new List<Elemento> { Elemento.Tierra }, Tierra, ARAION, 7f, 14f, 64f, "tierra_4");

        public static readonly Hechizo Teraeronis =
            new("Teraeronis", 5, 50,
                "La fuerza de la tierra se desata en una devastadora sucesión de impactos que sacuden el terreno.",
                new List<Elemento> { Elemento.Tierra }, Tierra, AERONIS, 10f, 20f, 128f, "tierra_5");

        public static readonly Hechizo Teraeravon =
            new("Teraeravon", 6, 60,
                "Una manifestación colosal de la tierra que quiebra el terreno y sepulta a sus enemigos bajo toneladas de roca.",
                new List<Elemento> { Elemento.Tierra }, Tierra, AERAVON, 12f, 24f, 256f, "tierra_6");


        // ============================================================
        // HIELO - CRY
        // ============================================================

        public static readonly Hechizo Cryis =
            new("Cryis", 1, 10,
                "Una pequeña descarga de frío elemental que cubre al objetivo con una fina capa de escarcha.",
                new List<Elemento> { Elemento.Hielo }, Hielo, IS, 1f, 2f, 8f, "hielo_1");

        public static readonly Hechizo Cryia =
            new("Cryia", 2, 20,
                "Un fragmento de hielo afilado que congela lentamente aquello que alcanza.",
                new List<Elemento> { Elemento.Hielo }, Hielo, IA, 3f, 6f, 16f, "hielo_2");

        public static readonly Hechizo Cryara =
            new("Cryara", 3, 30,
                "Una poderosa ráfaga de frío que cubre al enemigo de hielo y ralentiza sus movimientos.",
                new List<Elemento> { Elemento.Hielo }, Hielo, ARA, 5f, 10f, 32f, "hielo_3");

        public static readonly Hechizo Cryaraion =
            new("Cryaraion", 4, 40,
                "Una tormenta de hielo que congela el terreno y atrapa a quienes quedan en su interior.",
                new List<Elemento> { Elemento.Hielo }, Hielo, ARAION, 7f, 14f, 64f, "hielo_4");

        public static readonly Hechizo Cryaeronis =
            new("Cryaeronis", 5, 50,
                "El frío elemental alcanza un nivel extremo, cubriendo todo a su alrededor con hielo impenetrable.",
                new List<Elemento> { Elemento.Hielo }, Hielo, AERONIS, 10f, 20f, 128f, "hielo_5");

        public static readonly Hechizo Cryaeravon =
            new("Cryaeravon", 6, 60,
                "Una devastadora tormenta glacial que congela el campo de batalla y sepulta a sus enemigos bajo un invierno eterno.",
                new List<Elemento> { Elemento.Hielo }, Hielo, AERAVON, 12f, 24f, 256f, "hielo_6");


        // ============================================================
        // RAYO - FUL
        // ============================================================

        public static readonly Hechizo Fulis =
            new("Fulis", 1, 10,
                "Una chispa elemental que concentra una pequeña cantidad de energía eléctrica.",
                new List<Elemento> { Elemento.Electrico }, Rayo, IS, 1f, 2f, 8f, "rayo_1");

        public static readonly Hechizo Fulia =
            new("Fulia", 2, 20,
                "Un relámpago veloz que descarga su energía directamente sobre el objetivo.",
                new List<Elemento> { Elemento.Electrico }, Rayo, IA, 3f, 6f, 16f, "rayo_2");

        public static readonly Hechizo Fulara =
            new("Fulara", 3, 30,
                "Una descarga eléctrica concentrada que atraviesa al enemigo con una fuerza creciente.",
                new List<Elemento> { Elemento.Electrico }, Rayo, ARA, 5f, 10f, 32f, "rayo_3");

        public static readonly Hechizo Fularaion =
            new("Fularaion", 4, 40,
                "Un poderoso relámpago que cae con violencia y libera una explosión de energía eléctrica.",
                new List<Elemento> { Elemento.Electrico }, Rayo, ARAION, 7f, 14f, 64f, "rayo_4");

        public static readonly Hechizo Fulaeronis =
            new("Fulaeronis", 5, 50,
                "La energía eléctrica se acumula hasta alcanzar una intensidad capaz de atravesar incluso defensas resistentes.",
                new List<Elemento> { Elemento.Electrico }, Rayo, AERONIS, 10f, 20f, 128f, "rayo_5");

        public static readonly Hechizo Fulaeravon =
            new("Fulaeravon", 6, 60,
                "Una descarga colosal que convoca el poder de la tormenta y arrasa el campo con relámpagos consecutivos.",
                new List<Elemento> { Elemento.Electrico }, Rayo, AERAVON, 12f, 24f, 256f, "rayo_6");


        // ============================================================
        // SAGRADO - SAN
        // ============================================================

        public static readonly Hechizo Sanis =
            new("Sanis", 1, 10,
                "Una pequeña manifestación de energía sagrada que purifica aquello que toca.",
                new List<Elemento> { Elemento.Sagrado }, Sagrado, IS, 1f, 2f, 8f, "sagrado_1");

        public static readonly Hechizo Sania =
            new("Sania", 2, 20,
                "Una luminosa descarga sagrada que hiere a las criaturas corrompidas por fuerzas oscuras.",
                new List<Elemento> { Elemento.Sagrado }, Sagrado, IA, 3f, 6f, 16f, "sagrado_2");

        public static readonly Hechizo Sanara =
            new("Sanara", 3, 30,
                "Una poderosa ráfaga de luz que purifica la corrupción y castiga a los enemigos impíos.",
                new List<Elemento> { Elemento.Sagrado }, Sagrado, ARA, 5f, 10f, 32f, "sagrado_3");

        public static readonly Hechizo Sanaraion =
            new("Sanaraion", 4, 40,
                "Un torrente de energía divina que envuelve al enemigo en una intensa luz purificadora.",
                new List<Elemento> { Elemento.Sagrado }, Sagrado, ARAION, 7f, 14f, 64f, "sagrado_4");

        public static readonly Hechizo Sanaeronis =
            new("Sanaeronis", 5, 50,
                "Una manifestación de poder divino capaz de quebrar las defensas de quienes han abrazado la oscuridad.",
                new List<Elemento> { Elemento.Sagrado }, Sagrado, AERONIS, 10f, 20f, 128f, "sagrado_5");

        public static readonly Hechizo Sanaeravon =
            new("Sanaeravon", 6, 60,
                "Una poderosa invocación celestial que desciende como un juicio divino sobre todo enemigo alcanzado por su luz.",
                new List<Elemento> { Elemento.Sagrado }, Sagrado, AERAVON, 12f, 24f, 256f, "sagrado_6");


        // ============================================================
        // OSCURO - UMB
        // ============================================================

        public static readonly Hechizo Umbis =
            new("Umbis", 1, 10,
                "Una pequeña manifestación de energía oscura que se aferra al objetivo como una sombra viviente.",
                new List<Elemento> { Elemento.Maldito }, Oscuro, IS, 1f, 2f, 8f, "oscuro_1");

        public static readonly Hechizo Umbia =
            new("Umbia", 2, 20,
                "Una descarga de energía maldita que corrompe lentamente aquello que alcanza.",
                new List<Elemento> { Elemento.Maldito }, Oscuro, IA, 3f, 6f, 16f, "oscuro_2");

        public static readonly Hechizo Umbara =
            new("Umbara", 3, 30,
                "Una poderosa oleada de oscuridad que consume la energía de quienes quedan atrapados en ella.",
                new List<Elemento> { Elemento.Maldito }, Oscuro, ARA, 5f, 10f, 32f, "oscuro_3");

        public static readonly Hechizo Umbaraion =
            new("Umbaraion", 4, 40,
                "Una masa de energía maldita que se extiende como una sombra y devora todo a su alrededor.",
                new List<Elemento> { Elemento.Maldito }, Oscuro, ARAION, 7f, 14f, 64f, "oscuro_4");

        public static readonly Hechizo Umbaeronis =
            new("Umbaeronis", 5, 50,
                "La oscuridad elemental toma forma y libera una fuerza corruptora capaz de quebrar incluso las defensas más firmes.",
                new List<Elemento> { Elemento.Maldito }, Oscuro, AERONIS, 10f, 20f, 128f, "oscuro_5");

        public static readonly Hechizo Umbaeravon =
            new("Umbaeravon", 6, 60,
                "Una manifestación abismal de energía maldita que sumerge el campo de batalla en una oscuridad capaz de consumirlo todo.",
                new List<Elemento> { Elemento.Maldito }, Oscuro, AERAVON, 12f, 24f, 256f, "oscuro_6");
    }
}