# dvop_project
Please let me pass this subject :))
## Design
### Endpoints
#### ATM
- /atm --> list of atms ATM(atm_id, stock, withdraw_log, errors)
- /atm/broken --> atms with errors
- /atm/refil  --> atms that are close running out of stock
- /atm/{atm_id}/log --> shows withdraw_log
- /atm/{atm_id}/error --> shows error log
#### Branch
- /branch --> list of branches Branch(branch_id, Employee(employe_id, position, present), opening_hours, location)
- /branch/{branch_id}/manager --> list of managers in selected branch
- /branch/director --> list of director of each branch
- /branch/opened --> opened branches
### DB tables
- employes
    - employee_id (SERIAL PRIMARY KEY NOT NULL)
    - branch_id (FOREIGN KEY INT)
    - position (TEXT NOT NULL)
    - present (BOOLEAN NOT NULL)

- branch
    - branch_id (SERIAL PRIMARY KEY NOT NULL)
    - open_hours (TIME NOT NULL)
    - close_hours (TIME NOT NULL)
    - address (TEXT NOT NULL)

- atms
    - atm_id (SERIAL PRIMARY KEY NOT NULL)
    - stock (INT NOT NULL)
    - withdraw_log (TEXT NULL)
    - error_log (TEXT NULL)