ludusavi = Ludusavi
file-filter-executable = Exécutable
button-browse = Rechercher
button-open = Ouvrir
button-yes = Oui
button-yes-remembered = Oui, toujours
button-no = Non
button-no-remembered = Non, jamais
label-launch = Lancer
badge-failed = ERREUR
badge-ignored = IGNORÉ
needs-custom-entry =
    { $total-games ->
        [one] Ce jeu nécessite
       *[other] Certains jeux nécessitent
    } une entrée du chemin des fichiers de sauvegardes dans Ludusavi.

## Backup

back-up-specific-game =
    .confirm = Créer une copie sécurisée des sauvegardes pour { $game }?
    .on-success = Copie des sauvegardes effectuée pour { $game } ({ $processed-size })
    .on-unchanged = Pas de nouvelle sauvegarde à copier pour { $game }
    .on-empty = Impossible de trouver une sauvegarde à copier pour { $game }
    .on-failure = Sauvegardes sécurisées partiellement pour { $game } ({ $processed-size } sur { $total-size }), certaines ont échouées
# Defers to `back-up-specific-game.*`.
back-up-last-game = Copier et sécuriser les sauvegardes du dernier jeu lancé
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = Copier et sécuriser les sauvegardes des jeux sélectionnés
    .confirm = Copier et sécuriser les sauvegardes pour { $total-games } jeux sélectionnés ?
back-up-all-games = Copier et sécuriser les sauvegardes pour tous les jeux
    .confirm = Copier et sécuriser les sauvegardes pour tous les jeux que Ludusavi peut trouver?
    .on-success = Copies des sauvegardes effectuées pour { $processed-games } jeux ({ $processed-size }); cliquer pour voir la liste
    .on-failure = Copies des sauvegardes effectuées { $processed-games } sur { $total-games } jeux ({ $processed-size } sur { $total-size }), mais certaines ont échouées ; cliquer pour voir la liste
back-up-during-play-on-success = { $total-backups } Copies des sauvegardes ont été effectuées durant votre session de { $game }
back-up-during-play-on-failure = { $total-backups } Copies des sauvegardes ont été effectuées durant votre session de { $game }, dont { $failed-backups } ont échouées

## Restore

restore-specific-game =
    .confirm = Restaurer les données de sauvegarde pour { $game }?
    .on-success = Les données de sauvegarde ont été restaurées pour { $game } ({ $processed-size })
    .on-unchanged = Rien de plus à restaurer pour { $game }
    .on-empty = Aucune copie des sauvegardes trouvée pour restaurer { $game }
    .on-failure = Les données de sauvegarde ont été restaurées pour { $game } ({ $processed-size } sur { $total-size }), mais certaines ont échouées
# Defers to `restore-specific-game.*`.
restore-last-game = Restaurer les données de sauvegarde du dernier jeu lancé
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = Restaurer les données de sauvegarde des jeux sélectionnés
    .confirm = Restaurer les données de sauvegarde pour { $total-games } jeux sélectionnés?
restore-all-games = Restaurer les données de sauvegarde pour tous les jeux
    .confirm = Restaurer les données de sauvegarde pour tous les jeux que Ludusavi peut trouver?
    .on-success = Restauration des sauvegardes effectuées pour { $processed-games } jeux ({ $processed-size }); cliquer pour voir la liste
    .on-failure = Restauration des sauvegardes effectuées { $processed-games } sur { $total-games } jeux ({ $processed-size } sur { $total-size }), mais certaines ont échouées ; cliquer pour voir la liste

## Tags

add-tag-for-selected-games = Étiquette: "{ $tag }" - Ajouter pour les jeux sélectionnés
    .confirm =
        Ajouter l'étiquette "{ $tag }"  pour { $total-games } sélectionnés { $total-games ->
            [one] le jeu
           *[other] les jeux
        } et enlever toutes étiquettes incompatibles?
remove-tag-for-selected-games = Étiquette: "{ $tag }" - Enlever pour les jeux sélectionnés
    .confirm =
        Enlever l'étiquette "{ $tag }" pour { $total-games } sélectionnés { $total-games ->
            [one] le jeu
           *[other] les jeux
        } et enlever toutes étiquettes incompatibles?

