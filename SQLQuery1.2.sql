USE KTBECK;
GO

SELECT
    fk.name AS TenKhoaNgoai,
    OBJECT_NAME(fk.parent_object_id) AS BangCon,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS CotBangCon,
    OBJECT_NAME(fk.referenced_object_id) AS BangCha,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS CotBangCha,
    fk.delete_referential_action_desc AS KieuXoa
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc
    ON fk.object_id = fkc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) IN ('OrderDetail')
   OR OBJECT_NAME(fk.referenced_object_id) IN ('Order');