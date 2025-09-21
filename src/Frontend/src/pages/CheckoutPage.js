import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { ticketService } from '../services/ticketService';
import './CheckoutPage.css';

const CheckoutPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  
  // Obtener datos del estado de navegación
  const { event, selectedSeats, totalPrice } = location.state || {};

  const [formData, setFormData] = useState({
    cardNumber: '',
    expiryDate: '',
    cvv: '',
    cardholderName: '',
    billingAddress: '',
    city: '',
    zipCode: ''
  });

  useEffect(() => {
    // Redirigir si no hay datos de compra
    if (!event || !selectedSeats || selectedSeats.length === 0) {
      navigate('/events');
    }
  }, [event, selectedSeats, navigate]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      // Procesar la compra
      const purchaseData = {
        eventId: event.id,
        seats: selectedSeats,
        totalAmount: totalPrice,
        paymentInfo: formData
      };

      const result = await ticketService.purchaseTickets(purchaseData);
      
      // Redirigir a página de confirmación
      navigate('/purchase-confirmation', { 
        state: { 
          purchaseId: result.id,
          event: event,
          seats: selectedSeats,
          totalPrice: totalPrice
        }
      });
    } catch (err) {
      setError('Error al procesar el pago. Por favor, intenta nuevamente.');
      console.error('Purchase error:', err);
    } finally {
      setLoading(false);
    }
  };

  if (!event || !selectedSeats) {
    return <div>Cargando...</div>;
  }

  return (
    <div className="checkout-page">
      <div className="checkout-container">
        <h1>Finalizar Compra</h1>
        
        <div className="checkout-content">
          {/* Resumen de la compra */}
          <div className="order-summary">
            <h2>Resumen del Pedido</h2>
            <div className="event-info">
              <h3>{event.name}</h3>
              <p>Fecha: {new Date(event.date).toLocaleDateString()}</p>
              <p>Ubicación: {event.location}</p>
            </div>
            
            <div className="seats-info">
              <h4>Asientos Seleccionados:</h4>
              <ul>
                {selectedSeats.map((seat, index) => (
                  <li key={index}>
                    Fila {seat.row}, Asiento {seat.number} - ${seat.price}
                  </li>
                ))}
              </ul>
            </div>
            
            <div className="total-price">
              <strong>Total: ${totalPrice}</strong>
            </div>
          </div>

          {/* Formulario de pago */}
          <div className="payment-form">
            <h2>Información de Pago</h2>
            {error && <div className="error-message">{error}</div>}
            
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label htmlFor="cardNumber">Número de Tarjeta</label>
                <input
                  type="text"
                  id="cardNumber"
                  name="cardNumber"
                  value={formData.cardNumber}
                  onChange={handleInputChange}
                  placeholder="1234 5678 9012 3456"
                  required
                />
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="expiryDate">Fecha de Vencimiento</label>
                  <input
                    type="text"
                    id="expiryDate"
                    name="expiryDate"
                    value={formData.expiryDate}
                    onChange={handleInputChange}
                    placeholder="MM/YY"
                    required
                  />
                </div>
                <div className="form-group">
                  <label htmlFor="cvv">CVV</label>
                  <input
                    type="text"
                    id="cvv"
                    name="cvv"
                    value={formData.cvv}
                    onChange={handleInputChange}
                    placeholder="123"
                    required
                  />
                </div>
              </div>

              <div className="form-group">
                <label htmlFor="cardholderName">Nombre del Titular</label>
                <input
                  type="text"
                  id="cardholderName"
                  name="cardholderName"
                  value={formData.cardholderName}
                  onChange={handleInputChange}
                  placeholder="Juan Pérez"
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="billingAddress">Dirección de Facturación</label>
                <input
                  type="text"
                  id="billingAddress"
                  name="billingAddress"
                  value={formData.billingAddress}
                  onChange={handleInputChange}
                  placeholder="Calle Principal 123"
                  required
                />
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="city">Ciudad</label>
                  <input
                    type="text"
                    id="city"
                    name="city"
                    value={formData.city}
                    onChange={handleInputChange}
                    placeholder="Lima"
                    required
                  />
                </div>
                <div className="form-group">
                  <label htmlFor="zipCode">Código Postal</label>
                  <input
                    type="text"
                    id="zipCode"
                    name="zipCode"
                    value={formData.zipCode}
                    onChange={handleInputChange}
                    placeholder="15001"
                    required
                  />
                </div>
              </div>

              <div className="form-actions">
                <button 
                  type="button" 
                  className="btn-secondary"
                  onClick={() => navigate(-1)}
                >
                  Volver
                </button>
                <button 
                  type="submit" 
                  className="btn-primary"
                  disabled={loading}
                >
                  {loading ? 'Procesando...' : 'Completar Compra'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CheckoutPage;