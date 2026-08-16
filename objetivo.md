# Roadmap — Sistema Multijugador de Astera

## 1. Reconciliación del movimiento

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

## 2. AOI — Spawn y Despawn de jugadores

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