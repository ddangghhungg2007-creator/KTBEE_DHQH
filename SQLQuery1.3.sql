
USE KTBECK;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    PRINT N'BẮT ĐẦU XÓA DỮ LIỆU...';

    -- 1. Xóa chi tiết đơn hàng
    DELETE FROM dbo.OrderDetail;

    -- 2. Xóa đơn hàng
    DELETE FROM dbo.[Order];

    -- 3. Xóa khách hàng
    DELETE FROM dbo.Customer;

    -- 4. Xóa tài khoản Customer, giữ lại Admin
    DELETE FROM dbo.[User]
    WHERE UserRole = N'C';

    -- 5. Xóa sản phẩm
    DELETE FROM dbo.Product;

    -- 6. Xóa danh mục
    DELETE FROM dbo.Category;

    PRINT N'ĐÃ XÓA DỮ LIỆU. ĐANG ĐẶT LẠI ID...';

    -- 7. Đặt lại IDENTITY cho các bảng có cột tự tăng
    DECLARE @TableName NVARCHAR(256);
    DECLARE @SQL NVARCHAR(MAX);

    DECLARE db_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT DISTINCT
        QUOTENAME(OBJECT_SCHEMA_NAME(object_id))
        + N'.' +
        QUOTENAME(OBJECT_NAME(object_id))
    FROM sys.identity_columns
    WHERE object_id IN
    (
        OBJECT_ID(N'dbo.OrderDetail'),
        OBJECT_ID(N'dbo.Order'),
        OBJECT_ID(N'dbo.Customer'),
        OBJECT_ID(N'dbo.Product'),
        OBJECT_ID(N'dbo.Category')
    );

    OPEN db_cursor;

    FETCH NEXT FROM db_cursor INTO @TableName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @SQL =
            N'DBCC CHECKIDENT (''' +
            REPLACE(@TableName, '''', '''''') +
            N''', RESEED, 0) WITH NO_INFOMSGS;';

        EXEC sys.sp_executesql @SQL;

        FETCH NEXT FROM db_cursor INTO @TableName;
    END;

    CLOSE db_cursor;
    DEALLOCATE db_cursor;

    -- 8. Hoàn tất
    COMMIT TRANSACTION;

    PRINT N'================================';
    PRINT N'XÓA DỮ LIỆU THÀNH CÔNG!';
    PRINT N'ĐÃ GIỮ LẠI TÀI KHOẢN ADMIN.';
    PRINT N'================================';

END TRY

BEGIN CATCH

    IF CURSOR_STATUS('local', 'db_cursor') >= 0
        CLOSE db_cursor;

    IF CURSOR_STATUS('local', 'db_cursor') >= -1
        DEALLOCATE db_cursor;

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'XÓA DỮ LIỆU THẤT BẠI!';
    PRINT ERROR_MESSAGE();

END CATCH;
GO

-- 9. Kiểm tra dữ liệu sau khi xóa
SELECT N'OrderDetail' AS TenBang, COUNT(*) AS SoLuong
FROM dbo.OrderDetail

UNION ALL

SELECT N'Order', COUNT(*)
FROM dbo.[Order]

UNION ALL

SELECT N'Customer', COUNT(*)
FROM dbo.Customer

UNION ALL

SELECT N'User Admin', COUNT(*)
FROM dbo.[User]
WHERE UserRole = N'A'

UNION ALL

SELECT N'User Customer', COUNT(*)
FROM dbo.[User]
WHERE UserRole = N'C'

UNION ALL

SELECT N'Product', COUNT(*)
FROM dbo.Product

UNION ALL

SELECT N'Category', COUNT(*)
FROM dbo.Category;
GO


USE KTBECK;
GO

SELECT * FROM dbo.[User];
GO