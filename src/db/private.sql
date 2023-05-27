DROP TABLE IF EXISTS tokens;

CREATE TABLE tokens (
    token_hash TEXT NOT NULL,
    admin BOOLEAN NOT NULL
);



INSERT INTO tokens (token_hash, admin)
VALUES
    ('20e188ee80770a47aa06085cefb37b2fc24302426b76e8a7c0cbb83162b47f8e','t'), ('e42da6130b708a25b1502a0b5ffb0ed23aa9bf62c675f62ed331c241b82dd015', 'f');
