# Diagrama de Clases - Aplicacion Event Ticketing

Este diagrama muestra la estructura de clases para la aplicacion de venta de entradas para eventos, incluyendo las entidades principales y sus relaciones.

## Diagrama de Clases Principal

```mermaid
classDiagram
    class User {
        +Guid Id
        +string Email
        +string PasswordHash
        +string FirstName
        +string LastName
        +string PhoneNumber
        +DateTime RegistrationDate
        +UserRole Role
        +List~Ticket~ Tickets
        
        +UpdateProfile(firstName, lastName, phoneNumber)
        +ChangeRole(role)
        +GetFullName() string
    }

    class Event {
        +Guid Id
        +string Name
        +string Description
        +DateTime Date
        +string Location
        +string Category
        +int TotalCapacity
        +decimal BasePrice
        +bool HasNumberedSeats
        +Guid OrganizerId
        +List~Ticket~ IssuedTickets
        +List~Seat~ Seats
        
        +UpdateDetails(name, description, location)
        +GetAvailableSeats() int
        +CalculatePrice(seatId) decimal
    }

    class Ticket {
        +Guid Id
        +Guid EventId
        +Guid UserId
        +DateTime PurchaseDate
        +decimal Price
        +string Code
        +bool IsUsed
        +Guid? SeatId
        +string SeatRow
        +int SeatNumber
        +string SeatSection
        
        +MarkAsUsed()
        +GenerateQRCode() string
        +ValidateTicket() bool
    }

    class Seat {
        +Guid Id
        +Guid EventId
        +string Row
        +int Number
        +string Section
        +decimal PriceMultiplier
        +bool IsReserved
        
        +Reserve()
        +Release()
        +CalculateFinalPrice(basePrice) decimal
    }

    class UserRole {
        <<enumeration>>
        Customer
        Organizer
        Administrator
    }

    %% DTOs
    class UserDto {
        +Guid Id
        +string Email
        +string FirstName
        +string LastName
        +string Role
    }

    class EventDto {
        +Guid Id
        +string Name
        +string Description
        +DateTime Date
        +string Location
        +string Category
        +decimal BasePrice
        +bool HasNumberedSeats
        +int AvailableSeats
    }

    class TicketDto {
        +Guid Id
        +Guid EventId
        +string EventName
        +DateTime EventDate
        +string EventLocation
        +DateTime PurchaseDate
        +decimal Price
        +string Code
        +bool IsUsed
        +SeatInfoDto SeatInfo
    }

    %% Repositories
    class IUserRepository {
        <<interface>>
        +GetUserByEmailAsync(email) Task~User~
        +GetUsersByRoleAsync(role) Task~IEnumerable~User~~
        +IsEmailRegisteredAsync(email) Task~bool~
        +GetUserWithTicketsAsync(userId) Task~User~
    }

    class IEventRepository {
        <<interface>>
        +GetEventsAsync() Task~IEnumerable~Event~~
        +GetEventByIdAsync(id) Task~Event~
        +GetEventsByDateRangeAsync(start, end) Task~IEnumerable~Event~~
        +GetEventsByCategoryAsync(category) Task~IEnumerable~Event~~
    }

    class ITicketRepository {
        <<interface>>
        +GetTicketsByUserAsync(userId) Task~IEnumerable~Ticket~~
        +GetTicketsByEventAsync(eventId) Task~IEnumerable~Ticket~~
        +GetTicketByCodeAsync(code) Task~Ticket~
    }

    %% Services
    class IUserService {
        <<interface>>
        +RegisterUserAsync(email, password, firstName, lastName, phoneNumber, role) Task~User~
        +AuthenticateAsync(email, password) Task~User~
        +UpdateProfileAsync(userId, firstName, lastName, phoneNumber) Task~User~
        +GetUserByIdAsync(userId) Task~User~
    }

    class IEventService {
        <<interface>>
        +CreateEventAsync(eventDto) Task~Event~
        +GetEventsAsync() Task~IEnumerable~Event~~
        +GetEventByIdAsync(eventId) Task~Event~
        +UpdateEventAsync(eventId, eventDto) Task~Event~
        +DeleteEventAsync(eventId) Task
    }

    class ITicketService {
        <<interface>>
        +PurchaseTicketAsync(userId, eventId, seatId) Task~Ticket~
        +GetUserTicketsAsync(userId) Task~IEnumerable~Ticket~~
        +ValidateTicketAsync(code) Task~bool~
        +GetTicketByIdAsync(ticketId) Task~Ticket~
    }

    %% Controllers
    class AuthController {
        -IUserService _userService
        +Register(registerDto) Task~IActionResult~
        +Login(loginDto) Task~IActionResult~
    }

    class EventsController {
        -IEventService _eventService
        +GetEvents() Task~IActionResult~
        +GetEvent(id) Task~IActionResult~
        +CreateEvent(eventDto) Task~IActionResult~
        +UpdateEvent(id, eventDto) Task~IActionResult~
        +DeleteEvent(id) Task~IActionResult~
    }

    class TicketsController {
        -ITicketService _ticketService
        +PurchaseTicket(purchaseDto) Task~IActionResult~
        +GetUserTickets() Task~IActionResult~
        +ValidateTicket(code) Task~IActionResult~
    }

    %% Relaciones de Dominio
    User ||--o{ Ticket : "compra"
    User ||--|| UserRole : "tiene"
    Event ||--o{ Ticket : "genera"
    Event ||--o{ Seat : "contiene"
    Seat ||--o| Ticket : "asignado_a"
    
    %% Relaciones de Servicios
    AuthController --> IUserService
    EventsController --> IEventService
    TicketsController --> ITicketService
    
    %% Relaciones de Repositorios
    IUserService --> IUserRepository
    IEventService --> IEventRepository
    ITicketService --> ITicketRepository
```

