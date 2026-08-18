# Roadmap — Sistema Multijugador de Astera

## 1. Reconciliación del movimiento - Realizado 

### Objetivo general
Sincronizar la posición del jugador local con el estado del servidor sin que el movimiento se sienta entrecortado.

### Objetivos específicos
- Utilizar `Sequence` para identificar cada input enviado.
- Hacer que el servidor confirme el estado procesado.
- Detectar diferencias entre la posición local y la posición del servidor.
- Corregir la posición del jugador cuando exista desincronización.
- Reaplicar inputs pendientes después de una corrección.
- Evitar que pequeños errores de red produzcan teletransportes visibles.

### Resultado
El jugador local tendrá predicción de movimiento y reconciliación, manteniendo una sensación inmediata mientras permanece sincronizado con el servidor.

---

## 2. AOI — Spawn y Despawn de jugadores - Realizado

### Objetivo general
Hacer que cada cliente solamente mantenga en escena las entidades que se encuentran dentro de su área de interés.

### Objetivos específicos
- Utilizar el `SpatialGrid` para determinar qué jugadores son visibles.
- Comparar los jugadores de la snapshot actual con los que el cliente ya conoce.
- Crear automáticamente jugadores remotos nuevos.
- Actualizar jugadores que ya existen.
- Detectar jugadores que dejaron el AOI.
- Eliminar los nodos de jugadores que ya no son visibles.
- Mantener al jugador local separado del sistema de spawn remoto.

### Resultado

```text
Snapshot
   ↓
Comparación
   ├── Nuevo       → Spawn
   ├── Existente   → Actualizar
   └── Ausente     → Despawn
```

El cliente tendrá únicamente los jugadores que realmente necesita representar.

---

## 3. Desconexión y reconexión - Realizado

### Objetivo general
Garantizar que las conexiones y desconexiones no dejen entidades duplicadas, jugadores fantasma o referencias inválidas.

### Objetivos específicos
- Detectar correctamente la desconexión de un jugador.
- Eliminar su estado del servidor.
- Eliminarlo del `SpatialGrid`.
- Eliminar su `EventPool`.
- Eliminar su snapshot correspondiente.
- Hacer que los clientes remotos detecten su desaparición.
- Permitir que un jugador vuelva a conectarse correctamente.
- Garantizar que reciba un estado de juego limpio.
- Evitar conflictos entre `PlayerId`, cámara y autoridad local.

### Resultado

```text
A conecta
   ↓
B conecta
   ↓
B desconecta
   ↓
B desaparece de A
   ↓
B vuelve a conectar
   ↓
B aparece correctamente
```

Sin jugadores duplicados ni cámaras incorrectas.

---

## 4. Snapshots e interpolación - Realizado 

### Objetivo general
Conseguir que el movimiento remoto sea visualmente fluido independientemente de que las snapshots lleguen a 20 Hz.

### Objetivos específicos
- Mantener snapshots a 20 ticks por segundo.
- Conservar posición anterior y posición objetivo.
- Interpolar el movimiento entre snapshots.
- Ajustar correctamente la velocidad de interpolación.
- Manejar pérdida ocasional de snapshots.
- Evitar que un jugador remoto se quede congelado indefinidamente.
- Detectar teletransportaciones y aplicarlas directamente.
- Sincronizar correctamente animación y dirección.

### Resultado

```text
Servidor
20 snapshots/s
      ↓
Buffer
      ↓
Interpolación
      ↓
Movimiento remoto fluido
```

El servidor seguirá funcionando a 20 Hz mientras el cliente renderiza a la frecuencia de su propio juego.

---

## 5. Sistema de combate

### Objetivo general
Construir el sistema de combate sobre la arquitectura multijugador existente.

### Objetivos específicos
- Crear estados de ataque.
- Sincronizar ataques mediante paquetes.
- Sincronizar dirección de ataque.
- Sincronizar animaciones.
- Utilizar `HitBox` y `HurtBox`.
- Detectar colisiones mediante Godot Physics.
- Crear cooldowns.
- Evitar ataques imposibles.
- Determinar qué información debe validar el servidor.
- Replicar los eventos de combate a los jugadores cercanos.

### Resultado

```text
Jugador A
   │
   │ Ataque
   ▼
Godot Physics
   │
   ▼
HitBox → HurtBox
   │
   ▼
Evento de combate
   │
   ▼
Servidor
   │
   ├── Estado
   └── Replicación
           ↓
       Jugadores
```

El combate funcionará tanto localmente como en multijugador.

---

## 6. Vida, daño y muerte

### Objetivo general
Crear el ciclo de vida de las entidades combatibles.

### Objetivos específicos
- Añadir puntos de vida.
- Crear estados de daño.
- Aplicar daño.
- Crear invulnerabilidad temporal cuando corresponda.
- Sincronizar vida mediante el servidor.
- Replicar eventos de daño.
- Crear estado de muerte.
- Desactivar interacción durante la muerte.
- Implementar respawn.
- Sincronizar respawn con el AOI.

### Resultado

```text
HP 100
  ↓
Ataque
  ↓
Daño -25
  ↓
HP 75
  ↓
...
  ↓
HP 0
  ↓
Muerte
  ↓
Respawn
```

El estado de vida será consistente para todos los jugadores.

---

## 7. Sistema general de entidades

### Objetivo general
Preparar la arquitectura para que el `World` no esté limitado exclusivamente a jugadores.

### Objetivos específicos
- Crear una abstracción común para entidades.
- Incorporar entidades al `SpatialGrid`.
- Incorporar entidades al sistema AOI.
- Asignar identificadores únicos.
- Diferenciar tipos de entidades.
- Crear snapshots apropiadas para cada tipo.
- Permitir spawn/despawn dinámico.
- Preparar la arquitectura para NPCs, monstruos, objetos y proyectiles.

### Resultado

```text
World
 │
 ├── Players
 │
 ├── NPCs
 │
 ├── Monsters
 │
 ├── Items
 │
 └── Projectiles
        │
        ▼
   SpatialGrid
        │
        ▼
       AOI
        │
        ▼
     Clientes
```

El sistema de red dejará de estar diseñado específicamente alrededor de `Player` y pasará a manejar entidades del mundo.

---

## Orden de implementación

| # | Sistema                      | Prioridad |
|---|-------------------------------|-----------|
| 1 | Reconciliación                | Crítica   |
| 2 | AOI Spawn/Despawn              | Crítica   |
| 3 | Desconexión/Reconexión         | Crítica   |
| 4 | Snapshots/Interpolación        | Alta      |
| 5 | Combate                        | Alta      |
| 6 | Vida/Daño/Muerte                | Alta      |
| 7 | Sistema general de entidades    | Media     |


## Bugs Count:
- Si dos jugadores chocan, uno puede transportar al otro
- No deberían de poder existir dos clientes con el mismo id mandando ordenes
