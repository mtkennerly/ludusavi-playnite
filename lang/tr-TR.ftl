ludusavi = Ludusavi
file-filter-executable = Yürütülebilir
button-browse = Göz at
button-open = Aç
button-yes = Evet
button-yes-remembered = Evet, her zaman
button-no = Hayır
button-no-remembered = Hayır, asla
label-launch = Başlat
badge-failed = BAŞARISIZ
badge-ignored = GÖZ ARDI EDİLMİŞ
needs-custom-entry =
    { $total-games ->
        [one] Bu oyun şunu gerektirir:
       *[other] Bazı oyunlar şunu gerektirir:
    } Ludusavi'de eşleşen özel bir giriş.

## Backup

back-up-specific-game =
    .confirm = { $game } için kayıt verileri yedeklensin mi?
    .on-success = { $game } için kayıt verileri yedeklendi ({ $processed-size })
    .on-unchanged = { $game } için yedeklenecek yeni bir şey yok
    .on-empty = { $game } için yedeklenecek kayıt verisi bulunamadı
    .on-failure = { $game } için kayıt verileri yedeklendi ({ $processed-size } / { $total-size }), ancak bazı kayıt verileri başarısız oldu
# Defers to `back-up-specific-game.*`.
back-up-last-game = Oynanan son oyun için kayıt verilerini yedekle
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = Seçilen oyunlar için kayıt verilerini yedekle
    .confirm = Seçilen { $total-games } oyun için kayıt verileri yedeklensin mi?
back-up-all-games = Tüm oyunlar için kayıt verilerini yedekle
    .confirm = Ludusavi'nin bulabildiği tüm oyunlar için kayıt verileri yedeklensin mi?
    .on-success = { $processed-games } oyun için kayıtlar yedeklendi ({ $processed-size }); tam liste için tıklayın
    .on-failure = { $total-games } oyunun kayıtlarından { $processed-games } oyunun kayıtları başarıyla yedeklendi ({ $processed-size } / { $total-size }), ancak bazıları başarısız oldu; tam listeyi görmek için tıklayın
back-up-during-play-on-success = { $game } oynarken { $total-backups } yedekleme tetiklendi
back-up-during-play-on-failure = { $game } oynarken { $total-backups } yedekleme tetiklendi, { $failed-backups } yedekleme işlemi başarısız oldu

## Restore

restore-specific-game =
    .confirm = { $game } için kayıt verileri geri yüklensin mi?
    .on-success = { $game } için kayıt verileri geri yüklendi ({ $processed-size })
    .on-unchanged = { $game } için geri yüklenecek yeni bir şey yok
    .on-empty = { $game } için geri yüklenecek kayıt verisi bulunamadı
    .on-failure = { $game } için kayıt verileri geri yüklendi ({ $processed-size } / { $total-size }), ancak bazı kayıt verileri başarısız oldu
# Defers to `restore-specific-game.*`.
restore-last-game = Oynanan son oyun için kayıt verilerini geri yükle
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = Seçilen oyunlar için kayıt verilerini geri yükle
    .confirm = Seçilen { $total-games } oyun için kayıt verileri geri yüklensin mi?
restore-all-games = Tüm oyunlar için kayıt verilerini geri yükle
    .confirm = Ludusavi'nin bulabildiği tüm oyunlar için kayıt verileri geri yüklensin mi?
    .on-success = { $processed-games } oyun için kayıtlar geri yüklendi ({ $processed-size }); tam liste için tıklayın
    .on-failure = { $total-games } oyunun kayıtlarından { $processed-games } oyunun kayıtları başarıyla geri yüklendi ({ $processed-size } / { $total-size }), ancak bazıları başarısız oldu; tam listeyi görmek için tıklayın

## Tags

add-tag-for-selected-games = Etiket: "{ $tag }" - Seçilen oyunlara ekle
    .confirm =
        "{ $tag }" etiketi seçilen { $total-games } { $total-games ->
            [one] oyuna
           *[other] oyuna
        } eklensin ve çakışan etiketler kaldırılsın mı?
remove-tag-for-selected-games = Etiket: "{ $tag }" - Seçilen oyunlardan kaldır
    .confirm =
        "{ $tag }" etiketi seçilen { $total-games } { $total-games ->
            [one] oyundan
           *[other] oyundan
        } kaldırılsın ve çakışan etiketler kaldırılsın mı?

