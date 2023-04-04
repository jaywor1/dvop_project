DROP TABLE IF EXISTS atms;
DROP TABLE IF EXISTS branch CASCADE;
DROP TABLE IF EXISTS employes;

CREATE TABLE atms (
    atm_id SERIAL PRIMARY KEY NOT NULL,
    stock INT NOT NULL,
    withdraw_log TEXT NULL,
    error_log TEXT NULL
);

CREATE TABLE branch (
    branch_id SERIAL NOT NULL,
	PRIMARY KEY(branch_id),
    open_hours TIME NOT NULL,
    close_hours TIME NOT NULL,
    address TEXT NOT NULL
);

CREATE TABLE employes(
    employe_id SERIAL PRIMARY KEY NOT NULL,
    branch_id INT,
    FOREIGN KEY (branch_id) REFERENCES branch (branch_id),
    name TEXT NOT NULL,
    position TEXT NOT NULL,
    present BOOLEAN NOT NULL
);

INSERT INTO atms (stock)
VALUES
    (20000),(21000),(23400),(20940),(15000),(17500);

INSERT INTO atms (stock, error_log)
VALUES
    (20000, 'Skill issue'),(21000, 'Keyboard not working'),(23400, 'Windows update');

INSERT INTO atms (stock, withdraw_log)
VALUES
    (1540, '500\n4500\n200'),(21000, '300\n400\n500'),(23400, '700\n900\n1300\n9000');

INSERT INTO branch (open_hours, close_hours, address)
    VALUES
    ('06:30:00', '17:30:00', 'Preslova 25'),
    ('07:30:00', '19:30:00', 'Revolucni 49'),
    ('06:00:00', '21:30:00', 'Matousova 13');

INSERT INTO employes (branch_id, name, position, present)
    VALUES
    (1, 'Jonathan Davis', 'manager', 't'),
    (1, 'James Shaffer', 'IT support', 't'),
    (3, 'Reginald Arvizu', 'director', 'f'),
    (2, 'Brian Welch', 'IT support', 't'),
    (1, 'Ray Luzier', 'accountant', 'f'),
	(2, 'Eric Whitney', 'director', 'f'),
    (1, 'Corey Taylor', 'director', 't')
;