## Generic errors

operation-still-pending = Ludusavi est déjà en train de travailler sur une requête, merci de réessayer après que la notification de complétion soit apparue.
no-game-played-yet = Vous n'avez joué à aucun jeu pour le moment.
unable-to-run-ludusavi = Impossible de lancer Ludusavi.
cannot-open-folder = Impossible d'ouvrir le dossier.
unable-to-synchronize-with-cloud = Impossible de se synchroniser avec le cloud.
cloud-synchronize-conflict = Vos copies de sauvegardes locales et dans le cloud ne sont pas les mêmes. Ouvrez Ludusavi et lancer un upload ou un téléchargement pour résoudre cette erreur.

## Settings

config-executable-path = Nom ou chemin complet de l'exécutable Ludusavi:
config-backup-path = Utiliser ce chemin pour les copies sécurisées des sauvegardes:
config-override-backup-format = Forcer ce format.
config-override-backup-compression = Modifier le mode de compression.
config-override-backup-retention = Configurer la conservation des copies.
config-full-backup-limit = Nombre max. de copies des sauvegardes par jeu:
config-differential-backup-limit = Nombre max. de copies entre deux sauvegardes complètes:
config-do-backup-on-game-stopped = Faire une copie des sauvegardes d'un jeu après y avoir joué
config-do-restore-on-game-starting = Restaurer les sauvegardes d'un jeu avant le démarrage
config-ask-backup-on-game-stopped = Demander à chaque fois plutôt que le faire automatiquement
config-only-backup-on-game-stopped-if-pc = Le faire uniquement pour les jeux PC
config-retry-unrecognized-game-with-normalization = En cas d'erreur, réessayer en normalisant le titre
config-add-suffix-for-non-pc-game-names = Ajouter un suffixe au nom lors de la recherche d'un jeu non-PC (requiert une entrée personnalisée):
config-retry-non-pc-games-without-suffix = Si pas de résultat lors de la recherche avec suffixe, réessayer sans
config-do-platform-backup-on-non-pc-game-stopped = Faire une copie des sauvegardes en utilisant le nom de plateforme d'un jeu non-PC après y avoir joué (requiert une entrée personnalisée)
config-do-platform-restore-on-non-pc-game-starting = Restaurer les sauvegardes par nom de plateforme avant le démarrage d'un jeu non-PC
config-ask-platform-backup-on-non-pc-game-stopped = Demander à chaque fois plutôt que le faire automatiquement
config-do-backup-during-play = Sécuriser les sauvegardes d'un jeu à un certain intervalle, s'il est configuré pour faire une copie des sauvegardes après y avoir joué sans demander
config-ignore-benign-notifications = Montrer les notifications qu'en cas d'erreur
config-tag-games-with-backups = Étiqueter automatiquement les jeux possédant une copie sécurisée avec "{ $tag }"
config-tag-games-with-unknown-save-data = Étiqueter automatiquement les jeux avec des données de sauvegardes inconnues avec "{ $tag }"
config-check-app-update = Rechercher les mises-à-jour automatiquement pour Ludusavi
config-ask-when-multiple-games-are-running = Demande de confirmation quand plusieurs jeux sont en cours d'exécution
label-minutes = Minutes:
option-simple = Simple
option-none = Aucun(e)

## Miscellaneous

initial-setup-required = Ludusavi n'a pas l'air d'être installé. Téléchargez-le puis suivez les instructions d'installation du plugin.
upgrade-prompt = Installer la version de Ludusavi { $version } ou plus récente pour une meilleure expérience. Cliquer pour voir la dernière version disponible.
upgrade-available = Ludusavi { $version } est désormais disponible. Cliquer pour voir les notes de mise-à-jour.
unrecognized-game = Ludusavi ne reconnais pas { $game }
look-up-as-other-title = Chercher avec un autre titre
look-up-as-normal-title = Chercher avec le titre par défaut
open-backup-directory = Ouvrir le répertoire de copies sécurisées
