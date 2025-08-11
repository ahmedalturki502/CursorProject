# E-Commerce Frontend (Angular)

A modern, responsive e-commerce frontend built with Angular 18+ that connects to the .NET backend API.

## 🚀 Features

- **User Authentication**: Register, login, and logout functionality
- **Product Browsing**: View products with categories and search
- **Shopping Cart**: Add, update, and remove items from cart
- **Responsive Design**: Mobile-first approach with modern UI
- **Real-time Updates**: Live cart updates and user state management
- **JWT Authentication**: Secure token-based authentication
- **Modern UI/UX**: Clean, intuitive interface with smooth animations

## 🛠️ Technology Stack

- **Angular 18+**: Latest version with standalone components
- **TypeScript**: Type-safe development
- **SCSS**: Advanced styling with variables and mixins
- **RxJS**: Reactive programming for state management
- **Angular Material**: UI components (optional, can be added)
- **HTTP Client**: API communication with interceptors

## 📁 Project Structure

```
src/
├── app/
│   ├── components/
│   │   ├── auth/
│   │   │   ├── login/
│   │   │   └── register/
│   │   ├── cart/
│   │   │   └── cart/
│   │   ├── home/
│   │   ├── layout/
│   │   └── products/
│   │       └── product-list/
│   ├── models/
│   │   ├── user.model.ts
│   │   ├── product.model.ts
│   │   ├── cart.model.ts
│   │   └── order.model.ts
│   ├── services/
│   │   ├── auth.service.ts
│   │   ├── product.service.ts
│   │   ├── cart.service.ts
│   │   └── order.service.ts
│   ├── interceptors/
│   │   └── auth.interceptor.ts
│   ├── app.component.ts
│   ├── app.config.ts
│   └── app.routes.ts
├── environments/
│   ├── environment.ts
│   └── environment.development.ts
└── styles.scss
```

## 🚀 Getting Started

### Prerequisites

- Node.js (v18 or higher)
- npm or yarn
- Angular CLI (`npm install -g @angular/cli`)

### Installation

1. **Clone the repository** (if not already done):
   ```bash
   cd FrontEnd/ecommerce-frontend
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Start the development server**:
   ```bash
   ng serve
   ```

4. **Open your browser** and navigate to `http://localhost:4200`

### Backend Connection

Make sure the .NET backend is running on `http://localhost:5000` before using the frontend.

## 🔧 Configuration

### Environment Variables

The application uses environment files for configuration:

- `src/environments/environment.ts` - Production settings
- `src/environments/environment.development.ts` - Development settings

Update the `apiUrl` in these files if your backend runs on a different port.

### API Endpoints

The frontend connects to these backend endpoints:

- **Authentication**: `/api/auth/*`
- **Products**: `/api/products`
- **Categories**: `/api/categories`
- **Cart**: `/api/cart/*`
- **Orders**: `/api/orders/*`

## 🎨 UI Components

### Layout Component
- Responsive header with navigation
- User authentication status
- Shopping cart indicator
- Footer with copyright

### Home Component
- Hero section with call-to-action
- Featured products grid
- Category navigation
- Welcome message

### Authentication Components
- **Login**: Email/password authentication
- **Register**: User registration with validation
- Form validation and error handling
- Responsive design

### Product Components
- **Product List**: Grid layout with filtering
- Product cards with images
- Add to cart functionality
- Category filtering

### Cart Component
- Shopping cart management
- Quantity updates
- Item removal
- Cart summary with totals
- Checkout button

## 🔐 Authentication Flow

1. **Registration**: Users can create new accounts
2. **Login**: JWT token-based authentication
3. **Token Storage**: Tokens stored in localStorage
4. **Auto-logout**: Automatic logout on token expiration
5. **Protected Routes**: Authentication required for cart/orders

## 📱 Responsive Design

The application is fully responsive with:
- Mobile-first approach
- CSS Grid and Flexbox layouts
- Breakpoints for tablets and desktops
- Touch-friendly interactions

## 🚀 Build and Deployment

### Development Build
```bash
ng build
```

### Production Build
```bash
ng build --configuration production
```

### Serve Production Build
```bash
ng serve --configuration production
```

## 🧪 Testing

### Unit Tests
```bash
ng test
```

### E2E Tests
```bash
ng e2e
```

## 📦 Available Scripts

- `ng serve` - Start development server
- `ng build` - Build the application
- `ng test` - Run unit tests
- `ng e2e` - Run end-to-end tests
- `ng lint` - Run linting

## 🔧 Customization

### Styling
- Global styles in `src/styles.scss`
- Component-specific styles in respective `.scss` files
- CSS variables for consistent theming
- SCSS mixins for reusable styles

### Adding New Features
1. Create new components: `ng generate component components/feature-name`
2. Add routes in `app.routes.ts`
3. Create services for API calls
4. Add models for data structures

## 🐛 Troubleshooting

### Common Issues

1. **CORS Errors**: Ensure backend CORS is configured for `http://localhost:4200`
2. **API Connection**: Verify backend is running on port 5000
3. **Build Errors**: Check Node.js and Angular CLI versions
4. **Authentication Issues**: Clear localStorage and try logging in again

### Debug Mode
Enable debug logging in the browser console for detailed error information.

## 📄 License

This project is part of the E-Commerce application suite.

## 🤝 Contributing

1. Follow Angular style guide
2. Use TypeScript strict mode
3. Write unit tests for new features
4. Ensure responsive design
5. Update documentation

## 📞 Support

For issues and questions:
1. Check the troubleshooting section
2. Review the backend API documentation
3. Check browser console for errors
4. Verify network connectivity to backend
