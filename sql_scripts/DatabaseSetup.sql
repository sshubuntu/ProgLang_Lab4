CREATE TABLE IF NOT EXISTS grade_records (
    id SERIAL PRIMARY KEY,
    student_name VARCHAR(100) NOT NULL,
    subject_name VARCHAR(100) NOT NULL,
    mark INTEGER NOT NULL CHECK (mark >= 0 AND mark <= 100)
);
