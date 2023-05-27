    DROP TABLE IF EXISTS atms CASCADE;
    DROP TABLE IF EXISTS branch CASCADE;
    DROP TABLE IF EXISTS employes CASCADE;


    CREATE TABLE branch (
        branch_id SERIAL NOT NULL,
        PRIMARY KEY(branch_id),
        open_hours TIME NOT NULL,
        close_hours TIME NOT NULL,
        address TEXT NOT NULL
    );

    CREATE TABLE atms (
        atm_id SERIAL NOT NULL,
        PRIMARY KEY(atm_id),
		branch_id INT,
        FOREIGN KEY (branch_id) REFERENCES branch (branch_id),
        stock INT NOT NULL,
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




    INSERT INTO branch (open_hours, close_hours, address)
        VALUES
        ('06:30:00', '17:30:00', 'Preslova 25'),
        ('07:30:00', '19:30:00', 'Revolucni 49'),
        ('06:00:00', '21:30:00', 'Matousova 13');
		
		INSERT INTO atms (branch_id, stock, address)
    VALUES
        (1, 20000, 'Test 1'),(1, 21000, 'Test 1'),(2, 23400, 'Test 1'),(2, 23400, 'Test 1'),(2, 23400, 'Test 1'),(1, 20940, 'Test 1'),(3, 15000, 'Test 2'),(1, 17500, 'Test 34'),(2, 23400, 'Test 1'),(2, 4110, 'Test 1'),(2, 7100, 'Test 1'),(2, 700, 'Test 1'),(2, 100, 'Test 1'),(2, 23400, 'Test 4');

    INSERT INTO employes (branch_id, name, position, present)
        VALUES
        (1, 'Jonathan Davis', 'manager', 't'),
        (1, 'James Shaffer', 'IT support', 't'),
        (3, 'Reginald Arvizu', 'director', 'f'),
        (2, 'Brian Welch', 'IT support', 't'),
        (1, 'Ray Luzier', 'accountant', 'f'),
        (2, 'Eric Whitney', 'director', 'f'),
        (2, 'Lars Urlich', 'cleaner', 't'),
        (2, 'Jason Newsted', 'manager', 'f'),
        (2, 'Kirk Hammett', 'HR', 't'),
        (2, 'James Hetfield', 'accountant', 't'),
        (2, 'Dave Mustaine', 'accountant', 'f'),
        (2, 'Robert Trujillo', 'IT support', 't'),
        (1, 'Corey Taylor', 'director', 't')
    ;