## Generic errors

operation-still-pending = Ludusavi hâlâ önceki bir talep üzerinde çalışıyor. İşlemin tamamlandığına dair bildirimi gördüğünüzde lütfen tekrar deneyin.
no-game-played-yet = Bu oturumda henüz hiçbir oyun oynamadınız.
unable-to-run-ludusavi = Ludusavi çalıştırılamıyor.
cannot-open-folder = Klasör açılamıyor.
unable-to-synchronize-with-cloud = Bulut eşitlemesi yapılamıyor.
cloud-synchronize-conflict = Yerel ve bulut yedeklemeleriniz çakışıyor. Ludusavi'yi açın ve bir yükleme ya da indirme yaparak çözüm sağlayın.

## Settings

config-executable-path = Ludusavi yürütülebilir dosyasının adı veya tam yolu:
config-backup-path = Yedeklerin saklanacağı dizin için tam yolu geçersiz kıl:
config-override-backup-format = Yedekleme biçimini geçersiz kıl.
config-override-backup-compression = Yedek sıkıştırmayı geçersiz kıl.
config-override-backup-retention = Yedek tutmayı geçersiz kıl.
config-full-backup-limit = Oyun başına maksimum tam yedek sayısı:
config-differential-backup-limit = Her tam yedekleme için maksimum fark yedeği sayısı:
config-do-backup-on-game-stopped = Bir oyunu oynadıktan sonra kayıt verilerini yedekle
config-do-restore-on-game-starting = Ayrıca bir oyunu oynamadan önce kayıt verilerini geri yükle
config-ask-backup-on-game-stopped = Otomatik olarak yapmak yerine önce sor
config-only-backup-on-game-stopped-if-pc = Bunu sadece PC oyunları için yap
config-retry-unrecognized-game-with-normalization = Bulunamazsa, başlığı normalleştirerek yeniden deneyin
config-add-suffix-for-non-pc-game-names = PC dışı oyunları adlarına bu son eki ekleyerek arayın (özel giriş gerektirir):
config-retry-non-pc-games-without-suffix = Sonek ile bulunamazsa, sonek olmadan tekrar deneyin
config-do-platform-backup-on-non-pc-game-stopped = PC dışı oyunları oynadıktan sonra kayıt verilerini platform adına göre yedekle (özel giriş gerektirir)
config-do-platform-restore-on-non-pc-game-starting = Ayrıca PC dışı oyunları oynamadan önce kayıt verilerini platform adına göre geri yükle
config-ask-platform-backup-on-non-pc-game-stopped = Otomatik olarak yapmak yerine önce sor
config-do-backup-during-play = Oyun sırasında belirli aralıklarla oyunları yedekleyin, eğer oyundan sonra da sorulmadan yedekleneceklerse
config-ignore-benign-notifications = Yalnızca başarısızlık durumunda bildirimleri göster
config-tag-games-with-backups = Yedekleri olan oyunları otomatik olarak "{ $tag }" şeklinde etiketleyin
config-tag-games-with-unknown-save-data = Bilinmeyen kayıt verilerine sahip oyunları otomatik olarak "{ $tag }" şeklinde etiketleyin
config-check-app-update = Ludusavi güncellemelerini otomatik olarak kontrol et
config-ask-when-multiple-games-are-running = Birden fazla oyun çalışırken onay gerektir
label-minutes = Dakika:
option-simple = Basit
option-none = Hiçbiri

## Miscellaneous

initial-setup-required = Ludusavi yüklü görünmüyor. Lütfen indirin ve ardından eklenti kurulum talimatlarını izleyin.
upgrade-prompt = En iyi deneyim için Ludusavi { $version } veya daha yenisini yükleyin. Son sürümü görüntülemek için tıklayın.
upgrade-available = Ludusavi { $version } artık kullanılabilir. Sürüm notlarını görüntülemek için tıklayın.
unrecognized-game = Ludusavi { $game } öğesini tanımıyor
look-up-as-other-title = Başka bir başlıkla ara
look-up-as-normal-title = Varsayılan başlıkla ara
open-backup-directory = Yedekleme dizinini aç
