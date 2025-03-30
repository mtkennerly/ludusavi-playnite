ludusavi = Ludusavi
file-filter-executable = مِلَفّ تنفيذي
button-browse = تصفح
button-open = فتح
button-yes = نعم
button-yes-remembered = نعم، دائمًا
button-no = لا
button-no-remembered = لا، إطلاقًا
label-launch = تشغيل
badge-failed = فشل
badge-ignored = تجاهل
needs-custom-entry =
    { $total-games ->
        [one] تتطلب هذه اللعبة
       *[other] بعض الألعاب تتطلب
    } إدخال مخصص مطابق في لودسافي.

## Backup

back-up-specific-game =
    .confirm = هل ترغب في عمل نسخة احتياطية لبيانات الحفظ ل { $game }؟
    .on-success = تم النسخ الاحتياطي لحفظ { $game } ({ $processed-size })
    .on-unchanged = لا يوجد شيء جديد للنسخ الاحتياطي لـ { $game }
    .on-empty = لم يتم العثور على حفظ بيانات للنسخ الاحتياطي لـ { $game }
    .on-failure = تم النسخ الاحتياطي لحفظ { $game } ({ $processed-size } of { $total-size }) ، ولكن بعض عمليات الحفظ فشلت
# Defers to `back-up-specific-game.*`.
back-up-last-game = النسخ الاحتياطي لحفظ البيانات لآخر لعبة تم تشغيلها
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = النسخ الاحتياطي لبيانات حفظ الألعاب المحددة
    .confirm = النسخ الاحتياطي لبيانات حفظ ألعاب { $total-games } المحددة؟
back-up-all-games = Back up save data for all games
    .confirm = النسخ الاحتياطي لحفظ البيانات لجميع الألعاب التي يمكن لودوسافي العثور عليها؟
    .on-success = النسخ الاحتياطي للحفظ لـ { $processed-games } ألعاب ({ $processed-size })؛ انقر للقائمة الكاملة
    .on-failure = تم النسخ الاحتياطي لحفظ { $processed-games } من المباريات { $total-games } ({ $processed-size } of { $total-size })، لكن بعض الفشل؛ انقر للقائمة الكاملة
back-up-during-play-on-success = تم تفعيل النسخ الاحتياطية { $total-backups } أثناء تشغيل { $game }
back-up-during-play-on-failure = تم تشغيل نسخة احتياطية { $total-backups } أثناء تشغيل { $game }، فشل منها { $failed-backups }

## Restore

restore-specific-game =
    .confirm = هل تريد استعادة بيانات الحفظ ل { $game }؟
    .on-success = استعادة الحفظ ل { $game } ({ $processed-size })
    .on-unchanged = لا يوجد شيء جديد لإستعادته ل { $game }
    .on-empty = لم يتم العثور على بيانات حفظ لاستعادة { $game }
    .on-failure = استعادة الحفظ ل { $game } ({ $processed-size } of { $total-size }) ، ولكن بعض الحفظ فشل
# Defers to `restore-specific-game.*`.
restore-last-game = Restore save data for last game played
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = استعادة حفظ البيانات للألعاب المحددة
    .confirm = استعادة حفظ البيانات لـ { $total-games } ألعاب مختارة؟
restore-all-games = Restore save data for all games
    .confirm = استعادة حفظ البيانات لجميع الألعاب التي يمكن لودوسافي العثور عليها؟
    .on-success = استعادة الحفظ لألعاب { $processed-games } ({ $processed-size })؛ انقر للقائمة الكاملة
    .on-failure = استعادة الحفظ ل { $processed-games } من المباريات { $total-games } ({ $processed-size } of { $total-size }) ، لكن بعض الفشل؛ انقر للقائمة الكاملة

## Tags

add-tag-for-selected-games = العلامة: "{ $tag }" - إضافة للألعاب المحددة
    .confirm =
        إضافة "{ $tag }" علامة ل { $total-games } المحددة ، و { $total-games ->
            [one] اللعبة
           *[other] ألعاب
        } وإزالة أي علامات متضاربة؟
remove-tag-for-selected-games = العلامة: "{ $tag }" - إزالة للألعاب المحددة
    .confirm =
        إزالة "{ $tag }" علامة ل { $total-games } المحددة { $total-games ->
            [one] اللعبة
           *[other] ألعاب
        } وإزالة أي علامات متضاربة؟

