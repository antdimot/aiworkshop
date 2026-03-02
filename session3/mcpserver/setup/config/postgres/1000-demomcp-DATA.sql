-- Seed example customers
INSERT INTO public.customer (FirstName, LastName) VALUES
	('John', 'Doe'),
	('Jane', 'Smith'),
	('Carlos', 'Ruiz'),
	('Mei', 'Chen'),
	('Fatima', 'Khan');

-- Seed example products
INSERT INTO public.product (code, "type", title, "description", price, quantity) VALUES
    ('P001', 'Peripheral', 'Wireless Mouse', 'Ergonomic wireless mouse with adjustable DPI.', 25.99, 150),
    ('P002', 'Peripheral', 'Mechanical Keyboard', 'RGB backlit mechanical keyboard with blue switches.', 79.99, 100),
    ('P003', 'Monitor', 'HD Monitor', '24-inch full HD monitor with ultra-thin bezels.', 149.99, 75),
    ('P004', 'Peripheral', 'USB-C Hub', 'Multi-port USB-C hub with HDMI and Ethernet.', 39.99, 200),
    ('P005', 'Storage', 'External SSD', 'Portable 1TB external SSD with fast read/write speeds.', 129.99, 50),
    ('P006', 'Storage', 'NAS', 'Network-attached storage device with 4TB capacity.', 299.99, 30),
    ('P007', 'Monitor', '4K Monitor', '27-inch 4K UHD monitor with HDR support.', 399.99, 20),
    ('P008', 'Peripheral', 'Gaming Headset', 'Surround sound gaming headset with noise-canceling mic.', 59.99, 120),
    ('P009', 'Storage', 'External HDD', 'Portable 2TB external hard drive with USB 3.0.', 89.99, 80),   
    ('P010', 'Peripheral', 'Webcam', '1080p HD webcam with built-in microphone.', 49.99, 90),
    ('P011', 'Network', 'Wireless Router', 'Wi-Fi 6 wireless router with gigabit ethernet ports.', 179.99, 40),
    ('P012', 'Network', 'Network Switch', '24-port managed network switch with PoE support.', 249.99, 25),
    ('P013', 'Network', 'WiFi 6 Access Point', 'Enterprise-grade access point with mesh capability.', 219.99, 35),
    ('P014', 'Network', 'Ethernet Cable', 'Cat6A shielded ethernet cable, 305 feet spool.', 69.99, 110);

-- seed example orders and order details
INSERT INTO public."order" (customer_id, status) VALUES
    (1, 'NEW'),
    (2, 'SENT'),
    (2, 'NEW'),
    (3, 'SENT'),
    (4, 'NEW');

INSERT INTO public.order_detail (order_id, product_id, quantity, unit_price, total_price) VALUES
    (1, 1, 2, 25.99, 51.98),
    (1, 3, 1, 149.99, 149.99),
    (2, 2, 1, 79.99, 79.99),
    (2, 4, 3, 39.99, 119.97),
    (3, 5, 1, 129.99, 129.99),
    (3, 6, 2, 299.99, 599.98),
    (3, 7, 1, 399.99, 399.99),
    (4, 8, 1, 59.99, 59.99),
    (4, 9, 2, 89.99, 179.98),
    (5, 10, 1, 49.99, 49.99);