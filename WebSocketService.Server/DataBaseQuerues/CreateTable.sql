CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE TABLE messages_by_diagnostic_system (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
  content TEXT,
  value REAL,
  quality bool,
  time_stamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);