USE KTBECK;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Category)
BEGIN
    INSERT INTO dbo.Category (CategoryName)
    VALUES
        (N'iPhone'),
        (N'Macbook'),
        (N'Phụ kiện'),
        (N'AirPods'),
        (N'Apple Watch');
END
GO

SELECT * FROM dbo.Category;