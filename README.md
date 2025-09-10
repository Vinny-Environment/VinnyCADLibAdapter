# VinnyCADLibAdapter
Adapter for CSoft CADLib: Модель и Архив

# Установка

Файлы плагина к CSoft CADLib: Модель и Архив расположены в папке `plugins\cadlib` пакета `VinnyLibConverter`(см. [здесь](https://github.com/Vinny-Environment/VinnyLibConverter#%D1%83%D1%81%D1%82%D0%B0%D0%BD%D0%BE%D0%B2%D0%BA%D0%B0))
Плагин может быть установлен одинаково на версии стандартного CADLib и CADLib DX.
Плагин разрабатывался и тестировался на версии CADLib 3.2.520.2899 (декабрь 2023г).

Для загрузки плагина в CADLib необходимы права администратора (если путь установки CADLib системный по умолчанию в `C:\Program Files`).

1. Зайти в текстовый файл `C:\Program Files (x86)\CSoft\Model Studio CS\Viewer\bin\x64\plugins.xml`;
2. Добавить в конце в блок `Plugins` новую запись вида `<Plugin name="C:\Users\Georg\Documents\GitHub\VinnyLibBin\Debug\plugins\cadlib\VinnyCADLibLoader.dll" />`, где путь в кавычках указать полный файловый путь к библиотеке `VinnyCADLibLoader.dll` в Вашей папке с развернутым дистрибутивом VinnyLibConverter.
3. Сохранить файл и перезапустить CADLib.

Для CADLib DX действия аналогичны.
Если всё сделано корректно, то в CADLib появится лента `Vinny` с двумя командами на импорт и экспорт данных, активных при загрузке какой-либо модели и справочная команда "О плагине".

# Использование

Импорт модели осуществляется в выделенный пользователем каталог в Дереве структуры CADLib.

Экспорт данных осуществляется для видимых объектов сцены с полной иерархией до корня структуры

# Dev

Состоит из двух проектов:

* `VinnyCADLibLoader`: загрузчик в CADLib;
* `VinnyCADLibAdapter`: основная логика плагина;

В проектах используются внешние зависимости на библиотеки CADLib `C:\Program Files (x86)\CSoft\Model Studio CS\Viewer\bin\x64\`;
