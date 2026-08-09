# Objetivos actuales:
 - tener login, register y reanudar sesion
# Estado:
 - Server: todo
 - cliente: register

# Flujo:
 - cargar (conectar al server) -> funciona -> leer token -> reanudar sesion
                                           -> no hay token -> registrar / iniciar sesion 
                               -> no funciona -> reintentar / salir
                
