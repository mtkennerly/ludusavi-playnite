ludusavi = Ludusavi
file-filter-executable = Eseguibile
button-browse = Sfoglia
button-open = Apri
button-yes = Si
button-yes-remembered = Sì, sempre
button-no = No
button-no-remembered = No, mai
label-launch = Avvia
badge-failed = FALLITO
badge-ignored = IGNORATO
needs-custom-entry =
    { $total-games ->
        [one] Questo gioco richiede
        *[other] Alcuni giochi richiedono
    } una voce personalizzata corrispondente in { ludusavi }.

## Backup

back-up-specific-game =
    .confirm = Backup dei dati di salvataggio per { $game }?
    .on-success = Salvataggi salvati per { $game } ({ $processed-size })
    .on-unchanged = Niente di nuovo per cui eseguire il backup per { $game }
    .on-empty = Nessun dato di salvataggio trovato per il backup di { $game }
    .on-failure = Salvataggi salvati per { $game } ({ $processed-size } di { $total-size }), ma alcuni salvataggi non sono riusciti
# Defers to `back-up-specific-game.*`.
back-up-last-game = Backup dati di salvataggio per l'ultimo gioco giocato
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = Backup dei dati di salvataggio per i giochi selezionati
    .confirm = Backup dei dati di salvataggio per { $total-games } giochi selezionati?
back-up-all-games = Backup dei dati di salvataggio per tutti i giochi
    .confirm = Backup dei dati di salvataggio per tutti i giochi che Ludusavi può trovare?
    .on-success = Salvataggi salvati per { $processed-games } giochi ({ $processed-size }); clicca per la lista completa
    .on-failure = Salvataggi salvati per { $processed-games } di { $total-games } giochi ({ $processed-size } di { $total-size }), ma alcuni non sono riusciti; clicca per la lista completa
back-up-during-play-on-success = Attivato { $total-backups } backup durante la riproduzione di { $game }
back-up-during-play-on-failure = Attivato { $total-backups } backup durante la riproduzione di { $game }, di cui { $failed-backups } falliti

## Restore

restore-specific-game =
    .confirm = Ripristina i dati di salvataggio per { $game }?
    .on-success = Salvataggi ripristinati per { $game } ({ $processed-size })
    .on-unchanged = Niente di nuovo da ripristinare per { $game }
    .on-empty = Nessun salvataggio trovato da ripristinare per { $game }
    .on-failure = Salvataggi ripristinati per { $game } ({ $processed-size } di { $total-size }), ma alcuni salvataggi sono falliti
# Defers to `restore-specific-game.*`.
restore-last-game = Ripristina salvataggi dell'ultimo gioco giocato
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = Ripristina salvataggi per i giochi selezionati
    .confirm = Ripristina i dati di salvataggio per { $total-games } giochi selezionati?
restore-all-games = Ripristina i dati di salvataggio per tutti i giochi
    .confirm = Ripristinare il salvataggio dei dati per tutti i giochi che Ludusavi riesce a trovare?
    .on-success = Salvataggi ripristinati per { $processed-games } giochi ({ $processed-size }); clicca per la lista completa
    .on-failure = Salvataggi ripristinati per { $processed-games } di { $total-games } giochi ({ $processed-size } di { $total-size }), ma alcuni sono falliti; clicca per la lista completa

## Tags

add-tag-for-selected-games = Tag: "{ $tag }" - Aggiungi per i giochi selezionati
    .confirm =
        Aggiungere "{ $tag }" tag per { $total-games } selezionati { $total-games ->
            [one] gioco
           *[other] giochi
        } e rimuovere eventuali tag in conflitto?
remove-tag-for-selected-games = Tag: "{ $tag }" - Rimuovi per i giochi selezionati
    .confirm =
        Rimuovere "{ $tag }" tag per { $total-games } selezionati { $total-games ->
            [one] gioco
           *[other] giochi
        } e rimuovere eventuali tag in conflitto?

## Generic errors

operation-still-pending = { ludusavi } sta ancora lavorando su una richiesta precedente. Si prega di riprovare quando vedi la notifica che è finita.
no-game-played-yet = Non hai ancora giocato nulla in questa sessione.
unable-to-run-ludusavi = Impossibile avviare { ludusavi }.
cannot-open-folder = Impossibile aprire la cartella.
unable-to-synchronize-with-cloud = Impossibile sincronizzare con il cloud.
cloud-synchronize-conflict = I backup locali e del cloud sono in conflitto. Apri Ludusavi ed esegui un caricamento o un download per risolvere il problema.

## Settings

config-executable-path = Nome o percorso completo dell'eseguibile Ludusavi:
config-backup-path = Sovrascrivere il percorso completo alla directory per memorizzare i backup:
config-override-backup-format = Sovrascrivere il formato di backup.
config-override-backup-compression = Sovrascrivi la compressione del backup.
config-override-backup-retention = Sovrascrivi la ritenzione di backup.
config-full-backup-limit = Backup completi massimi per partita:
config-differential-backup-limit = Backup differenziali massimi per backup completi:
config-do-backup-on-game-stopped = Backup dei dati salvati per un gioco dopo averlo giocato
config-do-restore-on-game-starting = Ripristina anche i dati di salvataggio per un gioco prima di giocarlo
config-ask-backup-on-game-stopped = Chiedi prima invece di farlo automaticamente
config-only-backup-on-game-stopped-if-pc = Fai questo solo per i giochi per PC
config-retry-unrecognized-game-with-normalization = Se non trovato, riprovare normalizzando il titolo
config-add-suffix-for-non-pc-game-names = Cerca giochi non-PC aggiungendo questo suffisso ai loro nomi (richiede voce personalizzata):
config-retry-non-pc-games-without-suffix = Se non è stato trovato con il suffisso, riprova senza di esso
config-do-platform-backup-on-non-pc-game-stopped = Esegui il backup dei dati per nome della piattaforma dopo aver giocato con giochi non-PC (richiede una voce personalizzata)
config-do-platform-restore-on-non-pc-game-starting = Ripristina anche i dati di salvataggio per nome della piattaforma prima di giocare a giochi non-PC
config-ask-platform-backup-on-non-pc-game-stopped = Chiedi prima invece di farlo automaticamente
config-do-backup-during-play = Effettua il backup dei giochi in un intervallo durante la partita, anche se il backup sarebbe stato effettuato dopo la partita senza chiedere
config-ignore-benign-notifications = Mostra solo le notifiche in caso di fallimento
config-tag-games-with-backups = Etichetta automaticamente i giochi con i backup come "{ $tag }"
config-tag-games-with-unknown-save-data = Etichetta automaticamente i giochi con dati sconosciuti come "{ $tag }"
label-minutes = Minuti:
option-simple = Semplice
option-none = Nessuna

## Miscellaneous

upgrade-prompt = Installa Ludusavi { $version } o più recente per la migliore esperienza. Clicca per vedere l'ultima versione.
unrecognized-game = Ludusavi non riconosce { $game }
look-up-as-other-title = Cerca con un altro titolo
look-up-as-normal-title = Cerca con il titolo predefinito