## Diagrama de Secuencia - Registro y AutenticaciÃ³n

```mermaid
sequenceDiagram
    participant U as Usuario
    participant AC as AuthController
    participant US as UserService
    participant UR as UserRepository
    participant DB as Base de Datos

    Note over U,DB: Proceso de Registro
    U->>AC: POST /api/auth/register
    AC->>US: RegisterUserAsync(userData)
    US->>UR: IsEmailRegisteredAsync(email)
    UR->>DB: SELECT * FROM users WHERE email = ?
    DB-->>UR: Resultado
    UR-->>US: bool
    
    alt Email no registrado
        US->>UR: AddAsync(newUser)
        UR->>DB: INSERT INTO users
        DB-->>UR: Usuario creado
        UR-->>US: User
        US-->>AC: User
        AC-->>U: 201 Created + JWT Token
    else Email ya existe
        US-->>AC: Exception
        AC-->>U: 400 Bad Request
    end

    Note over U,DB: Proceso de Login
    U->>AC: POST /api/auth/login
    AC->>US: AuthenticateAsync(email, password)
    US->>UR: GetUserByEmailAsync(email)
    UR->>DB: SELECT * FROM users WHERE email = ?
    DB-->>UR: User data
    UR-->>US: User
    
    alt Credenciales vÃ¡lidas
        US-->>AC: User
        AC-->>U: 200 OK + JWT Token
    else Credenciales invÃ¡lidas
        US-->>AC: null
        AC-->>U: 401 Unauthorized
    end
```

## Diagrama de Secuencia - Compra de Entradas

```mermaid
sequenceDiagram
    participant U as Usuario
    participant TC as TicketsController
    participant TS as TicketService
    participant ES as EventService
    participant TR as TicketRepository
    participant ER as EventRepository
    participant SR as SeatRepository
    participant DB as Base de Datos

    U->>TC: POST /api/tickets/purchase
    TC->>TS: PurchaseTicketAsync(userId, eventId, seatId)
    
    TS->>ES: GetEventByIdAsync(eventId)
    ES->>ER: GetByIdAsync(eventId)
    ER->>DB: SELECT * FROM events WHERE id = ?
    DB-->>ER: Event data
    ER-->>ES: Event
    ES-->>TS: Event
    
    alt Evento existe y estÃ¡ disponible
        TS->>SR: GetSeatByIdAsync(seatId)
        SR->>DB: SELECT * FROM seats WHERE id = ?
        DB-->>SR: Seat data
        SR-->>TS: Seat
        
        alt Asiento disponible
            TS->>SR: ReserveSeatAsync(seatId)
            SR->>DB: UPDATE seats SET is_reserved = true
            DB-->>SR: Success
            
            TS->>TR: AddAsync(newTicket)
            TR->>DB: INSERT INTO tickets
            DB-->>TR: Ticket created
            TR-->>TS: Ticket
            TS-->>TC: Ticket
            TC-->>U: 201 Created + Ticket details
        else Asiento no disponible
            TS-->>TC: Exception
            TC-->>U: 400 Bad Request
        end
    else Evento no existe
        ES-->>TS: null
        TS-->>TC: Exception
        TC-->>U: 404 Not Found
    end
```

