ludusavi = Ludusavi
file-filter-executable = Ejecutable
button-browse = Examinar
button-open = Abrir
button-yes = Si
button-yes-remembered = Sí, siempre
button-no = No
button-no-remembered = No, nunca
label-launch = Abrir
badge-failed = FALLADO
badge-ignored = IGNORADO
needs-custom-entry =
    { $total-games ->
        [one] Este juego requiere
        *[other] Algunos juegos requieren
    } una entrada personalizada coincidente en Ludusavi.

## Backup

back-up-specific-game =
    .confirm = ¿Respaldar datos guardados para { $game }?
    .on-success = Se han respaldado los datos de salvado para { $game } ({ $processed-size })
    .on-unchanged = No hay nada nuevo que guardar para { $game }
    .on-empty = No se encontraron datos de guardado para respaldar de { $game }
    .on-failure = Se respaldaron los datos de guardado de { $game } ({ $processed-size } de { $total-size }), pero hubo errores en algunos respaldos
# Defers to `back-up-specific-game.*`.
back-up-last-game = Respaldar datos guardados para el último juego jugado
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = Respaldar datos guardados para los juegos seleccionados
    .confirm = ¿Respaldar datos guardados para { $total-games } juegos seleccionados?
back-up-all-games = Respaldar datos guardados para todos los juegos
    .confirm = ¿Respaldar datos guardados para todos los juegos que Ludusavi puede encontrar?
    .on-success = Se respaldaron datos de guardado de { $processed-games } juegos ({ $processed-size }); clic para lista completa
    .on-failure = Se respaldaron datos de guardado de { $processed-games } de { $total-games } juegos ({ $processed-size } de { $total-size }), pero algunos fallaron; haz clic para la lista completa
back-up-during-play-on-success = Se han activado { $total-backups } copias de seguridad mientras se jugaba { $game }
back-up-during-play-on-failure = Se han activado { $total-backups } copias de seguridad mientras se jugaba { $game }, de las cuales { $failed-backups } han fallado

## Restore

restore-specific-game =
    .confirm = ¿Respaldar datos guardados de { $game }?
    .on-success = Se restauraron datos de guardado de { $game } ({ $processed-size })
    .on-unchanged = Nada nuevo para restaurar para { $game }
    .on-empty = No se encontraron datos de guardado para respaldar de { $game }
    .on-failure = Se restauraron datos de guardado de { $game } ({ $processed-size } de { $total-size }), pero algunos respaldos fallaron
# Defers to `restore-specific-game.*`.
restore-last-game = Restaurar datos guardados para el último juego jugado
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = Restaurar datos guardados para los juegos seleccionados
    .confirm = ¿Restaurar datos de guardado para { $total-games } juegos seleccionados?
restore-all-games = Restaurar datos guardados para todos los juegos
    .confirm = ¿Restaurar datos guardados para todos los juegos que Ludusavi puede encontrar?
    .on-success = Se restauraron datos de guardado de { $processed-games } juegos ({ $processed-size }); clic para lista completa
    .on-failure = Se restauraron datos de guardado de { $processed-games } de { $total-games } juegos ({ $processed-size } de { $total-size }), pero algunos fallaron; haz clic para la lista completa

## Tags

add-tag-for-selected-games = Etiqueta: "{ $tag }" - Añadir para los juegos seleccionados
    .confirm =
        ¿Añadir etiqueta "{ $tag }" para { $total-games } seleccionadas { $total-games ->
            [one] juego
           *[other] juegos
        } y eliminar cualquier etiqueta en conflicto?
remove-tag-for-selected-games = Etiqueta: "{ $tag }" - Quitar de los juegos seleccionados
    .confirm =
        Eliminar la etiqueta "{ $tag }" para { $total-games } seleccionados { $total-games ->
            [one] juego
           *[other] juegos
        } y eliminar cualquier etiqueta en conflicto?

## Generic errors

operation-still-pending = Ludusavi todavía está trabajando en una solicitud anterior. Por favor, inténtalo de nuevo cuando veas la notificación de que ha terminado.
no-game-played-yet = Aún no has jugado nada en esta sesión.
unable-to-run-ludusavi = No se ha podido ejecutar Ludusavi.
cannot-open-folder = No se puede abrir la carpeta.
unable-to-synchronize-with-cloud = No se ha podido sincronizar con la nube.
cloud-synchronize-conflict = Tus copias de seguridad locales y en la nube están en conflicto. Abre Ludusavi y realiza una subida o descarga para resolver esto.

## Settings

config-executable-path = Nombre o ruta completa del ejecutable de Ludusavi:
config-backup-path = Reemplazar ruta completa del directorio para almacenar los respaldos:
config-override-backup-format = Reemplazar formato de copia de seguridad.
config-override-backup-compression = Reemplazar la compresión de copia de seguridad.
config-override-backup-retention = Reemplazar retención de copia de seguridad.
config-full-backup-limit = Máximas copias de seguridad completas por juego:
config-differential-backup-limit = Máximas copias de seguridad diferenciales por copia de seguridad:
config-do-backup-on-game-stopped = Respaldar los datos guardados de un juego después de jugar
config-do-restore-on-game-starting = También restaurar datos guardados para un juego antes de jugar
config-ask-backup-on-game-stopped = Preguntar primero en lugar de hacerlo automáticamente
config-only-backup-on-game-stopped-if-pc = Solo hacer esto para juegos de PC
config-retry-unrecognized-game-with-normalization = Si no se encuentra, vuelva a intentarlo normalizando el título
config-add-suffix-for-non-pc-game-names = Buscar juegos que no sean de PC añadiendo este sufijo a sus nombres (requiere entrada personalizada):
config-retry-non-pc-games-without-suffix = Si no se encuentra con el sufijo, entonces inténtelo de nuevo sin él
config-do-platform-backup-on-non-pc-game-stopped = Respaldar datos guardados por nombre de plataforma después de jugar juegos que no sean de PC (requiere entrada personalizada)
config-do-platform-restore-on-non-pc-game-starting = Restaurar también los datos guardados por nombre de plataforma antes de jugar partidas que no sean de PC
config-ask-platform-backup-on-non-pc-game-stopped = Preguntar primero en lugar de hacerlo automáticamente
config-do-backup-during-play = Respaldar juegos en un intervalo mientras se juega, si también serían respaldadas después del juego sin preguntar
config-ignore-benign-notifications = Mostrar sólo notificaciones por fallo
config-tag-games-with-backups = Etiquetar automáticamente juegos con copias de seguridad como "{ $tag }"
config-tag-games-with-unknown-save-data = Etiquetar automáticamente juegos con copias de seguridad desconocidas como "{ $tag }"
config-check-app-update = Check for Ludusavi updates automatically
config-ask-when-multiple-games-are-running = Require confirmation when multiple games are running
label-minutes = Minutos:
option-simple = Simple
option-none = Ninguno

## Miscellaneous

initial-setup-required = Ludusavi no parece estar instalado. Por favor, descargalo y sigue las instrucciones de configuración del plugin.
upgrade-prompt = Instala Ludusavi { $version } o más reciente para obtener la mejor experiencia. Haz clic para ver la última versión.
upgrade-available = Ludusavi { $version } is now available. Click to view the release notes.
unrecognized-game = Ludusavi no reconoce { $game }
look-up-as-other-title = Buscar con otro título
look-up-as-normal-title = Buscar con el título por defecto
open-backup-directory = Abrir directorio de respaldo
