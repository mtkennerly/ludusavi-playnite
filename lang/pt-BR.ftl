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
    } uma entrada personalizada correspondente em Ludusavi.

## Backup

back-up-specific-game =
    .confirm = Fazer backup dos dados salvos para { $game }?
    .on-success = Backup dos saves de { $game } ({ $processed-size })
    .on-unchanged = Nada de novo para salvar no { $game }
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
back-up-during-play-on-success = { $total-backups } backups feitos enquanto jogava { $game }
back-up-during-play-on-failure = { $total-backups } backups feitos enquanto jogava { $game }, { $failed-backups } falharam

## Restore

restore-specific-game =
    .confirm = Restaurar dados salvos para { $game }?
    .on-success = Restaurar saves de { $game } ({ $processed-size })
    .on-unchanged = Nada de novo para salvar no { $game }
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

operation-still-pending = Ludusavi ainda está trabalhando em um pedido anterior. Por favor, tente novamente quando você ver a notificação de que acabou.
no-game-played-yet = Você não jogou nada ainda nesta sessão.
unable-to-run-ludusavi = Não foi possível executar Ludusavi.
cannot-open-folder = Impossível abrir pasta.
unable-to-synchronize-with-cloud = Não foi possível sincronizar com a nuvem.
cloud-synchronize-conflict = Seus backups locais e da nuvem estão em conflito. Abra Ludusavi e execute um upload ou download para resolver isso.

## Settings

config-executable-path = Nome ou caminho completo do executável Ludusavi:
config-backup-path = Substituir o caminho completo para o diretório para armazenar os backups:
config-override-backup-format = Substituir formato de backup.
config-override-backup-compression = Sobrepor a compressão de backup.
config-override-backup-retention = Sobrescrever a retenção de backup.
config-full-backup-limit = Máximo de backups completos por jogo:
config-differential-backup-limit = Máximo de backups diferenciais por backup completo:
config-do-backup-on-game-stopped = Fazer backup de dados para um jogo depois de jogá-lo
config-do-restore-on-game-starting = Também restaurar dados para um jogo antes de jogá-lo
config-ask-backup-on-game-stopped = Perguntar primeiro em vez de fazer automaticamente
config-only-backup-on-game-stopped-if-pc = Só fazer isso para jogos de PC
config-retry-unrecognized-game-with-normalization = Se não for encontrado, tente novamente normalizando o título
config-add-suffix-for-non-pc-game-names = Procure por jogos não-PC adicionando esse sufixo para seus nomes (requer uma entrada personalizada):
config-retry-non-pc-games-without-suffix = Se não for encontrado com o sufixo, tente novamente sem ele
config-do-platform-backup-on-non-pc-game-stopped = Fazer backup de dados salvos com o nome da plataforma após jogar jogos não-PC (requer entrada personalizada)
config-do-platform-restore-on-non-pc-game-starting = Também restaurar dados salvos pelo nome da plataforma antes de jogar jogos não-PC
config-ask-platform-backup-on-non-pc-game-stopped = Perguntar primeiro em vez de fazer automaticamente
config-do-backup-during-play = Faça backup de jogos em um intervalo de tempo durante a partida, se eles também terão backup depois da partida sem perguntar
config-ignore-benign-notifications = Mostrar apenas notificações de falha
config-tag-games-with-backups = Marcar automaticamente jogos com backups como "{ $tag }"
config-tag-games-with-unknown-save-data = Marcar automaticamente jogos com saves desconhecidos como "{ $tag }"
config-check-app-update = Verificar por atualizações do Ludusavi automaticamente
config-ask-when-multiple-games-are-running = Exigir confirmação quando vários jogos estiverem rodando
label-minutes = Minutos:
option-simple = Simples
option-none = Nenhum

## Miscellaneous

initial-setup-required = Ludusavi parece não estar instalado. Por favor baixe-o e, em seguida, siga as instruções de configuração do plugin.
upgrade-prompt = Instale Ludusavi { $version } ou mais recente para obter a melhor experiência. Clique para ver a última versão.
upgrade-available = Ludusavi { $version } está disponível. Clique para ver as notas de lançamento.
unrecognized-game = { $game } não foi reconhecido pelo Ludusavi.
look-up-as-other-title = Procurar com outro título
look-up-as-normal-title = Procurar com título original
open-backup-directory = Abrir o diretório de backup