## Generic errors

operation-still-pending = لا يزال لودوسافي يعمل على طلب سابق. الرجاء المحاولة مرة أخرى عندما ترى الإشعار الذي تم إنجازه.
no-game-played-yet = أنت لم تلعب أي شيء بعد في هذه الجلسة.
unable-to-run-ludusavi = غير قادر على تشغيل لودسافي.
cannot-open-folder = لا يمكن فتح المجلد.
unable-to-synchronize-with-cloud = تعذر المزامنة مع السحابة.
cloud-synchronize-conflict = النسخ الاحتياطي المحلي والسحابي مختلف. حمل أو نزل لحل هذه المشكلة.

## Settings

config-executable-path = اسم أو المسار الكامل لجهاز تشغيل لودوسافي:
config-backup-path = تجاوز المسار الكامل إلى الدليل لتخزين النسخ الاحتياطية:
config-override-backup-format = تجاوز تنسيق النسخ الاحتياطي.
config-override-backup-compression = تجاوز ضغط النسخ الاحتياطي.
config-override-backup-retention = تجاوز الاحتفاظ بالنسخ الاحتياطي.
config-full-backup-limit = الحد الأقصى للنسخ الاحتياطي الكامل لكل لعبة:
config-differential-backup-limit = الحد الأقصى للنسخ الاحتياطية التفاضلية لكل نسخة احتياطية كاملة:
config-do-backup-on-game-stopped = النسخ الاحتياطي لحفظ البيانات للعبة بعد تشغيلها
config-do-restore-on-game-starting = أيضا استعادة حفظ البيانات للعبة قبل تشغيلها
config-ask-backup-on-game-stopped = اسأل أولاً بدلاً من القيام بذلك تلقائياً
config-only-backup-on-game-stopped-if-pc = قم بهذا فقط لألعاب الكمبيوتر
config-retry-unrecognized-game-with-normalization = إذا لم يتم العثور على إعادة المحاولة عن طريق تطبيع العنوان
config-add-suffix-for-non-pc-game-names = ابحث عن ألعاب غير الكمبيوتر بإضافة هذه اللاحقة إلى أسمائهم (يتطلب إدخال مخصص):
config-retry-non-pc-games-without-suffix = إذا لم يتم العثور على مع اللاصقة، ثم حاول مرة أخرى بدونه
config-do-platform-backup-on-non-pc-game-stopped = النسخ الاحتياطي لحفظ البيانات عن طريق اسم المنصة بعد تشغيل الألعاب غير الحاسوبية (يتطلب إدخال مخصص)
config-do-platform-restore-on-non-pc-game-starting = أيضا استعادة حفظ البيانات عن طريق اسم المنصة قبل تشغيل الألعاب غير الحاسوبية
config-ask-platform-backup-on-non-pc-game-stopped = اسأل أولاً بدلاً من القيام بذلك تلقائياً
config-do-backup-during-play = النسخ الاحتياطي للألعاب على فترة زمنية أثناء اللعب، إذا كان سيتم النسخ الاحتياطي لها أيضا بعد اللعب بدون طلب
config-ignore-benign-notifications = إظهار الإشعارات فقط عند الفشل
config-tag-games-with-backups = وسم الألعاب تلقائياً مع النسخ الاحتياطي "{ $tag }"
config-tag-games-with-unknown-save-data = وسم الألعاب تلقائياً مع النسخ الاحتياطي "{ $tag }"
config-check-app-update = التحقق من وجود تحديثات التطبيق تلقائيا
label-minutes = الدقائق:
option-simple = بسيط
option-none = لا شيء

## Miscellaneous

initial-setup-required = يبدو أن لودوسافي غير مثبت. الرجاء تنزيله ثم اتبع تعليمات إعداد الإضافة.
upgrade-prompt = قم بتثبيت Ludusavi { $version } أو أحدث للحصول على أفضل تجربة. انقر لعرض أحدث إصدار.
upgrade-available = Ludusavi { $version } متاح الآن. انقر لعرض ملاحظات الإصدار.
unrecognized-game = Ludusavi does not recognize { $game }
look-up-as-other-title = Look up with another title
look-up-as-normal-title = Look up with default title
open-backup-directory = جارِ إعداد دليل النسخ الاحتياطي
