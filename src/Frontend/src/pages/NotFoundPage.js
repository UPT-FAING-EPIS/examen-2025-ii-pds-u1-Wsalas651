import React from 'react';
import { Link } from 'react-router-dom';
import './NotFoundPage.css';

const NotFoundPage = () => {
  return (
    <div className="not-found-page">
      <div className="not-found-container">
        <div className="not-found-content">
          <h1 className="error-code">404</h1>
          <h2 className="error-title">Página No Encontrada</h2>
          <p className="error-message">
            Lo sentimos, la página que estás buscando no existe o ha sido movida.
          </p>
          <div className="error-actions">
            <Link to="/" className="btn-primary">
              Volver al Inicio
            </Link>
            <Link to="/events" className="btn-secondary">
              Ver Eventos
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default NotFoundPage;