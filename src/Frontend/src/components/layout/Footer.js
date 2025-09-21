import React from 'react';
import { Link } from 'react-router-dom';
import './Footer.css';

const Footer = () => {
  return (
    <footer className="footer">
      <div className="footer-container">
        <div className="footer-section">
          <h3>EventTicketing</h3>
          <p>Tu plataforma para comprar entradas a los mejores eventos.</p>
        </div>
        
        <div className="footer-section">
          <h4>Enlaces</h4>
          <ul className="footer-links">
            <li><Link to="/">Inicio</Link></li>
            <li><Link to="/events">Eventos</Link></li>
            <li><Link to="/login">Iniciar Sesión</Link></li>
            <li><Link to="/register">Registrarse</Link></li>
          </ul>
        </div>
        
        <div className="footer-section">
          <h4>Categorías</h4>
          <ul className="footer-links">
            <li><Link to="/events?category=conciertos">Conciertos</Link></li>
            <li><Link to="/events?category=teatro">Teatro</Link></li>
            <li><Link to="/events?category=deportes">Deportes</Link></li>
            <li><Link to="/events?category=conferencias">Conferencias</Link></li>
          </ul>
        </div>
        
        <div className="footer-section">
          <h4>Contacto</h4>
          <ul className="footer-contact">
            <li><i className="fas fa-map-marker-alt"></i> Av. Principal #123</li>
            <li><i className="fas fa-phone"></i> +123 456 7890</li>
            <li><i className="fas fa-envelope"></i> info@eventicketing.com</li>
          </ul>
          <div className="social-icons">
            <a href="https://facebook.com" target="_blank" rel="noopener noreferrer"><i className="fab fa-facebook"></i></a>
            <a href="https://twitter.com" target="_blank" rel="noopener noreferrer"><i className="fab fa-twitter"></i></a>
            <a href="https://instagram.com" target="_blank" rel="noopener noreferrer"><i className="fab fa-instagram"></i></a>
          </div>
        </div>
      </div>
      
      <div className="footer-bottom">
        <p>&copy; {new Date().getFullYear()} EventTicketing. Todos los derechos reservados.</p>
      </div>
    </footer>
  );
};

export default Footer;