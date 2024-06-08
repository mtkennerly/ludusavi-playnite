ludusavi = Ludusavi
file-filter-executable = Executável
button-browse = Pesquisar
button-open = Abrir
button-yes = Sim
button-yes-remembered = Sim, sempre
button-no = Não
button-no-remembered = Não, nunca
label-launch = Executar
badge-failed = FALHOU
badge-ignored = IGNORADO
needs-custom-entry =
    { $total-games ->
        [one] Este jogo requer
        *[other] Alguns jogos requerem
    } uma entrada personalizada correspondente em { ludusavi }.

## Backup

back-up-specific-game =
    .confirm = Fazer backup dos dados salvos para { $game }?
    .on-success = Backup dos saves de { $game } ({ $processed-size })
    .on-unchanged = Nothing new to back up for { $game }
    .on-empty = Não foram encontrados dados salvos para o backup de { $game }
    .on-failure = Foi realizado backup de saves para { $game } ({ $processed-size } de { $total-size }), mas alguns saves falharam
# Defers to `back-up-specific-game.*`.
back-up-last-game = Fazer backup de dados salvos para a última partida jogada
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = Faça backup de dados salvos para jogos selecionados
    .confirm = Fazer backup dos dados para { $total-games } jogos selecionados?
back-up-all-games = Fazer backup de dados salvos para todos os jogos
    .confirm = Fazer backup de dados para todos os jogos que Ludusavi pode encontrar?
    .on-success = Backup de saves para { $processed-games } jogos ({ $processed-size }); clique para lista completa
    .on-failure = Foi realizado backup de saves para { $processed-games } de { $total-games } jogos ({ $processed-size } de { $total-size }), mas alguns falharam; clique para lista completa
back-up-during-play-on-success = Triggered { $total-backups } backups while playing { $game }
back-up-during-play-on-failure = Triggered { $total-backups } backups while playing { $game }, of which { $failed-backups } failed

## Restore

restore-specific-game =
    .confirm = Restaurar dados salvos para { $game }?
    .on-success = Restaurar saves de { $game } ({ $processed-size })
    .on-unchanged = Nothing new to restore for { $game }
    .on-empty = Não foram encontrados dados salvos para restaurar { $game }
    .on-failure = Salvamentos restaurados para { $game } ({ $processed-size } de { $total-size }), mas alguns salvos falharam
# Defers to `restore-specific-game.*`.
restore-last-game = Restaurar dados salvos para a última partida jogada
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = Restaurar dados salvos para os jogos selecionados
    .confirm = Restaurar os dados de { $total-games } jogos selecionados?
restore-all-games = Restaurar dados salvos para todos os jogos
    .confirm = Restaurar dados salvos para todos os jogos que Ludusavi pode encontrar?
    .on-success = Salva restaurada para { $processed-games } jogos ({ $processed-size }); clique para lista completa
    .on-failure = Salvamentos restaurados para { $processed-games } de { $total-games } jogos ({ $processed-size } de { $total-size }), mas alguns falharam; clique para lista completa

## Tags

add-tag-for-selected-games = Tag: "{ $tag }" - Adicionar para jogos selecionados
    .confirm =
        Adicionar a tag "{ $tag }" em { $total-games } selecionados { $total-games ->
            [one] jogo
           *[other] jogos
        } e remover quaisquer tags conflitantes?
remove-tag-for-selected-games = Tag: "{ $tag }" - Remover para os jogos selecionados
    .confirm =
        Remover "{ $tag }" tag para { $total-games } selecionado { $total-games ->
            [one] jogo
           *[other] jogos
        } e remover quaisquer tags conflitantes?

## Generic errors

operation-still-pending = { ludusavi } ainda está trabalhando em um pedido anterior. Por favor, tente novamente quando você ver a notificação de que acabou.
no-game-played-yet = Você não jogou nada ainda nesta sessão.
unable-to-run-ludusavi = Não foi possível executar { ludusavi }.
cannot-open-folder = Cannot open folder.
unable-to-synchronize-with-cloud = Unable to synchronize with cloud.
cloud-synchronize-conflict = Your local and cloud backups are in conflict. Open Ludusavi and perform an upload or download to resolve this.

## Settings

config-executable-path = Nome ou caminho completo do executável Ludusavi:
config-backup-path = Substituir o caminho completo para o diretório para armazenar os backups:
config-override-backup-format = Override backup format.
config-override-backup-compression = Override backup compression.
config-override-backup-retention = Override backup retention.
config-full-backup-limit = Max full backups per game:
config-differential-backup-limit = Max differential backups per full backup:
config-do-backup-on-game-stopped = Fazer backup de dados para um jogo depois de jogá-lo
config-do-restore-on-game-starting = Também restaurar dados para um jogo antes de jogá-lo
config-ask-backup-on-game-stopped = Perguntar primeiro em vez de fazer automaticamente
config-only-backup-on-game-stopped-if-pc = Só fazer isso para jogos de PC
config-retry-unrecognized-game-with-normalization = If not found, retry by normalizing the title
config-add-suffix-for-non-pc-game-names = Procure por jogos não-PC adicionando esse sufixo para seus nomes (requer uma entrada personalizada):
config-retry-non-pc-games-without-suffix = Se não for encontrado com o sufixo, tente novamente sem ele
config-do-platform-backup-on-non-pc-game-stopped = Fazer backup de dados salvos com o nome da plataforma após jogar jogos não-PC (requer entrada personalizada)
config-do-platform-restore-on-non-pc-game-starting = Também restaurar dados salvos pelo nome da plataforma antes de jogar jogos não-PC
config-ask-platform-backup-on-non-pc-game-stopped = Perguntar primeiro em vez de fazer automaticamente
config-do-backup-during-play = Back up games on an interval during play, if they would also be backed up after play without asking
config-ignore-benign-notifications = Mostrar apenas notificações de falha
config-tag-games-with-backups = Automatically tag games with backups as "{ $tag }"
config-tag-games-with-unknown-save-data = Automatically tag games with unknown save data as "{ $tag }"
label-minutes = Minutes:
option-simple = Simples
option-none = Nenhum

## Miscellaneous

initial-setup-required = Ludusavi does not seem to be installed. Please download it and then follow the plugin setup instructions.
upgrade-prompt = Install Ludusavi { $version } or newer for the best experience. Click to view the latest release.
unrecognized-game = Ludusavi does not recognize { $game }
look-up-as-other-title = Look up with another title
look-up-as-normal-title = Look up with default title
open-backup-directory = Open backup directory
