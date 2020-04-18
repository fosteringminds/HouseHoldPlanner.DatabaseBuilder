create table migration_log (
    migration_log_id serial primary key not null,
    migration_log jsonb(50) not null,
    created_at timestamptz default current_timestamp
)