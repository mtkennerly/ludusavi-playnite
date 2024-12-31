ludusavi = Ludusavi
file-filter-executable = Eksekutabel
button-browse = Telusuri
button-open = Buka
button-yes = Iya
button-yes-remembered = Iya, selalu
button-no = Tidak
button-no-remembered = Tidak, jangan pernah
label-launch = Luncurkan
badge-failed = GAGAL
badge-ignored = Abaikan
needs-custom-entry =
    { $total-games ->
        [one] Permainan ini membutuhkan
       *[other] Permainan lain membutuhkan
    } sebuah entri kustom di Ludusavi.

## Backup

back-up-specific-game =
    .confirm = Cadangkan data untuk { $game }?
    .on-success = Data tercadangkan untuk { $game } ({ $processed-size })
    .on-unchanged = Tidak ada yang baru untuk dicadangkan dari { $game }
    .on-empty = Tidak ditemukan data untuk dicadangkan dari { $game }
    .on-failure = Telah dicadangkan data untuk { $game } ({ $processed-size } dari { $total-size }), tapi beberapa pencadangan gagal
# Defers to `back-up-specific-game.*`.
back-up-last-game = Cadangkan data untuk permainan yang terakhir kali dimainkan
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = Cadangkan data untuk permainan yang dipilih
    .confirm = Cadangkan data untuk { $total-games } permainan yang dipilih?
back-up-all-games = Cadangkan data untuk semua permainan
    .confirm = Cadangkan data untuk semua permainan yang Ludusavi dapat temukan?
    .on-success = Berhasil mencadangkan data untuk { $processed-games } permainan ({ $processed-size }); tekan untuk daftar penuh
    .on-failure = Telah dicadangkan data untuk { $processed-games } dari { $total-games } permainan ({ $processed-size } dari { $total-size }), tapi beberapa gagal; tekan untuk daftar penuh
back-up-during-play-on-success = Telah membuat { $total-backups } cadangan saat sedang memainkan { $game }
back-up-during-play-on-failure = Telah membuat { $total-backups } cadangan saat sedang memainkan { $game }, di mana { $failed-backups } pencadangan gagal

## Restore

restore-specific-game =
    .confirm = Pulihkan data untuk { $game }?
    .on-success = Berhasil memulihkan data untuk { $game } ({ $processed-size })
    .on-unchanged = Tidak ada yang baru untuk dicadangkan dari { $game }
    .on-empty = Tidak ditemukan data untuk dipulihkan dari { $game }
    .on-failure = Berhasil memulihkan data untuk { $game } ({ $processed-size } dari { $total-size })
# Defers to `restore-specific-game.*`.
restore-last-game = Pulihkan data untuk permainan yang terakhir dimainkan
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = Pulihkan data untuk permainan yang dipilih
    .confirm = Pulihkan data untuk { $total-games } permainan yang dipilih?
restore-all-games = Pulihkan data untuk semua permainan
    .confirm = Pulihkan data untuk semua permainan yang Ludusavi bisa temukan?
    .on-success = Telah dipulihkan data untuk { $processed-games } permainan ({ $processed-size }); tekan untuk daftar penuh
    .on-failure = Telah dipulihkan data untuk { $processed-games } dari { $total-games } permainan ({ $processed-size } dari { $total-size }), tapi beberapa gagal; tekan untuk daftar penuh

## Tags

add-tag-for-selected-games = Tag: "{ $tag }" - Add for selected games
    .confirm =
        Add "{ $tag }" tag for { $total-games } selected { $total-games ->
            [one] game
           *[other] games
        } and remove any conflicting tags?
remove-tag-for-selected-games = Tag: "{ $tag }" - Remove for selected games
    .confirm =
        Remove "{ $tag }" tag for { $total-games } selected { $total-games ->
            [one] game
           *[other] games
        } and remove any conflicting tags?

## Generic errors

operation-still-pending = Ludusavi masih mengerjakan permintaan sebelumnya. Mohon coba lagi ketika Anda melihat notifakasi bahwa pekerjaan telah selesai.
no-game-played-yet = Anda belum memainkan apapun di sesi ini.
unable-to-run-ludusavi = Tidak dapat meluncurkan Ludusavi.
cannot-open-folder = Tidak dapat membuka folder.
unable-to-synchronize-with-cloud = Tidak dapat menyinkronkan dengan penyimpanan awan.
cloud-synchronize-conflict = Cadangan lokal dan awan Anda bertentangan. Buka Ludusavi dan lakukan unggahan atau unduhan untuk menyelesaikan ini.

## Settings

config-executable-path = Nama atau jalur lengkap dari eksekutable Ludusavi:
config-backup-path = Abaikan jalur lengkap ke direktori untuk menyimpan pencadangan:
config-override-backup-format = Abaikan format pencadangan.
config-override-backup-compression = Abaikan kompresi pencadangan.
config-override-backup-retention = Override backup retention.
config-full-backup-limit = Pencadangan penuh maksimal per permainan:
config-differential-backup-limit = Pencadangan pembeda maksimal per permainan:
config-do-backup-on-game-stopped = Cadangkan data untuk sebuah permainan setelah memainkannya
config-do-restore-on-game-starting = Juga pulihkan data untuk sebuah permainan sebelum memainkannya
config-ask-backup-on-game-stopped = Tanya terlebih dahulu daripada melakukannya secara otomatis
config-only-backup-on-game-stopped-if-pc = Hanya lakukan ini untuk permainan komputer
config-retry-unrecognized-game-with-normalization = Jika tidak ditemukan, coba lagi dengan menormalisasikan judulnya
config-add-suffix-for-non-pc-game-names = Look up non-PC games by adding this suffix to their names (requires custom entry):
config-retry-non-pc-games-without-suffix = If not found with the suffix, then try again without it
config-do-platform-backup-on-non-pc-game-stopped = Back up save data by platform name after playing non-PC games (requires custom entry)
config-do-platform-restore-on-non-pc-game-starting = Also restore save data by platform name before playing non-PC games
config-ask-platform-backup-on-non-pc-game-stopped = Tanya dahulu daripada melakukannya secara otomatis
config-do-backup-during-play = Back up games on an interval during play, if they would also be backed up after play without asking
config-ignore-benign-notifications = Hanya munculkan notifikasi pada kegagalan
config-tag-games-with-backups = Automatically tag games with backups as "{ $tag }"
config-tag-games-with-unknown-save-data = Automatically tag games with unknown save data as "{ $tag }"
config-check-app-update = Cek untuk pembaharuan Ludusavi secara otomatis
label-minutes = Menit:
option-simple = Sederhana
option-none = Tidak ada

## Miscellaneous

initial-setup-required = Ludusavi tidak terpasang. Mohon unduh itu lalu ikuti instruksi pemasangan.
upgrade-prompt = Pasang Ludusavi { $version } atau lebih baru untuk pengalaman tebaik. Tekan untuk melihat rilis terbaru.
upgrade-available = Ludusavi { $version } sekarang tersedia. Tekan untuk melihat catatan rilis.
unrecognized-game = Ludusavi tidak mengenali { $game }
look-up-as-other-title = Telusuri dengan judul lain
look-up-as-normal-title = Telusuri dengan judul standar
open-backup-directory = Buka direktori pencadangan
