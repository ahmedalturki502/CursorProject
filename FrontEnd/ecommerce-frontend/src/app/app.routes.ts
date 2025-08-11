import { Routes } from '@angular/router';
import { Home } from './components/home/home';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'home', component: Home },
  { 
    path: 'products', 
    loadComponent: () => import('./components/products/product-list/product-list').then(m => m.ProductList)
  },
  { 
    path: 'cart', 
    loadComponent: () => import('./components/cart/cart/cart').then(m => m.Cart)
  },
  { 
    path: 'login', 
    loadComponent: () => import('./components/auth/login/login').then(m => m.Login)
  },
  { 
    path: 'register', 
    loadComponent: () => import('./components/auth/register/register').then(m => m.Register)
  },
  { path: '**', redirectTo: '/login' }
];
