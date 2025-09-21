-- Crear tablas para el sistema de venta de entradas

-- Tabla de usuarios
CREATE TABLE users (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  email TEXT UNIQUE NOT NULL,
  password TEXT NOT NULL,
  name TEXT NOT NULL,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
  updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Tabla de eventos
CREATE TABLE events (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  title TEXT NOT NULL,
  description TEXT,
  location TEXT NOT NULL,
  date TIMESTAMP WITH TIME ZONE NOT NULL,
  category TEXT,
  image_url TEXT,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
  updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Tabla de asientos
CREATE TABLE seats (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  event_id UUID NOT NULL REFERENCES events(id) ON DELETE CASCADE,
  row TEXT NOT NULL,
  number INTEGER NOT NULL,
  price DECIMAL(10, 2) NOT NULL,
  is_available BOOLEAN DEFAULT TRUE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
  updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
  UNIQUE(event_id, row, number)
);

-- Tabla de tickets
CREATE TABLE tickets (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID NOT NULL REFERENCES users(id),
  seat_id UUID NOT NULL REFERENCES seats(id),
  purchase_date TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
  status TEXT NOT NULL DEFAULT 'active',
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
  updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
  UNIQUE(seat_id)
);

-- Habilitar la extensión para generar UUIDs
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Habilitar Row Level Security (RLS)
ALTER TABLE users ENABLE ROW LEVEL SECURITY;
ALTER TABLE events ENABLE ROW LEVEL SECURITY;
ALTER TABLE seats ENABLE ROW LEVEL SECURITY;
ALTER TABLE tickets ENABLE ROW LEVEL SECURITY;

-- Políticas de seguridad para usuarios
-- Solo el propio usuario puede ver y editar su información
CREATE POLICY "Users can view their own data" ON users
  FOR SELECT USING (auth.uid() = id);

CREATE POLICY "Users can update their own data" ON users
  FOR UPDATE USING (auth.uid() = id);

-- Políticas de seguridad para eventos
-- Cualquiera puede ver los eventos
CREATE POLICY "Anyone can view events" ON events
  FOR SELECT USING (true);

-- Solo administradores pueden crear, actualizar o eliminar eventos
CREATE POLICY "Only admins can insert events" ON events
  FOR INSERT WITH CHECK (auth.uid() IN (SELECT id FROM users WHERE role = 'admin'));

CREATE POLICY "Only admins can update events" ON events
  FOR UPDATE USING (auth.uid() IN (SELECT id FROM users WHERE role = 'admin'));

CREATE POLICY "Only admins can delete events" ON events
  FOR DELETE USING (auth.uid() IN (SELECT id FROM users WHERE role = 'admin'));

-- Políticas de seguridad para asientos
-- Cualquiera puede ver los asientos
CREATE POLICY "Anyone can view seats" ON seats
  FOR SELECT USING (true);

-- Solo administradores pueden crear, actualizar o eliminar asientos
CREATE POLICY "Only admins can insert seats" ON seats
  FOR INSERT WITH CHECK (auth.uid() IN (SELECT id FROM users WHERE role = 'admin'));

CREATE POLICY "Only admins can update seats" ON seats
  FOR UPDATE USING (auth.uid() IN (SELECT id FROM users WHERE role = 'admin'));

CREATE POLICY "Only admins can delete seats" ON seats
  FOR DELETE USING (auth.uid() IN (SELECT id FROM users WHERE role = 'admin'));

-- Políticas de seguridad para tickets
-- Los usuarios pueden ver sus propios tickets
CREATE POLICY "Users can view their own tickets" ON tickets
  FOR SELECT USING (auth.uid() = user_id);

-- Los usuarios pueden crear sus propios tickets
CREATE POLICY "Users can create their own tickets" ON tickets
  FOR INSERT WITH CHECK (auth.uid() = user_id);

-- Los usuarios pueden actualizar sus propios tickets
CREATE POLICY "Users can update their own tickets" ON tickets
  FOR UPDATE USING (auth.uid() = user_id);

-- Función para comprar un ticket
CREATE OR REPLACE FUNCTION purchase_ticket(p_seat_id UUID, p_user_id UUID)
RETURNS UUID
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
  v_ticket_id UUID;
  v_is_available BOOLEAN;
BEGIN
  -- Verificar si el asiento está disponible
  SELECT is_available INTO v_is_available FROM seats WHERE id = p_seat_id;
  
  IF v_is_available = FALSE THEN
    RAISE EXCEPTION 'El asiento no está disponible';
  END IF;
  
  -- Marcar el asiento como no disponible
  UPDATE seats SET is_available = FALSE WHERE id = p_seat_id;
  
  -- Crear el ticket
  INSERT INTO tickets (user_id, seat_id)
  VALUES (p_user_id, p_seat_id)
  RETURNING id INTO v_ticket_id;
  
  RETURN v_ticket_id;
END;
$$;

-- Índices para mejorar el rendimiento
CREATE INDEX idx_events_date ON events(date);
CREATE INDEX idx_events_category ON events(category);
CREATE INDEX idx_seats_event_id ON seats(event_id);
CREATE INDEX idx_seats_availability ON seats(event_id, is_available);
CREATE INDEX idx_tickets_user_id ON tickets(user_id);