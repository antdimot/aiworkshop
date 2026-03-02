DROP TABLE IF EXISTS public.order_detail;
DROP TABLE IF EXISTS public."order";
DROP TABLE IF EXISTS public.product;
DROP TABLE IF EXISTS public.customer;
    
CREATE TABLE public.customer (
    id                BIGSERIAL     NOT NULL,
    FirstName         VARCHAR(50)   NOT NULL,
    LastName          VARCHAR(50)   NOT NULL,
    created           TIMESTAMPTZ   NOT NULL DEFAULT now(),
    updated           TIMESTAMPTZ   NULL,
    PRIMARY KEY (id)
);

CREATE TABLE public.product (
    id                BIGSERIAL      NOT NULL,
    code              VARCHAR(50)    NOT NULL,
    type              VARCHAR(100)   NOT NULL,
    title             VARCHAR(100)   NOT NULL,
    "description"     TEXT           NOT NULL,
    price             NUMERIC(10,2)  NOT NULL DEFAULT 0.00,
    quantity          INTEGER        NOT NULL DEFAULT 0,
    is_enabled        BOOLEAN        NOT NULL DEFAULT true,
    created           TIMESTAMPTZ    NOT NULL DEFAULT now(),
    updated           TIMESTAMPTZ    NULL,
    PRIMARY KEY (id)
);

CREATE TABLE public."order" (
    id                BIGSERIAL      NOT NULL,
    customer_id       BIGINT         NOT NULL,
    status            VARCHAR(10)    NOT NULL DEFAULT 'NEW',
    created           TIMESTAMPTZ    NOT NULL DEFAULT now(),
    updated           TIMESTAMPTZ    NULL,
    PRIMARY KEY (id),
    FOREIGN KEY(customer_id) REFERENCES customer(id)
);

CREATE TABLE public.order_detail (
    id                BIGSERIAL      NOT NULL,
    order_id          BIGINT         NOT NULL,
    product_id        BIGINT         NOT NULL,
    quantity          INTEGER        NOT NULL DEFAULT 1,
    unit_price        NUMERIC(10,2)  NOT NULL DEFAULT 0.00,
    total_price       NUMERIC(10,2)  NOT NULL DEFAULT 0.00,
    created           TIMESTAMPTZ    NOT NULL DEFAULT now(),
    updated           TIMESTAMPTZ    NULL,  
    PRIMARY KEY (id),
    FOREIGN KEY(order_id) REFERENCES "order"(id),
    FOREIGN KEY(product_id) REFERENCES product(id)
);