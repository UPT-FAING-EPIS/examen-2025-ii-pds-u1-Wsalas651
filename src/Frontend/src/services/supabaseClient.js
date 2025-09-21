import { createClient } from '@supabase/supabase-js';

// Obtener las variables de entorno
const supabaseUrl = process.env.REACT_APP_SUPABASE_URL;
const supabaseAnonKey = process.env.REACT_APP_SUPABASE_ANON_KEY;

// Validar que las variables de entorno estén definidas
if (!supabaseUrl || !supabaseAnonKey) {
  console.error('Las variables de entorno de Supabase no están configuradas correctamente.');
}

// Crear el cliente de Supabase
const supabase = createClient(supabaseUrl, supabaseAnonKey);

export default supabase;