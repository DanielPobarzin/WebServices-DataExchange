CREATE OR REPLACE FUNCTION notify_trigger() 
RETURNS TRIGGER AS $trigger$
DECLARE
  rec RECORD;          -- Переменная для хранения строки (NEW или OLD)
  payload TEXT;        -- Переменная для хранения JSON-пакета
  channel_name TEXT;   -- Имя канала для уведомления
  payload_items JSON;  -- JSON-представление строки
BEGIN
  -- Определение операции (INSERT, UPDATE, DELETE)
  CASE TG_OP
    WHEN 'INSERT', 'UPDATE' THEN
      rec := NEW;  -- Для INSERT и UPDATE используем NEW
    WHEN 'DELETE' THEN
      rec := OLD;  -- Для DELETE используем OLD
    ELSE
      RAISE EXCEPTION 'Unknown TG_OP: "%". Should not occur!', TG_OP;
  END CASE;

  -- Проверка наличия имени канала
  IF TG_ARGV[0] IS NULL THEN
    RAISE EXCEPTION 'A Channel Name is required as first argument';
  END IF;

  -- Получаем имя канала из аргументов триггера
  channel_name := TG_ARGV[0];

  -- Преобразуем строку в JSON
  payload_items := row_to_json(rec);

  -- Формируем итоговый JSON-пакет
  payload := json_build_object(
    'timestamp', CURRENT_TIMESTAMP,  -- Текущее время
    'row_version', rec.xmin,         -- Версия строки (xmin)
    'operation', TG_OP,              -- Тип операции (INSERT, UPDATE, DELETE)
    'schema', TG_TABLE_SCHEMA,       -- Схема таблицы
    'table', TG_TABLE_NAME,          -- Имя таблицы
    'payload', payload_items         -- Данные строки
  );

  -- Отправляем уведомление в канал
  PERFORM pg_notify(channel_name, payload);

  -- Возвращаем строку (NEW или OLD)
  RETURN rec;
END;
$trigger$ LANGUAGE plpgsql;