## Arquitectura por Capas

```mermaid
graph TB
    subgraph "Capa de PresentaciÃ³n"
        A[Controllers]
        B[DTOs]
        C[Middleware]
    end
    
    subgraph "Capa de AplicaciÃ³n"
        D[Services]
        E[Interfaces de Servicios]
    end
    
    subgraph "Capa de Dominio"
        F[Entidades]
        G[Value Objects]
        H[Domain Services]
    end
    
    subgraph "Capa de Infraestructura"
        I[Repositories]
        J[Data Context]
        K[External Services]
    end
    
    subgraph "Base de Datos"
        L[(Supabase PostgreSQL)]
    end
    
    A --> D
    B --> A
    C --> A
    D --> E
    E --> F
    D --> I
    I --> J
    J --> L
    K --> L
    
    style A fill:#e1f5fe
    style D fill:#f3e5f5
    style F fill:#e8f5e8
    style I fill:#fff3e0
```

## Patrones de Diseño Implementados

### 1. Repository Pattern
- **PropÃ³sito**: Encapsula la lÃ³gica de acceso a datos
- **ImplementaciÃ³n**: `IUserRepository`, `IEventRepository`, `ITicketRepository`
- **Beneficios**: SeparaciÃ³n de responsabilidades, testabilidad

### 2. Service Layer Pattern
- **PropÃ³sito**: Encapsula la lÃ³gica de negocio
- **ImplementaciÃ³n**: `UserService`, `EventService`, `TicketService`
- **Beneficios**: ReutilizaciÃ³n de cÃ³digo, mantenibilidad

### 3. Data Transfer Object (DTO)
- **PropÃ³sito**: Transferencia de datos entre capas
- **ImplementaciÃ³n**: `UserDto`, `EventDto`, `TicketDto`
- **Beneficios**: Desacoplamiento, control de datos expuestos

### 4. Dependency Injection
- **PropÃ³sito**: InversiÃ³n de control y gestiÃ³n de dependencias
- **ImplementaciÃ³n**: Interfaces inyectadas en constructores
- **Beneficios**: Testabilidad, flexibilidad, bajo acoplamiento

### 5. Domain-Driven Design (DDD)
- **PropÃ³sito**: Modelado del dominio de negocio
- **ImplementaciÃ³n**: Entidades ricas, value objects, servicios de dominio
- **Beneficios**: CÃ³digo expresivo, mantenibilidad

## Principios SOLID

### Single Responsibility Principle (SRP)
- Cada clase tiene una Ãºnica responsabilidad
- `UserService` solo maneja operaciones de usuario
- `TicketService` solo maneja operaciones de tickets

### Open/Closed Principle (OCP)
- Clases abiertas para extensiÃ³n, cerradas para modificaciÃ³n
- Uso de interfaces permite extensibilidad sin modificar cÃ³digo existente

### Liskov Substitution Principle (LSP)
- Las implementaciones pueden sustituir a sus interfaces
- Cualquier implementaciÃ³n de `IUserRepository` puede usarse indistintamente

### Interface Segregation Principle (ISP)
- Interfaces especÃ­ficas y cohesivas
- `IUserService`, `IEventService`, `ITicketService` son interfaces especializadas

### Dependency Inversion Principle (DIP)
- Dependencia de abstracciones, no de concreciones
- Controllers dependen de interfaces de servicios, no de implementaciones concretas

---

*Diagrama generado automaticamente para la aplicaciÃ³n Event Ticketing*
*Fecha de generacion: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")*
