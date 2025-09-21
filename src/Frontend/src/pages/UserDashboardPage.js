import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { ticketService } from '../services/ticketService';
import './UserDashboardPage.css';

const UserDashboardPage = () => {
  const { user, updateProfile, logout } = useAuth();
  const navigate = useNavigate();
  
  const [activeTab, setActiveTab] = useState('tickets');
  const [tickets, setTickets] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  // Formulario de perfil
  const [profileForm, setProfileForm] = useState({
    name: user?.name || '',
    email: user?.email || '',
    phone: user?.phone || ''
  });
  
  // Formulario de cambio de contraseña
  const [passwordForm, setPasswordForm] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });
  
  const [profileSuccess, setProfileSuccess] = useState(false);
  const [passwordSuccess, setPasswordSuccess] = useState(false);
  const [profileError, setProfileError] = useState(null);
  const [passwordError, setPasswordError] = useState(null);

  // Cargar tickets del usuario
  useEffect(() => {
    const fetchUserTickets = async () => {
      if (activeTab === 'tickets') {
        try {
          setLoading(true);
          const data = await ticketService.getUserTickets();
          setTickets(data);
          setError(null);
        } catch (err) {
          setError('Error al cargar tus entradas. Por favor, intenta nuevamente.');
          console.error('Error fetching user tickets:', err);
        } finally {
          setLoading(false);
        }
      }
    };

    fetchUserTickets();
  }, [activeTab]);

  // Formatear fecha
  const formatDate = (dateString) => {
    if (!dateString) return '';
    const options = { year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit' };
    return new Date(dateString).toLocaleDateString('es-ES', options);
  };

  // Manejar cambio de tab
  const handleTabChange = (tab) => {
    setActiveTab(tab);
    // Resetear mensajes de éxito y error al cambiar de tab
    setProfileSuccess(false);
    setPasswordSuccess(false);
    setProfileError(null);
    setPasswordError(null);
  };

  // Manejar cambios en el formulario de perfil
  const handleProfileChange = (e) => {
    const { name, value } = e.target;
    setProfileForm(prev => ({
      ...prev,
      [name]: value
    }));
  };

  // Manejar cambios en el formulario de contraseña
  const handlePasswordChange = (e) => {
    const { name, value } = e.target;
    setPasswordForm(prev => ({
      ...prev,
      [name]: value
    }));
  };

  // Enviar formulario de perfil
  const handleProfileSubmit = async (e) => {
    e.preventDefault();
    try {
      setLoading(true);
      await updateProfile(profileForm);
      setProfileSuccess(true);
      setProfileError(null);
    } catch (err) {
      setProfileError('Error al actualizar el perfil. Por favor, intenta nuevamente.');
      console.error('Error updating profile:', err);
    } finally {
      setLoading(false);
    }
  };

  // Enviar formulario de cambio de contraseña
  const handlePasswordSubmit = async (e) => {
    e.preventDefault();
    
    // Validar que las contraseñas coincidan
    if (passwordForm.newPassword !== passwordForm.confirmPassword) {
      setPasswordError('Las contraseñas no coinciden.');
      return;
    }
    
    try {
      setLoading(true);
      // Llamar al servicio para cambiar la contraseña
      await ticketService.changePassword(
        passwordForm.currentPassword,
        passwordForm.newPassword
      );
      
      setPasswordSuccess(true);
      setPasswordError(null);
      
      // Limpiar el formulario
      setPasswordForm({
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      });
    } catch (err) {
      setPasswordError('Error al cambiar la contraseña. Verifica que la contraseña actual sea correcta.');
      console.error('Error changing password:', err);
    } finally {
      setLoading(false);
    }
  };

  // Manejar cierre de sesión
  const handleLogout = async () => {
    try {
      await logout();
      navigate('/');
    } catch (err) {
      console.error('Error logging out:', err);
    }
  };

  // Renderizar contenido según la pestaña activa
  const renderTabContent = () => {
    switch (activeTab) {
      case 'tickets':
        return renderTicketsTab();
      case 'profile':
        return renderProfileTab();
      case 'security':
        return renderSecurityTab();
      default:
        return renderTicketsTab();
    }
  };

  // Renderizar pestaña de entradas
  const renderTicketsTab = () => {
    if (loading) {
      return (
        <div className="loading-container">
          <div className="spinner"></div>
          <p>Cargando tus entradas...</p>
        </div>
      );
    }

    if (error) {
      return (
        <div className="error-message">
          <i className="fas fa-exclamation-circle"></i>
          <p>{error}</p>
          <button className="btn btn-primary" onClick={() => setActiveTab('tickets')}>
            Reintentar
          </button>
        </div>
      );
    }

    if (tickets.length === 0) {
      return (
        <div className="no-tickets">
          <i className="fas fa-ticket-alt"></i>
          <h3>No tienes entradas</h3>
          <p>Explora eventos y compra entradas para verlas aquí.</p>
          <Link to="/events" className="btn btn-primary">
            Explorar eventos
          </Link>
        </div>
      );
    }

    return (
      <div className="tickets-container">
        <h2>Mis Entradas</h2>
        
        <div className="tickets-list">
          {tickets.map(ticket => (
            <div key={ticket.id} className="ticket-card">
              <div className="ticket-header">
                <h3>{ticket.event.name}</h3>
                <span className="ticket-status">{ticket.status}</span>
              </div>
              
              <div className="ticket-details">
                <div className="ticket-info">
                  <div className="info-item">
                    <i className="fas fa-calendar"></i>
                    <span>{formatDate(ticket.event.date)}</span>
                  </div>
                  
                  <div className="info-item">
                    <i className="fas fa-map-marker-alt"></i>
                    <span>{ticket.event.location}</span>
                  </div>
                  
                  <div className="info-item">
                    <i className="fas fa-chair"></i>
                    <span>Sección {ticket.seat.section}, Fila {ticket.seat.row}, Asiento {ticket.seat.number}</span>
                  </div>
                  
                  <div className="info-item">
                    <i className="fas fa-ticket-alt"></i>
                    <span>Código: {ticket.confirmationCode}</span>
                  </div>
                </div>
                
                <div className="ticket-actions">
                  <Link to={`/tickets/${ticket.id}`} className="btn btn-outline btn-sm">
                    Ver detalles
                  </Link>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  };

  // Renderizar pestaña de perfil
  const renderProfileTab = () => {
    return (
      <div className="profile-container">
        <h2>Mi Perfil</h2>
        
        {profileSuccess && (
          <div className="alert alert-success">
            <i className="fas fa-check-circle"></i>
            Perfil actualizado correctamente.
          </div>
        )}
        
        {profileError && (
          <div className="alert alert-danger">
            <i className="fas fa-exclamation-circle"></i>
            {profileError}
          </div>
        )}
        
        <form onSubmit={handleProfileSubmit} className="profile-form">
          <div className="form-group">
            <label htmlFor="name">Nombre completo</label>
            <input
              type="text"
              id="name"
              name="name"
              value={profileForm.name}
              onChange={handleProfileChange}
              className="form-control"
              required
            />
          </div>
          
          <div className="form-group">
            <label htmlFor="email">Correo electrónico</label>
            <input
              type="email"
              id="email"
              name="email"
              value={profileForm.email}
              onChange={handleProfileChange}
              className="form-control"
              required
            />
          </div>
          
          <div className="form-group">
            <label htmlFor="phone">Teléfono</label>
            <input
              type="tel"
              id="phone"
              name="phone"
              value={profileForm.phone}
              onChange={handleProfileChange}
              className="form-control"
            />
          </div>
          
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Guardando...' : 'Guardar cambios'}
          </button>
        </form>
      </div>
    );
  };

  // Renderizar pestaña de seguridad
  const renderSecurityTab = () => {
    return (
      <div className="security-container">
        <h2>Seguridad</h2>
        
        <div className="password-section">
          <h3>Cambiar contraseña</h3>
          
          {passwordSuccess && (
            <div className="alert alert-success">
              <i className="fas fa-check-circle"></i>
              Contraseña actualizada correctamente.
            </div>
          )}
          
          {passwordError && (
            <div className="alert alert-danger">
              <i className="fas fa-exclamation-circle"></i>
              {passwordError}
            </div>
          )}
          
          <form onSubmit={handlePasswordSubmit} className="password-form">
            <div className="form-group">
              <label htmlFor="currentPassword">Contraseña actual</label>
              <input
                type="password"
                id="currentPassword"
                name="currentPassword"
                value={passwordForm.currentPassword}
                onChange={handlePasswordChange}
                className="form-control"
                required
              />
            </div>
            
            <div className="form-group">
              <label htmlFor="newPassword">Nueva contraseña</label>
              <input
                type="password"
                id="newPassword"
                name="newPassword"
                value={passwordForm.newPassword}
                onChange={handlePasswordChange}
                className="form-control"
                required
                minLength="8"
              />
              <small className="form-text">La contraseña debe tener al menos 8 caracteres.</small>
            </div>
            
            <div className="form-group">
              <label htmlFor="confirmPassword">Confirmar nueva contraseña</label>
              <input
                type="password"
                id="confirmPassword"
                name="confirmPassword"
                value={passwordForm.confirmPassword}
                onChange={handlePasswordChange}
                className="form-control"
                required
              />
            </div>
            
            <button type="submit" className="btn btn-primary" disabled={loading}>
              {loading ? 'Actualizando...' : 'Actualizar contraseña'}
            </button>
          </form>
        </div>
        
        <div className="logout-section">
          <h3>Cerrar sesión</h3>
          <p>Cierra tu sesión en todos los dispositivos.</p>
          <button onClick={handleLogout} className="btn btn-danger">
            Cerrar sesión
          </button>
        </div>
      </div>
    );
  };

  return (
    <div className="user-dashboard-page">
      <div className="dashboard-container">
        <div className="dashboard-sidebar">
          <div className="user-info">
            <div className="user-avatar">
              {user?.name ? user.name.charAt(0).toUpperCase() : 'U'}
            </div>
            <div className="user-details">
              <h3>{user?.name || 'Usuario'}</h3>
              <p>{user?.email || ''}</p>
            </div>
          </div>
          
          <nav className="dashboard-nav">
            <button 
              className={`nav-item ${activeTab === 'tickets' ? 'active' : ''}`}
              onClick={() => handleTabChange('tickets')}
            >
              <i className="fas fa-ticket-alt"></i>
              <span>Mis Entradas</span>
            </button>
            
            <button 
              className={`nav-item ${activeTab === 'profile' ? 'active' : ''}`}
              onClick={() => handleTabChange('profile')}
            >
              <i className="fas fa-user"></i>
              <span>Mi Perfil</span>
            </button>
            
            <button 
              className={`nav-item ${activeTab === 'security' ? 'active' : ''}`}
              onClick={() => handleTabChange('security')}
            >
              <i className="fas fa-shield-alt"></i>
              <span>Seguridad</span>
            </button>
          </nav>
        </div>
        
        <div className="dashboard-content">
          {renderTabContent()}
        </div>
      </div>
    </div>
  );
};

export default UserDashboardPage;