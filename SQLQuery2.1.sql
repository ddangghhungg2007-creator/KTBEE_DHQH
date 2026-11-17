USE KTBECK;
GO

SELECT
    o.OrderID AS [Mã đơn hàng],
    o.CustomerID AS [Mã khách hàng],
    c.CustomerName AS [Tên người đặt],
    c.CustomerPhone AS [Số điện thoại],
    c.CustomerEmail AS [Email],
    o.OrderDate AS [Ngày đặt],
    o.TotalAmount AS [Tổng tiền],
    o.PaymentMethod AS [Phương thức thanh toán],
    o.PaymentStatus AS [Trạng thái thanh toán],
    o.DeliveryMethod AS [Phương thức giao hàng],
    o.OrderStatus AS [Trạng thái đơn hàng],
    o.AddressDelivery AS [Địa chỉ giao hàng],
    p.ProductName AS [Tên sản phẩm],
    od.Quantity AS [Số lượng],
    od.UnitPrice AS [Đơn giá],
    od.Quantity * od.UnitPrice AS [Thành tiền]
FROM dbo.[Order] o
LEFT JOIN dbo.Customer c
    ON o.CustomerID = c.CustomerID
LEFT JOIN dbo.OrderDetail od
    ON o.OrderID = od.OrderID
LEFT JOIN dbo.Product p
    ON od.ProductID = p.ProductID
ORDER BY o.OrderID DESC;