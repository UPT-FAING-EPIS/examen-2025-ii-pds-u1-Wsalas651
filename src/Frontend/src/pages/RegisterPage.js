import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './AuthPages.css';

const RegisterPage = () => {
  const { register } = useAuth();
  const navigate = useNavigate();
  
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    password: '',
    confirmPassword: '',
    acceptTerms: false
  });
  
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    // Validar que las contraseñas coincidan
    if (formData.password !== formData.confirmPassword) {
      setError('Las contraseñas no coinciden.');
      return;
    }
    
    // Validar aceptación de términos
    if (!formData.acceptTerms) {
      setError('Debes aceptar los términos y condiciones para registrarte.');
      return;
    }
    
    try {
      setLoading(true);
      setError(null);
      
      await register(formData.name, formData.email, formData.password);
      
      // Redirigir al usuario a la página principal
      navigate('/');
    } catch (err) {
      setError('Error al registrar la cuenta. El correo electrónico podría estar en uso.');
      console.error('Registration error:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-container">
        <div className="auth-header">
          <h1>Crear Cuenta</h1>
          <p>Regístrate para comprar entradas y acceder a ofertas exclusivas</p>
        </div>
        
        {error && (
          <div className="alert alert-danger">
            <i className="fas fa-exclamation-circle"></i>
            {error}
          </div>
        )}
        
        <form onSubmit={handleSubmit} className="auth-form">
          <div className="form-group">
            <label htmlFor="name">Nombre completo</label>
            <input
              type="text"
              id="name"
              name="name"
              value={formData.name}
              onChange={handleChange}
              className="form-control"
              placeholder="Tu nombre completo"
              required
            />
          </div>
          
          <div className="form-group">
            <label htmlFor="email">Correo electrónico</label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              className="form-control"
              placeholder="tu@email.com"
              required
            />
          </div>
          
          <div className="form-group">
            <label htmlFor="password">Contraseña</label>
            <input
              type="password"
              id="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              className="form-control"
              placeholder="Crea una contraseña segura"
              required
              minLength="8"
            />
            <small className="form-text">La contraseña debe tener al menos 8 caracteres.</small>
          </div>
          
          <div className="form-group">
            <label htmlFor="confirmPassword">Confirmar contraseña</label>
            <input
              type="password"
              id="confirmPassword"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              className="form-control"
              placeholder="Repite tu contraseña"
              required
            />
          </div>
          
          <div className="form-check">
            <input
              type="checkbox"
              id="acceptTerms"
              name="acceptTerms"
              checked={formData.acceptTerms}
              onChange={handleChange}
              required
            />
            <label htmlFor="acceptTerms">
              Acepto los <Link to="/terms" target="_blank">términos y condiciones</Link> y la <Link to="/privacy" target="_blank">política de privacidad</Link>
            </label>
          </div>
          
          <button 
            type="submit" 
            className="btn btn-primary btn-block btn-lg" 
            disabled={loading}
          >
            {loading ? 'Registrando...' : 'Crear cuenta'}
          </button>
        </form>
        
        <div className="auth-footer">
          <p>¿Ya tienes una cuenta? <Link to="/login">Inicia sesión</Link></p>
        </div>
      </div>
    </div>
  );
};

export default RegisterPage;