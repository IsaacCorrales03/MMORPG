# Astera MMORPG — Documentación Técnica

> Documentación generada a partir del análisis del código fuente del repositorio `MMORPG-main`. Proyecto en estado **early development / prototipo funcional**.

## Tabla de contenidos

1. [Descripción general](#1-descripción-general)
2. [Arquitectura](#2-arquitectura)
3. [Estructura del repositorio](#3-estructura-del-repositorio)
4. [Requisitos e instalación](#4-requisitos-e-instalación)
5. [Protocolo de red](#5-protocolo-de-red)
6. [Servidor (`Server/`)](#6-servidor-server)
7. [Cliente (`cliente/`)](#7-cliente-cliente)
8. [Librería compartida (`Shared/`)](#8-librería-compartida-shared)
9. [Modelo de datos](#9-modelo-de-datos)
10. [Flujos principales](#10-flujos-principales)
11. [Seguridad — estado actual y riesgos](#11-seguridad--estado-actual-y-riesgos)
12. [Deuda técnica / TODOs detectados](#12-deuda-técnica--todos-detectados)

---

## 1. Descripción general

**Astera** es un MMORPG 2D en desarrollo con arquitectura cliente-servidor autoritativa:

- **Servidor**: aplicación de consola en **C# / .NET 10**, con red UDP vía **LiteNetLib**, serialización binaria con **MessagePack**, persistencia con **EF Core** (SQLite en desarrollo, con soporte de PostgreSQL vía Npgsql) y autenticación por usuario/contraseña con `PasswordHasher` de ASP.NET Identity.
- **Cliente**: juego 2D construido en **Godot 4.7 (.NET / C#)**, con su propia capa de red basada en LiteNetLib, persistencia local de sesión cifrada con AES, e interpolación de movimiento de jugadores remotos.
- **Shared**: proyecto de clase de biblioteca (.NET 8) referenciado por ambos, que define los paquetes de red (DTOs), tipos de dominio (`Vector2`, `PlayerState`, `Chunk`, etc.) y utilidades de envío de paquetes.

El servidor corre un **tick loop autoritativo a 20 Hz**, valida el movimiento reportado por el cliente contra una distancia máxima permitida, y distribuye el estado del mundo a los clientes mediante snapshots filtrados por *Area of Interest* (chunks cercanos).

## 2. Arquitectura

```
┌──────────────────────┐        UDP (LiteNetLib)          ┌──────────────────────┐
│   Cliente (Godot)    │ ◄───────────────────────────────►│   Server (consola)   │
│                      │      MessagePack + Sobre         │                      │
│  Red/Cliente.cs      │                                  │  Red/NetworkServer   │
│  Red/Router.cs       │                                  │  Red/Router          │
│  Handlers/*          │                                  │  Handlers/*          │
│  Game/GameState      │                                  │  Mundo/World (tick)  │
│  Game/World, Player  │                                  │  Managers/Session    │
└─────────┬────────────┘                                  │  Managers/DataBase   │
          │                                               │  Servicios/Auth      │
          │ referencia                                    └───────────┬──────────┘
          ▼                                                           │
┌─────────────────────────────────────────────────────────────────────┴────┐
│                        Shared (Class Library, net8.0)                    │
│  Paquetes/* (DTOs MessagePack)  ·  Tipos/* (Vector2, Chunk, PlayerState) │
│  utils/PacketSender · utils/Claves                                       │
└──────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
                         SQLite (game.db) vía EF Core
```
Ambos extremos (`Server/Red/Router.cs` y `cliente/Red/Router.cs`) implementan el mismo patrón: leer un byte de tipo de paquete + payload MessagePack, deserializar según un diccionario `TipoPaquete → Type`, y despachar a un handler específico.

## 3. Estructura del repositorio

```
MMORPG-main/
├── MMORPG.slnx                # Solution (formato .slnx, nuevo formato de VS/dotnet)
├── objetivo.md                 # Notas de objetivos actuales del proyecto
├── Server/                     # Servidor de juego (.NET 10, consola)
│   ├── Program.cs              # Entry point: crea GameServer y corre el loop principal
│   ├── GameServer.cs           # Orquesta NetworkServer + World, tick fijo (20 Hz)
│   ├── ServerConsole.cs        # Dashboard interactivo en consola (players, comandos)
│   ├── Red/
│   │   ├── NetworkServer.cs    # Wrapper de LiteNetLib: conexión, auth por clave, recepción
│   │   └── Router.cs           # Enrutamiento de paquetes entrantes a handlers/eventos
│   ├── Handlers/                # IPacketHandler: Login, Register, ResumeSession, Spawn
│   ├── Managers/
│   │   ├── DataBaseManager.cs  # DbContext (EF Core, SQLite)
│   │   └── SessionManager.cs   # Sesiones en memoria (token ↔ usuario ↔ NetPeer)
│   ├── Servicios/
│   │   ├── ServicioAutenticacion.cs  # Registro/login, hashing de contraseñas
│   │   └── TokenGeneretor.cs         # Generador de tokens de sesión aleatorios
│   ├── Mundo/World.cs          # Estado del mundo, tick loop, movimiento, snapshots, AOI
│   ├── DBEntities/Jugador.cs   # Entidad EF Core (tabla Jugadores)
│   ├── Migrations/             # Migraciones EF Core (SQLite)
│   └── game.db(-wal/-shm)      # Base de datos SQLite (⚠ versionada en el repo)
├── cliente/                     # Cliente Godot 4.7 (.NET, C#)
│   ├── project.godot
│   ├── Escenas/                 # .tscn: Login, register, world, loading_screen, player
│   ├── Game/                    # Lógica de juego: Player, World, Camera, GameState, Login, Register, MainMenu, Enemy, Cargando
│   ├── Handlers/                 # Handlers de paquetes recibidos del servidor
│   ├── Red/                      # Cliente.cs (conexión), Router.cs, TokenManager.cs (persistencia cifrada)
│   ├── Styles/, Assets/          # Fuentes, spritesheets, shaders, fondos
│   └── Cliente.csproj
├── Shared/                       # Librería compartida (net8.0)
│   ├── Paquetes/                 # DTOs de red (MessagePack) + enum TipoPaquete
│   ├── Tipos/                    # Vector2, PlayerState, Chunk, ChunkPosition, SpatialGrid, Sesion, PlayerSnapshot
│   └── utils/                    # Claves (clave compartida cliente-servidor), PacketSender
└── .gitignore
```

## 4. Requisitos e instalación

| Componente | Requisito |
|---|---|
| Servidor | .NET SDK **10.0** |
| Shared | .NET SDK **8.0** (target del proyecto) |
| Cliente | **Godot 4.7.1** (mono/.NET) + .NET SDK 8.0 (9.0 si se exporta a Android) |
| Base de datos | SQLite embebido (por defecto). Hay paquete de Npgsql referenciado para Postgres, aunque el `DbContext` actual usa `UseSqlite` de forma fija. |

**Servidor:**
```bash
cd Server
dotnet restore
dotnet run
```
El servidor abre el puerto UDP **8455**, con un máximo de **10 jugadores** simultáneos (constantes en `GameServer.cs`). Al iniciarse, muestra un dashboard en consola con estado y jugadores conectados; acepta los comandos `stop`, `players`, `clear`, `help`.

**Cliente:**
Abrir `cliente/` desde Godot 4.7 (con soporte .NET) y ejecutar la escena principal. En el primer arranque genera un `config.json` junto al ejecutable con:
```json
{ "server_ip": "127.0.0.1", "server_port": 8455 }
```

## 5. Protocolo de red

- **Transporte:** LiteNetLib (UDP) sobre el puerto **8455**.
- **Autenticación de conexión:** al conectar, el cliente debe enviar la clave compartida `Claves.ClaveServidor` (definida en `Shared/utils/Clave.cs`); el servidor la valida en `OnConnectionRequest` antes de aceptar el peer.
- **Framing de paquete:** cada mensaje se compone de:
  1. 1 byte → `TipoPaquete` (enum, ver abajo)
  2. bytes con longitud prefijada → payload serializado con **MessagePack**
- **Tipos de paquete (`TipoPaquete`):**

| Valor | Dirección típica | DTO |
|---|---|---|
| `PeticionInicioSesion` | Cliente → Servidor | `PaquetePeticionInicioSesion` |
| `RespuestaInicioSesion` | Servidor → Cliente | `PaqueteRespuestaInicioSesion` |
| `PeticionAparecerJugador` | Cliente → Servidor | `PaquetePeticionAparecerJugador` |
| `PeticionRegistro` | Cliente → Servidor | `PaquetePeticionRegistro` |
| `RespuestaRegistro` | Servidor → Cliente | `PaqueteRespuestaRegistro` |
| `PeticionReanudarSesion` | Cliente → Servidor | `PaquetePeticionReanudarSesion` |
| `RespuestaReanudarSesion` | Servidor → Cliente | `PaqueteRespuestaReanudarSesion` |
| `Movimiento` | Cliente → Servidor | `PaqueteMovimiento` |
| `RespuestaAparecerJugador` | Servidor → Cliente | `PaqueteRespuestaAparecerJugador` |
| `Snapshot` | Servidor → Cliente | `PaqueteSnapshots` |

- **Canales de entrega (`Shared/utils/PacketSender.cs`):**
  - `EnviarOrdenado` → `ReliableOrdered` (login, registro, spawn, resume).
  - `EnviarMovimiento` → `Unreliable` (inputs de movimiento del cliente).
  - `EnviarSnapshot` → `Sequenced` (snapshots periódicos del servidor).

## 6. Servidor (`Server/`)

### 6.1 Ciclo de vida (`Program.cs`, `GameServer.cs`)
- `Program.cs` instancia `GameServer`, lo arranca, lanza un hilo para `ServerConsole` y corre un loop principal basado en `Stopwatch` que llama a `servidor.Update(delta)` en cada iteración (con `Thread.Sleep(1)`).
- `GameServer.Update(delta)` hace polling de eventos de red y acumula tiempo (`_tickAccumulator`) para ejecutar `World.Tick()` a una tasa fija de **20 Hz** (`TickRate = 1/20`).

### 6.2 Networking (`Red/NetworkServer.cs`, `Red/Router.cs`)
- `NetworkServer` encapsula LiteNetLib: gestiona conexión/desconexión, valida el cupo máximo de jugadores y la clave de acceso, y delega cada paquete recibido a `PacketRouter.Enrutar`.
- `PacketRouter` deserializa el paquete según `TiposDePaquete` y decide el destino:
  - Si hay un `IPacketHandler` registrado (login, registro, resume, spawn) → lo invoca directamente.
  - Si no hay handler pero el tipo está marcado como "evento" del mundo (actualmente solo `Movimiento`) → identifica al jugador vía `SesionManager.ObtenerPorPeer(peer)` y encola el paquete en `World.AddEvent`.

### 6.3 Handlers (`Handlers/`)
Todos implementan `IPacketHandler.Handle(NetPeer, IPaquete)`:
- **`LoginHandler`**: delega en `ServicioAutenticacion.IniciarSesion`; si es válido, crea sesión (`SesionManager.CrearSesion`) y responde con token, id y username.
- **`RegisterHandler`**: delega en `ServicioAutenticacion.registrar_jugador`; crea sesión automáticamente tras un registro exitoso.
- **`ResumeSessionHandler`**: reanuda una sesión existente a partir de un token guardado por el cliente (`SesionManager.Reanudar`), rotando el token.
- **`AparecerJugadorHandler`**: añade al jugador al `World` (`World.AddPlayer`) y responde con la posición de aparición. *(Nota: actualmente no reutiliza la última posición conocida — ver sección de deuda técnica).*

### 6.4 Mundo y simulación (`Mundo/World.cs`)
Componentes clave:
- `players: Dictionary<int, PlayerState>` — estado autoritativo de cada jugador.
- `EventPool: Dictionary<int, List<IPaquete>>` — cola de eventos pendientes por jugador, resuelta en cada tick. Los paquetes de movimiento se **coalescen**: si llegan movimientos "consecutivos" (`Consecutive > 1`), reemplaza el último en vez de acumular, evitando que la cola crezca sin control.
- `mapa: SpatialGrid` — grilla espacial de chunks de **512 unidades** (`ChunkPosition.Size`), usada para *Area of Interest* (AOI).
- **Validación de movimiento (`ProcesarMovimiento`)**: calcula la distancia máxima permitida (`MoveSpeed * ClientPhysicsDelta(1/60) * Consecutive`) y, si la posición reportada por el cliente la excede, la recorta a esa distancia en la dirección reportada — es decir, el servidor es autoritativo y corrige el movimiento sospechoso en vez de solo rechazarlo.
- **Snapshots (`BuildPacketSnapshots`)**: por cada jugador, recorre los chunks en un radio de `AoiChunkRadiusX=3 / AoiChunkRadiusY=2` alrededor de su chunk actual, y arma un paquete `PaqueteSnapshots` con los jugadores visibles. Se envía a cada peer vía `PacketSender.EnviarSnapshot` (canal `Sequenced`) en cada tick.

### 6.5 Persistencia y autenticación
- `DataBase : DbContext` (EF Core) expone `DbSet<Jugador>`, configurado con SQLite (`Data Source=game.db`), hardcodeado en `OnConfiguring` (no usa variables de entorno/appsettings).
- `Jugador` (entidad): `Id`, `NombreUsuario`, `PasswordHash`, `Email`, `FechaCreacion`.
- `ServicioAutenticacion` usa `PasswordHasher<Jugador>` (ASP.NET Identity) para hash/verificación, con **rehash automático** si el algoritmo queda desactualizado.
- `SesionManager` mantiene tres diccionarios concurrentes en memoria: `token→Sesion`, `usuarioId→token`, `usuarioId→NetPeer`. Al crear una nueva sesión para un usuario ya logueado, invalida la sesión anterior (single-session por usuario).

### 6.6 Consola del servidor (`ServerConsole.cs`)
Dashboard en tiempo real (refresco cada 500 ms) que muestra estado, cantidad de jugadores y su posición/velocidad, más un prompt de comandos (`stop`, `players`, `clear`, `help`) leído carácter por carácter desde `Console.ReadKey`.

## 7. Cliente (`cliente/`)

Proyecto Godot 4.7 (.NET). Escenas principales en `Escenas/`: `Login.tscn`, `register.tscn`, `loading_screen.tscn`, `world.tscn`, `player.tscn`.

### 7.1 Conexión (`Red/Cliente.cs`)
- Singleton (`Cliente.Instancia`) que lee `config.json` (IP/puerto del servidor, generado con valores por defecto si no existe) y gestiona la conexión LiteNetLib.
- Máquinas de estado propias: `EstadoConexion` (Desconectado/Conectando/Conectado/Fallida) y `EstadoAutenticacion` (NoAutenticado/Autenticando/Autenticado), expuestas como eventos C# (`OnEstadoConexionCambiado`, `OnEstadoAutenticacionCambiado`).
- Al conectar, intenta reanudar sesión automáticamente si existe un token guardado localmente (`TokenManager.LeerTokenGuardado`).
- Incluye timeout de conexión de 8 segundos.

### 7.2 Enrutamiento y handlers (`Red/Router.cs`, `Handlers/`)
Mismo patrón que el servidor: deserializa por `TipoPaquete` y despacha a un handler estático (`LoginHandler`, `RegisterHandler`, `ResumeSesionHandler`, `SpawnPlayerHandler`, `SnapshotHandler`), que actualizan `GameState` y emiten señales de Godot consumidas por las escenas de UI.

### 7.3 Persistencia local de sesión (`Red/TokenManager.cs`)
Guarda el token de sesión cifrado con **AES** en `user://ProfileData.enc`. **La clave AES está hardcodeada en el código fuente** (ver sección de seguridad).

### 7.4 Estado del juego (`Game/GameState.cs`)
Singleton central: usuario, token, id, y señales de dominio (`InicioSesionExitoso`, `RegistroExitoso`, `SesionReanudadaExitosa`, `AparecerJugador`, etc.). `IniciarJuego()` cambia a la escena del mundo y envía la petición de spawn al servidor.

### 7.5 Jugador y mundo (`Game/Player.cs`, `Game/World.cs`)
- **`Player`** distingue entre jugador local (`EsLocal`) y remoto:
  - Local: procesa input (`Input.GetVector`), mueve con `CharacterBody2D.MoveAndSlide`, y en cada `_PhysicsProcess` envía un `PaqueteMovimiento` con secuencia, input, posición reportada y contador de frames consecutivos con el mismo input (usado por el servidor para la validación de distancia).
  - Remoto: no procesa input local; interpola su posición hacia el `targetPosition` recibido en snapshots mediante suavizado exponencial (`Lerp` con factor `1 - e^(-speed·dt)`).
  - Sistema de animaciones direccionales (idle/walk/attack por dirección) y ataque básico con hurtbox.
- **`World`** (escena) escucha señales de `GameState` para instanciar el jugador local al recibir confirmación de spawn, y para crear/actualizar jugadores remotos a partir de cada `PaqueteSnapshots` recibido.
- **`Camera`**: sigue al jugador local con suavizado, zoom con rueda del mouse (rango 2.0–6.0), y bloqueo de cámara togglable.

### 7.6 UI (`Game/Login.cs`, `Game/Register.cs`, `Game/Cargando.cs`)
Pantallas de login/registro construidas con estilo propio (paleta ámbar/pergamino, esquinas ornamentales dibujadas por código), y una pantalla de carga (`Cargando`) que muestra estado de conexión/autenticación, tips rotativos y ping en tiempo real, con reintento manual ante fallos.

## 8. Librería compartida (`Shared/`)

- **`Paquetes/`**: todos los DTOs de red, decorados con `[MessagePackObject]` / `[Key(n)]` para serialización binaria eficiente. Implementan `IPaquete` (propiedad `Tipo` de solo lectura que identifica el `TipoPaquete`).
- **`Tipos/`**:
  - `Vector2`: struct propio (no depende de Godot), con operaciones básicas (suma, resta, producto escalar, distancia, normalización) — usado tanto en servidor como en el payload de red.
  - `PlayerState`: estado autoritativo de un jugador en el servidor (posición, velocidad, chunk, dirección, nombre).
  - `PlayerSnapshot`: versión serializable/liviana del estado de un jugador para enviar a los clientes.
  - `Chunk` / `ChunkPosition` / `SpatialGrid`: sistema de grilla espacial para AOI (chunks de 512 unidades).
  - `Sesion`: sesión autenticada (token, usuario, `NetPeer` asociado, fecha de creación).
- **`utils/`**:
  - `Claves.ClaveServidor`: clave compartida cliente-servidor para aceptar conexiones LiteNetLib (**hardcodeada**, ver seguridad).
  - `PacketSender`: helpers estáticos para serializar (MessagePack) y enviar paquetes por los distintos canales de entrega de LiteNetLib.

## 9. Modelo de datos

Base de datos SQLite (`Server/game.db`), gestionada con EF Core (carpeta `Migrations/` con 2 migraciones: `InitialCreate`, `InitialCreate2`).

**Tabla `Jugadores`** (mapeada desde `DBEntities/Jugador.cs`):

| Columna | Tipo | Notas |
|---|---|---|
| `Id` | int | PK, autoincremental |
| `NombreUsuario` | string | único a nivel de aplicación (verificado en `ServicioAutenticacion`, sin constraint explícito visto en el modelo) |
| `PasswordHash` | string | hash generado por `PasswordHasher<Jugador>` |
| `Email` | string | único a nivel de aplicación |
| `FechaCreacion` | DateTime (UTC) | default `DateTime.UtcNow` |

## 10. Flujos principales

**Registro:**
`Register.cs` (cliente) → `PaquetePeticionRegistro` → `RegisterHandler` (servidor) → `ServicioAutenticacion.registrar_jugador` (valida duplicados, hashea password, persiste) → `SesionManager.CrearSesion` → `PaqueteRespuestaRegistro` (token) → cliente guarda token cifrado y navega al juego.

**Login:**
`Login.cs` → `PaquetePeticionInicioSesion` → `LoginHandler` → `ServicioAutenticacion.IniciarSesion` (verifica hash, rehash si aplica) → crea sesión → `PaqueteRespuestaInicioSesion` (token, id, username).

**Reanudar sesión (reconexión):**
Al conectar, el cliente lee el token cifrado local y envía `PaquetePeticionReanudarSesion` → `ResumeSessionHandler` → `SesionManager.Reanudar` (rota el token) → respuesta con nuevos datos de sesión.

**Aparecer en el mundo (spawn):**
Tras autenticarse, `GameState.IniciarJuego()` cambia a la escena `world.tscn` y envía `PaquetePeticionAparecerJugador` → `AparecerJugadorHandler` agrega al jugador a `World` → responde `PaqueteRespuestaAparecerJugador` → cliente instancia el `Player` local.

**Movimiento y sincronización (loop continuo):**
Cliente: cada `_PhysicsProcess`, mueve localmente (movimiento predicho/autoritativo del lado cliente) y envía `PaqueteMovimiento` (unreliable) → Servidor: encola el evento por jugador (coalescido si es consecutivo) → en cada tick (20 Hz) `World.Tick()` valida/corrige la posición, reconstruye snapshots por AOI y envía `PaqueteSnapshots` (sequenced) a cada jugador → Cliente: `World.cs` actualiza/crea jugadores remotos e interpola su posición.

## 11. Seguridad — estado actual y riesgos

Estos son hallazgos directos del código, útiles para priorizar hardening antes de producción:

- **Clave de servidor hardcodeada**: `Shared/utils/Clave.cs` contiene la clave de conexión en texto plano dentro del código fuente, compartida entre cliente y servidor. Cualquiera con acceso al binario del cliente puede extraerla.
- **Clave AES hardcodeada en el cliente**: `TokenManager.cs` cifra el token de sesión localmente con una clave fija embebida en el código, lo que ofuscación pero no protege realmente el archivo `ProfileData.enc` de alguien con acceso al binario/decompilación.
- **Cadena de conexión a base de datos hardcodeada**: `DataBaseManager.cs` fija `Data Source=game.db` sin usar configuración externa (`appsettings.json`, variables de entorno), lo cual dificulta el despliegue en distintos entornos y expone la ruta en el propio código.
- **Base de datos versionada en el repositorio**: `Server/game.db`, `game.db-wal` y `game.db-shm` están incluidos en el repo, lo que puede filtrar datos de usuarios (incluso si son de prueba) y genera conflictos de merge.
- **Unicidad de usuario/email no garantizada a nivel de esquema**: la validación de duplicados se hace con una consulta previa (`AnyAsync`) en vez de una constraint `UNIQUE` en la base de datos, lo que deja una ventana de condición de carrera ante registros concurrentes.
- **Sin límite de intentos de login/registro** (rate limiting o backoff) visible en el código.

## 12. Deuda técnica / TODOs detectados

- `AparecerJugadorHandler`: el comentario en el propio código indica que falta obtener la última posición conocida del jugador desde el `SessionManager`/DB en vez de spawnear siempre en `(0,0)`.
- `ServerConsole.MostrarJugadoresDebajo`: método vacío, pendiente de implementar un panel de log de eventos.
- `ServerConsole.EjecutarComando`: los comandos `help` y desconocidos no tienen implementación (cases vacíos).
- `Enemy.cs`: script placeholder, sin lógica de combate/IA implementada.
- `objetivo.md`: documento de notas informal indicando que el objetivo actual es sincronizar movimiento entre dos jugadores — confirma que el proyecto está en etapa temprana.
- Falta de tests automatizados (no se encontraron proyectos de test en la solución).
