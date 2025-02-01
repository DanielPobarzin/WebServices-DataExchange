CREATE OR REPLACE TRIGGER organization_notify_trigger
AFTER INSERT OR UPDATE OR DELETE ON messages_by_diagnostic_system
FOR EACH ROW EXECUTE PROCEDURE notify_trigger('core_db_event');