create table bank_statement_transactions (
    transaction_id serial primary key not null,
    institution_name varchar(50) not null,
    transaction_data jsonb not null,
    created_at timestamptz default current_timestamp
)