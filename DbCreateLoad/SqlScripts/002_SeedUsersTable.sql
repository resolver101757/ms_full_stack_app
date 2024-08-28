INSERT INTO Users (Username, PasswordHash, CreatedAt, Email)
VALUES 
('admin', 'hashed_password1', GETDATE(), 'admin@example.com'),
('user1', 'hashed_password2', GETDATE(), 'user1@example.com'),
('user2', 'hashed_password3', GETDATE(), 'user2@example.com');

-- Remove the existing INSERT statement above this block
