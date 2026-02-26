-- Seed example customers
INSERT INTO public.customer (FirstName, LastName) VALUES
	('John', 'Doe'),
	('Jane', 'Smith'),
	('Carlos', 'Ruiz'),
	('Mei', 'Chen'),
	('Fatima', 'Khan');

-- Seed example products
INSERT INTO public.product (code, title, "description", price, quantity) VALUES
    ('P001', 'Wireless Mouse', 'Ergonomic wireless mouse with adjustable DPI.', 25.99, 150),
    ('P002', 'Mechanical Keyboard', 'RGB backlit mechanical keyboard with blue switches.', 79.99, 100),
    ('P003', 'HD Monitor', '24-inch full HD monitor with ultra-thin bezels.', 149.99, 75),
    ('P004', 'USB-C Hub', 'Multi-port USB-C hub with HDMI and Ethernet.', 39.99, 200),
    ('P005', 'External SSD', 'Portable 1TB external SSD with fast read/write speeds.', 129.99, 50),
    ('P006', 'NAS', 'Network-attached storage device with 4TB capacity.', 299.99, 30);

-- seed example orders and order details
INSERT INTO public."order" (customer_id, status) VALUES
    (1, 'NEW'),
    (2, 'SENT'),
    (3, 'NEW');

INSERT INTO public.order_detail (order_id, product_id, quantity, unit_price, total_price) VALUES
    (1, 1, 2, 25.99, 51.98),
    (1, 3, 1, 149.99, 149.99),
    (2, 2, 1, 79.99, 79.99),
    (2, 4, 3, 39.99, 119.97),
    (3, 5, 1, 129.99, 129.99);