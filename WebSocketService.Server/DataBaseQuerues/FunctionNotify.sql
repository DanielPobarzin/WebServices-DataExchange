CREATE OR REPLACE FUNCTION notify_trigger() 
RETURNS TRIGGER AS $trigger$
DECLARE
  rec RECORD;          -- ���������� ��� �������� ������ (NEW ��� OLD)
  payload TEXT;        -- ���������� ��� �������� JSON-������
  channel_name TEXT;   -- ��� ������ ��� �����������
  payload_items JSON;  -- JSON-������������� ������
BEGIN
  -- ����������� �������� (INSERT, UPDATE, DELETE)
  CASE TG_OP
    WHEN 'INSERT', 'UPDATE' THEN
      rec := NEW;  -- ��� INSERT � UPDATE ���������� NEW
    WHEN 'DELETE' THEN
      rec := OLD;  -- ��� DELETE ���������� OLD
    ELSE
      RAISE EXCEPTION 'Unknown TG_OP: "%". Should not occur!', TG_OP;
  END CASE;

  -- �������� ������� ����� ������
  IF TG_ARGV[0] IS NULL THEN
    RAISE EXCEPTION 'A Channel Name is required as first argument';
  END IF;

  -- �������� ��� ������ �� ���������� ��������
  channel_name := TG_ARGV[0];

  -- ����������� ������ � JSON
  payload_items := row_to_json(rec);

  -- ��������� �������� JSON-�����
  payload := json_build_object(
    'timestamp', CURRENT_TIMESTAMP,  -- ������� �����
    'row_version', rec.xmin,         -- ������ ������ (xmin)
    'operation', TG_OP,              -- ��� �������� (INSERT, UPDATE, DELETE)
    'schema', TG_TABLE_SCHEMA,       -- ����� �������
    'table', TG_TABLE_NAME,          -- ��� �������
    'payload', payload_items         -- ������ ������
  );

  -- ���������� ����������� � �����
  PERFORM pg_notify(channel_name, payload);

  -- ���������� ������ (NEW ��� OLD)
  RETURN rec;
END;
$trigger$ LANGUAGE plpgsql;