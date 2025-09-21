import React, { useEffect } from 'react';
import { useLocation, useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './PurchaseConfirmationPage.css';

const PurchaseConfirmationPage = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { user } = useAuth();
  
  // Obtener datos de la compra del estado de navegación
  const purchaseData = location.state || {};
  const { eventId, eventName, seats, quantity, totalPrice } = purchaseData;
  
  // Generar un número de confirmación aleatorio
  const confirmationNumber = `TKT-${Math.floor(100000 + Math.random() * 900000)}`;
  
  // Fecha de compra
  const purchaseDate = new Date().toLocaleDateString('es-ES', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });

  // Formatear precio
  const formatPrice = (price) => {
    if (price === undefined || price === null) return '';
    return new Intl.NumberFormat('es-ES', {
      style: 'currency',
      currency: 'EUR',
      minimumFractionDigits: 2
    }).format(price);
  };

  // Verificar si hay datos de compra
  useEffect(() => {
    if (!eventId || !seats || seats.length === 0) {
      navigate('/events');
    }
  }, [eventId, seats, navigate]);

  // Si no hay datos de compra, mostrar mensaje de carga
  if (!eventId || !seats || seats.length === 0) {
    return (
      <div className="loading-container">
        <div className="spinner"></div>
        <p>Cargando información de compra...</p>
      </div>
    );
  }

  return (
    <div className="purchase-confirmation-page">
      <div className="confirmation-container">
        <div className="confirmation-header">
          <div className="success-icon">
            <i className="fas fa-check-circle"></i>
          </div>
          <h1>¡Compra Exitosa!</h1>
          <p>Tu compra se ha completado correctamente</p>
        </div>

        <div className="confirmation-details">
          <div className="confirmation-number">
            <h3>Número de confirmación</h3>
            <p>{confirmationNumber}</p>
          </div>

          <div className="purchase-summary">
            <h3>Resumen de compra</h3>
            
            <div className="summary-item">
              <span>Evento:</span>
              <span>{eventName}</span>
            </div>
            
            <div className="summary-item">
              <span>Fecha de compra:</span>
              <span>{purchaseDate}</span>
            </div>
            
            <div className="summary-item">
              <span>Cantidad de entradas:</span>
              <span>{quantity}</span>
            </div>
            
            <div className="summary-item">
              <span>Total pagado:</span>
              <span className="total-price">{formatPrice(totalPrice)}</span>
            </div>
          </div>

          <div className="ticket-details">
            <h3>Detalles de entradas</h3>
            
            <div className="tickets-list">
              {seats.map((seat, index) => (
                <div key={index} className="ticket-item">
                  <div className="ticket-header">
                    <span className="ticket-number">Entrada #{index + 1}</span>
                    <span className="ticket-id">{confirmationNumber}-{index + 1}</span>
                  </div>
                  
                  <div className="ticket-content">
                    <div className="ticket-event-name">{eventName}</div>
                    
                    <div className="ticket-seat-info">
                      <div>
                        <strong>Sección:</strong> {seat.section}
                      </div>
                      <div>
                        <strong>Fila:</strong> {seat.row}
                      </div>
                      <div>
                        <strong>Asiento:</strong> {seat.number}
                      </div>
                    </div>
                    
                    <div className="ticket-barcode">
                      {/* Simulación de código de barras */}
                      <div className="barcode"></div>
                      <span className="barcode-number">{confirmationNumber}-{seat.id}</span>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="purchase-info">
            <h3>Información importante</h3>
            <ul>
              <li>Se ha enviado un correo electrónico con los detalles de tu compra a {user?.email}.</li>
              <li>Puedes acceder a tus entradas en cualquier momento desde tu panel de usuario.</li>
              <li>Para asistir al evento, presenta el código QR o el número de confirmación en la entrada.</li>
              <li>En caso de cualquier problema, contacta con nuestro servicio de atención al cliente.</li>
            </ul>
          </div>
        </div>

        <div className="confirmation-actions">
          <Link to="/user/tickets" className="btn btn-primary">
            Ver mis entradas
          </Link>
          <Link to="/events" className="btn btn-outline">
            Explorar más eventos
          </Link>
        </div>
      </div>
    </div>
  );
};

export default PurchaseConfirmationPage;